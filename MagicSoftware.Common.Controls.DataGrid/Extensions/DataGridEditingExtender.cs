using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;
using MagicSoftware.Common.Controls.Table.Extensions.Editing;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public enum ItemsControlEditMode
   {
      ReadOnly,
      SingleLine,
      AlwaysInEdit,
      Persistent
   }

   public class DataGridEditingExtender : ElementExtenderBase<DataGrid>
   {
      #region Edit Mode property

      public static readonly DependencyProperty EditModeProperty =
         DependencyProperty.RegisterAttached("EditMode", typeof(ItemsControlEditMode), typeof(DataGridEditingExtender), new UIPropertyMetadata(ItemsControlEditMode.ReadOnly, OnEditModeChanged));

      public static ItemsControlEditMode GetEditMode(DependencyObject obj)
      {
         return (ItemsControlEditMode)obj.GetValue(EditModeProperty);
      }

      public static void SetEditMode(DependencyObject obj, ItemsControlEditMode value)
      {
         obj.SetValue(EditModeProperty, value);
      }

      private static void OnEditModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         var editingExtender = GetEditingExtender(sender);
         if (editingExtender != null)
            editingExtender.UpdateEditMode();
      }

      #endregion Edit Mode property

      #region Editing Extender property

      /// <summary>
      /// This property is used internally to relate an editing extender to an element.
      /// </summary>
      private static readonly DependencyProperty EditingExtenderProperty =
          DependencyProperty.RegisterAttached("EditingExtender", typeof(DataGridEditingExtender), typeof(DataGridEditingExtender), new UIPropertyMetadata(null));

      private static DataGridEditingExtender GetEditingExtender(DependencyObject obj)
      {
         return (DataGridEditingExtender)obj.GetValue(EditingExtenderProperty);
      }

      private static void SetEditingExtender(DependencyObject obj, DataGridEditingExtender value)
      {
         obj.SetValue(EditingExtenderProperty, value);
      }

      #endregion Editing Extender property

      private DataGridEditStateMachine editModeWorker = null;

      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected ItemsControlEditMode EditMode
      {
         get { return GetEditMode(TargetElement); }
         set { SetEditMode(TargetElement, value); }
      }

      protected override void Cleanup()
      {
         editModeWorker.Cleanup();
         //CommandManager.RemovePreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
         TargetElement.RemoveHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown));
         //TargetElement.PreviewKeyDown -= TargetElement_PreviewKeyDown;
         TargetElement.PreviewMouseDown -= TargetElement_PreviewMouseDown;
         SetEditingExtender(TargetElement, null);
      }

      protected override void Setup()
      {
         SetEditingExtender(TargetElement, this);
         UpdateEditMode();
         TargetElement.AddHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown), true);
         //TargetElement.PreviewKeyDown += TargetElement_PreviewKeyDown;
         TargetElement.PreviewMouseDown += TargetElement_PreviewMouseDown;
      }

      protected void TargetElement_PreviewKeyDown(object sender, RoutedEventArgs e)
      {
         editModeWorker.ProcessKey((KeyEventArgs)e);
      }

      private DataGridEditStateMachine CreateEditStateMachine(ItemsControlEditMode editMode)
      {
         switch (editMode)
         {
            case ItemsControlEditMode.ReadOnly:
               return new ReadOnlyEditStateMachine();

            case ItemsControlEditMode.SingleLine:
               return new SingleLineEditStateMachine();

            case ItemsControlEditMode.AlwaysInEdit:
               return new AlwaysEditStateMachine();

            case ItemsControlEditMode.Persistent:
               break;

            default:
               break;
         }
         return new ReadOnlyEditStateMachine();
      }

      private void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         if ((args.Command == DataGrid.BeginEditCommand) || (args.Command == DataGrid.CommitEditCommand) || (args.Command == DataGrid.CancelEditCommand))
         {
            log.DebugFormat("Inhibiting command {0}", ((RoutedCommand)args.Command).Name);
            args.CanExecute = false;
            args.ContinueRouting = false;
            args.Handled = true;
         }
      }

      private void TargetElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
      }

      private void UpdateEditMode()
      {
         if (editModeWorker != null)
            editModeWorker.Cleanup();

         editModeWorker = CreateEditStateMachine(EditMode);

         if (editModeWorker != null)
         {
            editModeWorker.TargetElement = TargetElement;
            editModeWorker.Setup();
         }
      }
   }
}