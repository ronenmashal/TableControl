using System;
using System.Windows.Input;
using System.Windows.Threading;

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
         CurrentItemProvider.CurrentChanged -= TargetElementProxy_CurrentChanged;
         base.Cleanup();
      }

      public override void Setup()
      {
         base.Setup();
         CurrentItemProvider.CurrentChanged += TargetElementProxy_CurrentChanged;

         beginEditTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, this.TargetElement.Dispatcher);
         beginEditTimer.Interval = TimeSpan.FromMilliseconds(10);
         beginEditTimer.Tick += beginEditTimer_Tick;
         beginEditTimer.Start();
      }

      internal override void ProcessKey(KeyEventArgs e)
      {
         switch (e.Key)
         {
            case Key.Enter:
               if (EditStateService.IsEditing)
                  EditStateService.CommitEdit();
               else
                  EditStateService.BeginEdit();
               e.Handled = true;
               break;

            case Key.F2:
               if (!EditStateService.IsEditing)
                  EditStateService.BeginEdit();
               e.Handled = true;
               break;

            case Key.Escape:
               if (EditStateService.IsEditing)
                  EditStateService.CancelEdit();
               e.Handled = true;
               break;

            default:
               break;
         }
         if (!EditStateService.IsEditing)
            StartBeginEditTimer();
      }

      protected override bool CanLeaveCurrentLine()
      {
         if (EditStateService.IsEditing)
            return EditStateService.CommitEdit();

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
         if (!EditStateService.IsEditing)
         {
            if (EditStateService.BeginEdit())
               beginEditTimer.Stop();
         }
      }

      private void StartBeginEditTimer()
      {
         beginEditTimer.Start();
      }

      private void TargetElementProxy_CurrentChanged(object sender, EventArgs e)
      {
         StartBeginEditTimer();
      }
   }
}