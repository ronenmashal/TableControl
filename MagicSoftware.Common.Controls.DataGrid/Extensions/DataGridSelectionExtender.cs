using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Proxies;
using System.Windows.Input;
using log4net;
using MagicSoftware.Common.Utils;
using LogLevel = log4net.Core.Level;
using System.Windows.Media;
using MagicSoftware.Common.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using MagicSoftware.Common.ViewModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridSelectionExtender : ElementExtenderBase<DataGrid>
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected EnhancedDGProxy DataGridProxy { get { return (EnhancedDGProxy)TargetElementProxy; } }
      protected IEditingItemsControlProxy EditProxy { get; private set; }

      SelectionModeManager selectionModeManager;

      protected override void Setup()
      {
         EditProxy = DataGridProxy.GetAdapter<IEditingItemsControlProxy>();
         var currentItemProvider = DataGridProxy.GetAdapter<ICurrentItemService>();
         selectionModeManager = new SelectionModeManager(TargetElement, currentItemProvider);
      }

      protected override void Cleanup()
      {
         selectionModeManager.Dispose();
      }
   }

   class SelectionModeManager : IDisposable
   {
      public static readonly SingleSelectionMode SingleSelectionMode = new SingleSelectionMode();
      public static readonly MultiSelectionMode MultiSelectionMode = new MultiSelectionMode();

      protected ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected MultiSelector Element { get; private set; }
      protected ICurrentItemService CurrentItemTracker { get; private set; }

      private SelectionMode currentSelectionMode;

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

         currentSelectionMode = SingleSelectionMode;
         currentSelectionMode.Enter();
      }

      public virtual void Dispose()
      {
         Element.RemoveHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown));
         Element.RemoveHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp));
         Element.RemoveHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown));
      }

      void SetCurrentSelectionMode(SelectionMode nextSelectionMode)
      {
         if (nextSelectionMode != currentSelectionMode)
         {
            currentSelectionMode.Leave();
            currentSelectionMode = nextSelectionMode;
            currentSelectionMode.Enter();
         }
      }

      void TargetElement_PreviewKeyDown(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      protected void TargetElement_PreviewKeyUp(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      protected void TargetElement_PreviewMouseDown(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      void CurrentItemTracker_CurrentChanged(object sender, RoutedEventArgs args)
      {
         currentSelectionMode.OnCurrentItemChanged();
      }

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


   }

   abstract class SelectionMode
   {
      protected ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected MultiSelector Element { get; private set; }
      protected ICurrentItemService CurrentItemTracker { get; private set; }

      public void Initialize(MultiSelector element, ICurrentItemService currentItemTracker)
      {
         Element = element;
         CurrentItemTracker = currentItemTracker;
      }

      public abstract void Enter();
      public abstract void Leave();
      public abstract void OnCurrentItemChanged();


      protected abstract SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs);
      protected abstract SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs);

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
   }

   class SingleSelectionMode : SelectionMode
   {
      public override void Enter()
      {
         log.Debug("Entering single selection mode");
         Element.SelectedItem = CurrentItemTracker.CurrentItem;
      }

      public override void Leave()
      {

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

      public override void OnCurrentItemChanged()
      {
         Element.SelectedItem = CurrentItemTracker.CurrentItem;
      }
   }

   class MultiSelectionMode : SelectionMode
   {
      int lastDistanceFromSelectionAnchor = 0;
      object lastKnownCurrentItem;

      bool ctrlIsDown = false;
      bool shiftIsDown = false;

      static readonly KeyGesture ToggleSelectionKey = new KeyGesture(Key.Space, ModifierKeys.Control);

      public override void Enter()
      {
         log.Debug("Entering multi-selection mode.");
         log.DebugFormat("Current item is {0}", CurrentItemTracker.CurrentItem);
         lastKnownCurrentItem = CurrentItemTracker.CurrentItem;
         lastDistanceFromSelectionAnchor = 0;
      }

      public override void Leave()
      {

      }

      void UpdateKeyboardStatus()
      {
         ModifierKeys modifiers = InputManager.Current.PrimaryKeyboardDevice.Modifiers;
         ctrlIsDown = (modifiers.HasFlag(ModifierKeys.Control));
         shiftIsDown = (modifiers.HasFlag(ModifierKeys.Shift));
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

      protected override SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs)
      {
         UpdateKeyboardStatus();
         if (!ctrlIsDown && !shiftIsDown)
            return SelectionModeManager.SingleSelectionMode;

         return this;
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

      void AddRangeToSelection(int fromItemIndex, int toItemIndex)
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

      void RemovedRangeFromSelection(int fromItemIndex, int toItemIndex)
      {
         int direction = Math.Sign(toItemIndex - fromItemIndex);
         for (int i = fromItemIndex; i != toItemIndex; i += direction)
         {
            var item = Element.Items.GetItemAt(fromItemIndex);
            if (!Element.SelectedItems.Contains(item))
               break;
            Element.SelectedItems.Add(item);
         }
      }
   }


}
