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
using MagicSoftware.Common.Controls.Proxies;
using System.Collections;
using System.Timers;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public enum ItemsControlEditMode
   {
      ReadOnly,
      SingleLine,
      AlwaysInEdit,
      Persistent
   }

   class DataGridEditingExtender : ElementExtenderBase<DataGrid>
   {
      static ExtenderType GetExtender<ExtenderType>(DependencyObject extendedObject)
         where ExtenderType : class, IElementExtender
      {
         IList extenders = ElementExtensions.GetExtenders(extendedObject);
         foreach (IElementExtender extender in extenders)
         {
            if (extender is ExtenderType)
            {
               return (ExtenderType)extender;
            }
         }
         return null;
      }

      public static ItemsControlEditMode GetEditMode(DependencyObject obj)
      {
         return (ItemsControlEditMode)obj.GetValue(EditModeProperty);
      }

      public static void SetEditMode(DependencyObject obj, ItemsControlEditMode value)
      {
         obj.SetValue(EditModeProperty, value);
      }

      // Using a DependencyProperty as the backing store for EditMode.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditModeProperty =
          DependencyProperty.RegisterAttached("EditMode", typeof(ItemsControlEditMode), typeof(DataGridEditingExtender), new UIPropertyMetadata(ItemsControlEditMode.ReadOnly));

      static void OnEditModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         var element = (FrameworkElement)sender;
         if (element != null)
         {
            var editingExtender = GetExtender<DataGridEditingExtender>(element);
            editingExtender.UpdateEditMode();
         }
      }


      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      IDataGridEditMode editModeWorker = null;

      protected ItemsControlEditMode EditMode
      {
         get { return GetEditMode(TargetElement); }
         set { SetEditMode(TargetElement, value); }
      }

      protected override void Setup()
      {
         UpdateEditMode();
         editModeWorker.Setup();
         //CommandManager.AddPreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
         TargetElement.PreviewKeyDown += TargetElement_PreviewKeyDown;
         TargetElement.PreviewMouseDown += TargetElement_PreviewMouseDown;
      }

      protected override void Cleanup()
      {
         editModeWorker.Cleanup();
         //CommandManager.RemovePreviewCanExecuteHandler(TargetElement, PreviewCanExecuteCommand);
         TargetElement.PreviewKeyDown -= TargetElement_PreviewKeyDown;
         TargetElement.PreviewMouseDown -= TargetElement_PreviewMouseDown;
      }

      private void UpdateEditMode()
      {
         if (editModeWorker != null)
            editModeWorker.Cleanup();

         editModeWorker = CreateEditModeWorker(EditMode);

         if (editModeWorker != null)
         {
            editModeWorker.TargetElementProxy = (EnhancedDGProxy)TargetElementProxy;
            editModeWorker.Setup();
         }
      }

      private IDataGridEditMode CreateEditModeWorker(ItemsControlEditMode editMode)
      {
         switch (editMode)
         {
            case ItemsControlEditMode.ReadOnly:
               return new ReadOnlyEditMode();

            case ItemsControlEditMode.SingleLine:
               return new SingleLineEditMode();

            case ItemsControlEditMode.AlwaysInEdit:
               return new AlwaysEditMode();

            case ItemsControlEditMode.Persistent:
               break;
            default:
               break;
         }
         return new ReadOnlyEditMode();
      }

      protected void TargetElement_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         editModeWorker.ProcessKey(e);
      }

      void TargetElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
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
      public EnhancedDGProxy TargetElementProxy { get; set; }
      protected ICurrentItemProvider CurrentItemProvider { get; private set; }

      public virtual void Setup()
      {
         CurrentItemProvider = TargetElementProxy.GetAdapter<ICurrentItemProvider>();
         TargetElementProxy.PreviewCanExecute += PreviewCanExecuteCommand;
         CurrentItemProvider.PreviewCurrentChanging += DataGridEditingExtender_PreviewCurrentChanging;
      }

      public virtual void Cleanup()
      {
         TargetElementProxy.PreviewCanExecute -= PreviewCanExecuteCommand;
         CurrentItemProvider.PreviewCurrentChanging -= DataGridEditingExtender_PreviewCurrentChanging;
      }

      void DataGridEditingExtender_PreviewCurrentChanging(object sender, CancelableRoutedEventArgs e)
      {
         e.Canceled = !CanLeaveCurrentLine();
      }


      protected abstract void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args);

      internal abstract void ProcessKey(KeyEventArgs e);

      protected abstract bool CanLeaveCurrentLine();
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

      internal override void ProcessKey(KeyEventArgs e)
      {
         if (!DataGridNavigationExtender.IsVerticalNavigationKey(e.Key))
            e.Handled = true;
      }

      protected override bool CanLeaveCurrentLine()
      {
         return false;
      }
   }

   class SingleLineEditMode : IDataGridEditMode
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public ICurrentItemProvider CurrentItemTracker { get; set; }

      public SingleLineEditMode()
      {
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }

      internal override void ProcessKey(KeyEventArgs e)
      {
         using (var editProxy = TargetElementProxy.GetAdapter<IElementEditStateProxy>())
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
         }

      }

      protected override bool CanLeaveCurrentLine()
      {
         using (var editProxy = TargetElementProxy.GetAdapter<IElementEditStateProxy>())
         {
            if (editProxy.IsEditing)
            {
               return editProxy.CommitEdit();
            }
            return true;
         }
      }
   }

   class AlwaysEditMode : IDataGridEditMode
   {
      DispatcherTimer beginEditTimer;
      IElementEditStateProxy editProxy;

      public override void Setup()
      {
         base.Setup();
         editProxy = TargetElementProxy.GetAdapter<IElementEditStateProxy>();

         CurrentItemProvider.CurrentChanged += TargetElementProxy_CurrentChanged;

         beginEditTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, this.TargetElementProxy.GetDispatcher());
         beginEditTimer.Interval = TimeSpan.FromMilliseconds(10);
         beginEditTimer.Tick += beginEditTimer_Tick;
         beginEditTimer.Start();
      }

      public override void Cleanup()
      {
         editProxy.Dispose();
         beginEditTimer.Stop();
         beginEditTimer.Tick -= beginEditTimer_Tick;
         CurrentItemProvider.CurrentChanged -= TargetElementProxy_CurrentChanged;
         base.Cleanup();
      }

      void TargetElementProxy_CurrentChanged(object sender, RoutedEventArgs e)
      {
         StartBeginEditTimer();
      }

      void StartBeginEditTimer()
      {
         beginEditTimer.Start();
      }

      void beginEditTimer_Tick(object sender, EventArgs e)
      {
         if (!editProxy.IsEditing)
         {
            if (editProxy.BeginEdit())
               beginEditTimer.Stop();
         }
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
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
   }

}
