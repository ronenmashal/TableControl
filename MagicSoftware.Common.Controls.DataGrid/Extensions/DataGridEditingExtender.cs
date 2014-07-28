using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;

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
         DependencyProperty.RegisterAttached("EditMode", typeof(ItemsControlEditMode), typeof(DataGridEditingExtender), new UIPropertyMetadata(ItemsControlEditMode.ReadOnly));

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

   internal class SingleLineEditStateMachine : DataGridEditStateMachine
   {
      public SingleLineEditStateMachine()
      {
      }

      public ICurrentItemService CurrentItemTracker { get; set; }

      internal override void ProcessKey(KeyEventArgs e)
      {
         log.DebugFormat("Processing key {0} on {1}", e.Key, this);
         var editProxy = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);
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
      }

      protected override bool CanLeaveCurrentLine()
      {
         var editProxy = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);
         if (editProxy.IsEditing)
         {
            return editProxy.CommitEdit();
         }
         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }
   }
}