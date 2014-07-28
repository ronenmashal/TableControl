using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal class AlwaysEditStateMachine : DataGridEditStateMachine
   {
      private DispatcherTimer beginEditTimer;
      private IElementEditStateService editProxy;

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
         editProxy = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);

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
               if (editProxy.IsEditing)
                  editProxy.CommitEdit();
               else
                  editProxy.BeginEdit();
               e.Handled = true;
               break;

            case Key.F2:
               if (!editProxy.IsEditing)
                  editProxy.BeginEdit();
               e.Handled = true;
               break;

            case Key.Escape:
               if (editProxy.IsEditing)
                  editProxy.CancelEdit();
               e.Handled = true;
               break;

            default:
               break;
         }
         if (!editProxy.IsEditing)
            StartBeginEditTimer();
      }

      protected override bool CanLeaveCurrentLine()
      {
         if (editProxy.IsEditing)
            return editProxy.CommitEdit();

         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }

      private void beginEditTimer_Tick(object sender, EventArgs e)
      {
         if (!editProxy.IsEditing)
         {
            if (editProxy.BeginEdit())
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