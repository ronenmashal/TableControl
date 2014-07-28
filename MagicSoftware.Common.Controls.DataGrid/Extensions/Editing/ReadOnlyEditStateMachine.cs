using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal class ReadOnlyEditStateMachine : DataGridEditStateMachine
   {
      internal override void ProcessKey(KeyEventArgs e)
      {
      }

      protected override bool CanLeaveCurrentLine()
      {
         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         if ((args.Command == DataGrid.BeginEditCommand) || (args.Command == DataGrid.CommitEditCommand) || (args.Command == DataGrid.CancelEditCommand))
         {
            log.DebugFormat("Inhibiting command {0}", ((RoutedCommand)args.Command).Name);
            args.CanExecute = false;
            args.ContinueRouting = false;
            args.Handled = true;
         }
      }
   }
}