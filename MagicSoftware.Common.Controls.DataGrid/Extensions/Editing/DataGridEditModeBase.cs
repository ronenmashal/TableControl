using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal abstract class DataGridEditModeBase
   {
      protected ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public FrameworkElement TargetElement { get; set; }

      protected ICommandRegulationService CommandRegulator { get; private set; }

      protected ICurrentCellService CurrentCellService { get; private set; }

      public virtual void Cleanup()
      {
         CommandRegulator.PreviewCanExecute -= PreviewCanExecuteCommand;
         CurrentCellService.PreviewCurrentCellChanging -= DataGridEditingExtender_PreviewCurrentChanging;
      }

      public virtual void Setup()
      {
         CurrentCellService = UIServiceProvider.GetService<ICurrentCellService>(TargetElement);
         CommandRegulator = UIServiceProvider.GetService<ICommandRegulationService>(TargetElement);

         CommandRegulator.PreviewCanExecute += PreviewCanExecuteCommand;
         CurrentCellService.PreviewCurrentCellChanging += DataGridEditingExtender_PreviewCurrentChanging;
      }

      public override string ToString()
      {
         return "{" + GetType().Name + "}";
      }

      internal abstract void ProcessKey(KeyEventArgs e);

      protected abstract bool CanLeaveCurrentLine();

      protected virtual bool CanLeaveCurrentCell()
      {
         return true;
      }

      protected abstract void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args);

      private void DataGridEditingExtender_PreviewCurrentChanging(object sender, PreviewChangeEventArgs e)
      {
         UniversalCellInfo oldCell = (UniversalCellInfo)(e.OldValue ?? new UniversalCellInfo());
         UniversalCellInfo newCell = (UniversalCellInfo)(e.NewValue ?? new UniversalCellInfo());

         if (!CanLeaveCurrentCell())
            CancelNavigation(e);
         else
            if (oldCell.Item != newCell.Item)
            {
               log.DebugFormat("Processing line change event on {0}", this);

               if (!CanLeaveCurrentLine())
                  CancelNavigation(e);
            }
      }

      void CancelNavigation(PreviewChangeEventArgs e)
      {
         e.Canceled = true;
         log.DebugFormat("-- Navigation canceled.");
      }
   }
}