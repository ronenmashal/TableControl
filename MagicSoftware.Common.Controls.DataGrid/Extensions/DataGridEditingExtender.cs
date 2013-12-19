using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Input;
using System.Windows;
using log4net;
using MagicSoftware.Common.Utils;
using LogLevel = log4net.Core.Level;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridEditingExtender : ElementExtenderBase<DataGrid>
   {


      public static IDataGridEditMode GetEditMode(DependencyObject obj)
      {
         return (IDataGridEditMode)obj.GetValue(EditModeProperty);
      }

      public static void SetEditMode(DependencyObject obj, IDataGridEditMode value)
      {
         obj.SetValue(EditModeProperty, value);
      }

      // Using a DependencyProperty as the backing store for EditMode.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditModeProperty =
          DependencyProperty.RegisterAttached("EditMode", typeof(IDataGridEditMode), typeof(DataGridEditingExtender), new UIPropertyMetadata(new ReadOnlyEditMode()));



      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected override void Setup()
      {
         CommandManager.AddPreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
      }

      protected override void Cleanup()
      {
         CommandManager.RemovePreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
      }

      void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
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

   abstract class IDataGridEditMode
   {
      public DataGrid TargetElement { get; set; }

      public void Setup()
      {
         CommandManager.AddPreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
      }

      public void Cleanup()
      {
         CommandManager.RemovePreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
      }

      protected abstract void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args);
   }

   class ReadOnlyEditMode : IDataGridEditMode
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

   class SingleLineEditMode : IDataGridEditMode
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         throw new NotImplementedException();
      }
   }


   abstract class EditState
   {

   }

   class NotEditingState : EditState
   {

   }

   class EditingState : EditState
   {

   }
}
