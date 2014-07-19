using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using log4net;
using MagicSoftware.Common.Controls.Table.Models;
using MagicSoftware.Common.Controls.Table.Utils;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface ISelectionService
   {
      void SetSelectionView(ISelectionView selectionView);
   }

   [ImplementedService(typeof(ISelectionService))]
   internal class DataGridSelectionExtender : ISelectionService, IUIService
   {


      public static ISelectionView GetSelectionView(DependencyObject obj)
      {
         return (ISelectionView)obj.GetValue(SelectionViewProperty);
      }

      public static void SetSelectionView(DependencyObject obj, ISelectionView value)
      {
         obj.SetValue(SelectionViewProperty, value);
      }

      public static readonly DependencyProperty SelectionViewProperty =
          DependencyProperty.RegisterAttached("SelectionView", typeof(ISelectionView), typeof(DataGridSelectionExtender), new UIPropertyMetadata(null, OnSelectionViewChanged));

      static void OnSelectionViewChanged(object obj, DependencyPropertyChangedEventArgs args)
      {
         var target = (FrameworkElement)obj;
         var selectionSvc = UIServiceProvider.GetService<DataGridSelectionExtender>(target);
         // Selection service may be null because it was either (1) not loaded yet or (2) not assigned.
         if (selectionSvc != null)
            selectionSvc.SetSelectionView((ISelectionView)args.NewValue);
      }

      public static readonly string LoggerName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".SelectionService";
      private int id;
      private ILog log = log4net.LogManager.GetLogger(LoggerName);

      private SelectionModeManager selectionModeManager;

      public DataGridSelectionExtender()
      {
         id = IdGenerator.GetNewId(this);
      }

      private event Action Attached;

      public bool IsAttached
      {
         get { return TargetElement != null; }
      }

      private MultiSelector TargetElement { get; set; }

      public void AttachToElement(FrameworkElement element)
      {
         if (!(element is MultiSelector))
            throw new InvalidOperationException("The selection service requires a multi-selector.");

         TargetElement = (MultiSelector)element;
         UIServiceProvider.AddServiceProviderFullyAttachedHandler(element, Element_ServiceProviderIsFullyAttached);
         if (Attached != null)
            Attached();
      }

      public void DetachFromElement(FrameworkElement element)
      {
         if (selectionModeManager != null)
            selectionModeManager.Dispose();
         TargetElement = null;
         selectionModeManager = null;
      }

      public void Dispose()
      {
         DetachFromElement(TargetElement);
      }

      public void SetSelectionView(ISelectionView selectionView)
      {
         if (selectionModeManager != null)
            selectionModeManager.SetSelectionView(selectionView);
      }

      private void Element_ServiceProviderIsFullyAttached(object obj, RoutedEventArgs args)
      {
         UIServiceProvider.RemoveServiceProviderFullyAttachedHandler(TargetElement, Element_ServiceProviderIsFullyAttached);
         var currentItemProvider = UIServiceProvider.GetService<ICurrentItemService>(TargetElement);
         if (selectionModeManager != null)
            selectionModeManager.Dispose();
         selectionModeManager = new SelectionModeManager(TargetElement, currentItemProvider);
         var selectionView = GetSelectionView(TargetElement);
         if (selectionView != null)
            selectionModeManager.SetSelectionView(selectionView);
      }
   }

   internal class IdleSelectionMode : SelectionMode
   {
      public override void Enter()
      {
      }

      public override void Leave()
      {
      }

      public override void OnCurrentItemChanged()
      {
      }

      protected override SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs)
      {
         return this;
      }

      protected override SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs)
      {
         return this;
      }
   }

   internal class MultiSelectionMode : SelectionMode
   {
      private static readonly KeyGesture ToggleSelectionKey = new KeyGesture(Key.Space, ModifierKeys.Control);
      private bool ctrlIsDown = false;
      private int lastDistanceFromSelectionAnchor = 0;
      private object lastKnownCurrentItem;
      private bool shiftIsDown = false;

      public override void Enter()
      {
         var anchor = CurrentItemTracker.CurrentItem;
         if (Element.SelectedItem != null)
            anchor = Element.SelectedItem;
         log.Debug("Entering multi-selection mode.");
         log.DebugFormat("Anchor is {0}", anchor);
         lastKnownCurrentItem = anchor;
         lastDistanceFromSelectionAnchor = 0;
      }

      public override void Leave()
      {
      }

      public override void OnCurrentItemChanged()
      {
         UpdateKeyboardStatus();
         if (!ctrlIsDown || shiftIsDown)
         {
            int newDistanceFromSelectionAnchor = CurrentItemTracker.CurrentPosition - Element.SelectedIndex;
            log.DebugFormat("New distance to anchor: {0}", newDistanceFromSelectionAnchor);

            if (Math.Sign(newDistanceFromSelectionAnchor) != Math.Sign(lastDistanceFromSelectionAnchor))
            {
               RemovedRangeFromSelection(Element.SelectedIndex + lastDistanceFromSelectionAnchor, Element.SelectedIndex);
               AddRangeToSelection(Element.SelectedIndex + newDistanceFromSelectionAnchor, Element.SelectedIndex);
            }
            else if (Math.Abs(newDistanceFromSelectionAnchor) < Math.Abs(lastDistanceFromSelectionAnchor))
            {
               log.DebugFormat("Removing item {0} from selection", lastKnownCurrentItem);
               RemovedRangeFromSelection(Element.SelectedIndex + lastDistanceFromSelectionAnchor, Element.SelectedIndex + newDistanceFromSelectionAnchor);
            }
            else
            {
               log.DebugFormat("Adding item {0} to selection", CurrentItemTracker.CurrentItem);
               AddRangeToSelection(Element.SelectedIndex + newDistanceFromSelectionAnchor, Element.SelectedIndex + lastDistanceFromSelectionAnchor);
            }
            lastKnownCurrentItem = CurrentItemTracker.CurrentItem;
            lastDistanceFromSelectionAnchor = newDistanceFromSelectionAnchor;
         }
      }

      protected override SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs)
      {
         if (keyEventArgs.IsDown)
         {
            if (SelectionModeManager.IsMultiSelectionKey(keyEventArgs.Key))
               return this;

            ModifierKeys modifiers = keyEventArgs.KeyboardDevice.Modifiers;
            if (!(modifiers.HasFlag(ModifierKeys.Shift) || modifiers.HasFlag(ModifierKeys.Control)))
            {
               return SelectionModeManager.SingleSelectionMode;
            }

            if (ToggleSelectionKey.Matches(sender, keyEventArgs))
            {
               log.Debug("Toggling current item's selection");
               ToggleItemSelection();
            }
         }

         return this;
      }

      protected override SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs)
      {
         UpdateKeyboardStatus();
         if (!ctrlIsDown && !shiftIsDown)
            return SelectionModeManager.SingleSelectionMode;

         return this;
      }

      private void AddRangeToSelection(int fromItemIndex, int toItemIndex)
      {
         int direction = Math.Sign(toItemIndex - fromItemIndex);
         for (int i = fromItemIndex; i != toItemIndex; i += direction)
         {
            var item = Element.Items.GetItemAt(i);
            if (Element.SelectedItems.Contains(item))
               break;
            Element.SelectedItems.Add(item);
         }
      }

      private void RemovedRangeFromSelection(int fromItemIndex, int toItemIndex)
      {
         int direction = Math.Sign(toItemIndex - fromItemIndex);
         for (int i = fromItemIndex; i != toItemIndex; i += direction)
         {
            var item = Element.Items.GetItemAt(fromItemIndex);
            if (!Element.SelectedItems.Contains(item))
               continue;
            Element.SelectedItems.Remove(item);
         }
      }

      private void ToggleItemSelection()
      {
         if (Element.SelectedItems.Contains(CurrentItemTracker.CurrentItem))
         {
            Element.SelectedItems.Remove(CurrentItemTracker.CurrentItem);
         }
         else
         {
            Element.SelectedItems.Add(CurrentItemTracker.CurrentItem);
         }
      }

      private void UpdateKeyboardStatus()
      {
         ModifierKeys modifiers = InputManager.Current.PrimaryKeyboardDevice.Modifiers;
         ctrlIsDown = (modifiers.HasFlag(ModifierKeys.Control));
         shiftIsDown = (modifiers.HasFlag(ModifierKeys.Shift));
      }
   }

   internal abstract class SelectionMode
   {
      protected ILog log = log4net.LogManager.GetLogger(DataGridSelectionExtender.LoggerName);

      protected ICurrentItemService CurrentItemTracker { get; private set; }

      protected MultiSelector Element { get; private set; }

      public abstract void Enter();

      public void Initialize(MultiSelector element, ICurrentItemService currentItemTracker)
      {
         Element = element;
         CurrentItemTracker = currentItemTracker;
      }

      public abstract void Leave();

      public abstract void OnCurrentItemChanged();

      internal SelectionMode HandleInputEvent(object sender, InputEventArgs inputEventArgs)
      {
         if (inputEventArgs is KeyEventArgs)
         {
            return HandleKeyboardEvent(sender, inputEventArgs as KeyEventArgs);
         }
         else if (inputEventArgs is MouseButtonEventArgs)
         {
            return HandleMouseEvent(sender, inputEventArgs as MouseButtonEventArgs);
         }
         log.Debug("Don't know how to handle input event " + inputEventArgs);
         return this;
      }

      protected abstract SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs);

      protected abstract SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs);
   }

   internal class SelectionModeManager : IDisposable
   {
      public static readonly IdleSelectionMode IdleSelectionMode = new IdleSelectionMode();
      public static readonly MultiSelectionMode MultiSelectionMode = new MultiSelectionMode();
      public static readonly SingleSelectionMode SingleSelectionMode = new SingleSelectionMode();
      protected ILog log = log4net.LogManager.GetLogger(DataGridSelectionExtender.LoggerName);

      private SelectionMode currentSelectionMode;

      private SelectionViewManager selectionViewManager = null;

      public SelectionModeManager(MultiSelector element, ICurrentItemService currentItemTracker)
      {
         Element = element;
         CurrentItemTracker = currentItemTracker;
         CurrentItemTracker.CurrentChanged += CurrentItemTracker_CurrentChanged;

         Element.AddHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown), true);
         Element.AddHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp), true);
         Element.AddHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown), true);

         SingleSelectionMode.Initialize(element, currentItemTracker);
         MultiSelectionMode.Initialize(element, currentItemTracker);

         SetCurrentSelectionMode(IdleSelectionMode);
         currentSelectionMode.Enter();
      }

      protected ICurrentItemService CurrentItemTracker { get; private set; }

      protected MultiSelector Element { get; private set; }

      public static bool IsMultiSelectionKey(Key key)
      {
         switch (key)
         {
            case Key.LeftCtrl:
            case Key.RightCtrl:
            case Key.LeftShift:
            case Key.RightShift:
               return true;
         }
         return false;
      }

      public virtual void Dispose()
      {
         Element.RemoveHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown));
         Element.RemoveHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp));
         Element.RemoveHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown));
         CurrentItemTracker.CurrentChanged -= CurrentItemTracker_CurrentChanged;
         CurrentItemTracker = null;
      }

      public void SetSelectionView(ISelectionView selectionView)
      {
         if (selectionViewManager != null)
            selectionViewManager.Dispose();

         if (selectionView == null)
            selectionViewManager = null;

         //SetCurrentSelectionMode(IdleSelectionMode);

         selectionViewManager = new SelectionViewManager(Element, selectionView);
         Element.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => 
         {
            selectionViewManager.UpdateSelectionFromSelectionView();
         }));
      }

      protected void TargetElement_PreviewKeyUp(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      protected void TargetElement_PreviewMouseDown(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      private void CurrentItemTracker_CurrentChanged(object sender, EventArgs args)
      {
         currentSelectionMode.OnCurrentItemChanged();
      }

      private void SetCurrentSelectionMode(SelectionMode nextSelectionMode)
      {
         if (nextSelectionMode != currentSelectionMode)
         {
            if (currentSelectionMode != null)
               currentSelectionMode.Leave();
            currentSelectionMode = nextSelectionMode;
            currentSelectionMode.Enter();
         }
      }

      private void TargetElement_PreviewKeyDown(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }
   }

   internal class SingleSelectionMode : SelectionMode
   {
      public override void Enter()
      {
         log.Debug("Entering single selection mode");
         Element.SelectedItem = CurrentItemTracker.CurrentItem;
      }

      public override void Leave()
      {
      }

      public override void OnCurrentItemChanged()
      {
         Element.SelectedItem = CurrentItemTracker.CurrentItem;
      }

      protected override SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs)
      {
         if (keyEventArgs.IsDown)
         {
            if (SelectionModeManager.IsMultiSelectionKey(keyEventArgs.Key))
            {
               log.Debug("Pressed key is multi selection key.");
               return SelectionModeManager.MultiSelectionMode;
            }
         }

         return this;
      }

      protected override SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs)
      {
         return this;
      }
   }
}