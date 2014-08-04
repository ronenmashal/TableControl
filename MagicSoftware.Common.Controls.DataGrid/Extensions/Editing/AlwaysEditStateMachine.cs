using System;
using System.Windows.Input;
using System.Windows.Threading;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal class AlwaysEditStateMachine : DataGridEditStateMachine
   {
      private DispatcherTimer beginEditTimer;

      private IElementEditStateService EditStateService
      {
         get
         {
            var itemsService = UIServiceProvider.GetService<ICurrentCellService>(this.TargetElement);
            var editStateService = UIServiceProvider.GetService<IElementEditStateService>(itemsService.CurrentItemContainer, false);
            if (editStateService == null)
            {
               // Current item does not have an edit state service - get the ItemsControl's edit state service.
               editStateService = UIServiceProvider.GetService<IElementEditStateService>(this.TargetElement);
            }
            return editStateService;
         }
      }

      public override void Cleanup()
      {
         beginEditTimer.Stop();
         beginEditTimer.Tick -= beginEditTimer_Tick;
         CurrentCellService.CurrentCellChanged -= TargetElementProxy_CurrentChanged;
         base.Cleanup();
      }

      public override void Setup()
      {
         base.Setup();
         CurrentCellService.CurrentCellChanged += TargetElementProxy_CurrentChanged;

         beginEditTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, this.TargetElement.Dispatcher);
         beginEditTimer.Interval = TimeSpan.FromMilliseconds(20);
         beginEditTimer.Tick += beginEditTimer_Tick;
         beginEditTimer.Start();
      }

      internal override void ProcessKey(KeyEventArgs e)
      {
      }

      IDisposable focusDeferHandler = null;

      protected override bool CanLeaveCurrentLine()
      {
         if (EditStateService.IsEditingField)
         {
            return EditStateService.CommitItemEdit();
         }

         return true;
      }

      protected override bool CanLeaveCurrentCell()
      {
         if (EditStateService.IsEditingField)
         {
            var fms = UIServiceProvider.GetService<IFocusManagementService>(TargetElement);
            focusDeferHandler = fms.DeferFocusChanges();
            return EditStateService.CommitFieldEdit();
         }

         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }

      private void beginEditTimer_Tick(object sender, EventArgs e)
      {
         // Try getting an edit state service for the current item.
         if (!EditStateService.IsEditingField)
         {
            if (EditStateService.BeginItemEdit())
            {
               beginEditTimer.Stop();
            }
         }
      }

      private void StartBeginEditTimer()
      {
         beginEditTimer.Start();
      }

      readonly AutoResetFlag waitingForKeyUp = new AutoResetFlag();

      private void TargetElementProxy_CurrentChanged(object sender, EventArgs e)
      {
         if (!waitingForKeyUp.IsSet)
         {
            if (!EditStateService.IsEditingItem)
            {
               IDisposable flagReset = waitingForKeyUp.Set();
               KeyEventHandler handler = null;
               handler = (s, a) =>
               {
                  TargetElement.PreviewKeyUp -= handler;
                  StartBeginEditTimer();
                  flagReset.Dispose();
               };
               TargetElement.PreviewKeyUp += handler;
            }
            else
            {
               EditStateService.BeginFieldEdit();
            }
            if (focusDeferHandler != null)
               focusDeferHandler.Dispose();
         }
      }
   }
}