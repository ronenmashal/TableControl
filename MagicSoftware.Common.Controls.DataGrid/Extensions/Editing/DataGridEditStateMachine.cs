using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal abstract class DataGridEditStateMachine
   {
      protected ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public FrameworkElement TargetElement { get; set; }

      protected ICommandRegulationService CommandRegulator { get; private set; }

      protected ICurrentItemService CurrentItemProvider { get; private set; }

      public virtual void Cleanup()
      {
         CommandRegulator.PreviewCanExecute -= PreviewCanExecuteCommand;
         CurrentItemProvider.PreviewCurrentChanging -= DataGridEditingExtender_PreviewCurrentChanging;
      }

      public virtual void Setup()
      {
         CurrentItemProvider = UIServiceProvider.GetService<ICurrentItemService>(TargetElement);
         CommandRegulator = UIServiceProvider.GetService<ICommandRegulationService>(TargetElement);

         CommandRegulator.PreviewCanExecute += PreviewCanExecuteCommand;
         CurrentItemProvider.PreviewCurrentChanging += DataGridEditingExtender_PreviewCurrentChanging;
      }

      public override string ToString()
      {
         return "{" + GetType().Name + "}";
      }

      internal abstract void ProcessKey(KeyEventArgs e);

      protected abstract bool CanLeaveCurrentLine();

      protected abstract void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args);

      private void DataGridEditingExtender_PreviewCurrentChanging(object sender, CancelableEventArgs e)
      {
         var args = e as PreviewChangeEventArgs;
         var changeType = args.OldValue == null ? args.NewValue.GetType() : args.OldValue.GetType();

         log.DebugFormat("Processing current changing of {0} event on {1}", changeType, this);
         e.Canceled = !CanLeaveCurrentLine();
         if (e.Canceled)
         {
            log.DebugFormat("-- Canceling event.");
         }
      }
   }
}