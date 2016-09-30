using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions.Selection
{
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
         if (ElementSelectionService.SelectedItem != null)
            anchor = ElementSelectionService.SelectedItem;
         log.Debug("Entering multi-selection mode.");
         log.DebugFormat("Anchor is {0}", anchor);
         lastKnownCurrentItem = anchor;
         lastDistanceFromSelectionAnchor = 0;

         var inputService = UIServiceProvider.GetService<InputService>(ElementSelectionService.Element);
         inputService.RegisterKeyGestureAction(ToggleSelectionKey, (a) => ToggleItemSelection());
      }

      public override void Leave()
      {
         var inputService = UIServiceProvider.GetService<InputService>(ElementSelectionService.Element);
         inputService.UnregisterGestureAction(ToggleSelectionKey);
      }

      public override void OnCurrentItemChanged()
      {
         if (!ElementSelectionService.Element.IsKeyboardFocusWithin)
            return;

         UpdateKeyboardStatus();
         if (!ctrlIsDown || shiftIsDown)
         {
            int newDistanceFromSelectionAnchor = CurrentItemTracker.CurrentPosition - ElementSelectionService.SelectedIndex;
            log.DebugFormat("New distance to anchor: {0}", newDistanceFromSelectionAnchor);

            if (Math.Sign(newDistanceFromSelectionAnchor) != Math.Sign(lastDistanceFromSelectionAnchor))
            {
               RemovedRangeFromSelection(ElementSelectionService.SelectedIndex + lastDistanceFromSelectionAnchor, ElementSelectionService.SelectedIndex);
               AddRangeToSelection(ElementSelectionService.SelectedIndex + newDistanceFromSelectionAnchor, ElementSelectionService.SelectedIndex);
            }
            else if (Math.Abs(newDistanceFromSelectionAnchor) < Math.Abs(lastDistanceFromSelectionAnchor))
            {
               log.DebugFormat("Removing item {0} from selection", lastKnownCurrentItem);
               RemovedRangeFromSelection(ElementSelectionService.SelectedIndex + lastDistanceFromSelectionAnchor, ElementSelectionService.SelectedIndex + newDistanceFromSelectionAnchor);
            }
            else
            {
               log.DebugFormat("Adding item {0} to selection", CurrentItemTracker.CurrentItem);
               AddRangeToSelection(ElementSelectionService.SelectedIndex + newDistanceFromSelectionAnchor, ElementSelectionService.SelectedIndex + lastDistanceFromSelectionAnchor);
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

            if (ToggleSelectionKey.Matches(sender, keyEventArgs))
            {
               log.Debug("Toggling current item's selection");
               ToggleItemSelection();
               keyEventArgs.Handled = true;
            }
         }
         else if (keyEventArgs.IsUp)
         {
            UpdateKeyboardStatus();
            if (!ctrlIsDown && !shiftIsDown)
               return SelectionModeManager.SingleSelectionMode;
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
            var item = ((ItemsControl)ElementSelectionService.Element).Items.GetItemAt(i);
            if (ElementSelectionService.SelectedItems.Contains(item))
               break;
            ElementSelectionService.SelectedItems.Add(item);
         }
      }

      private void RemovedRangeFromSelection(int fromItemIndex, int toItemIndex)
      {
         int direction = Math.Sign(toItemIndex - fromItemIndex);
         for (int i = fromItemIndex; i != toItemIndex; i += direction)
         {
            var item = ((ItemsControl)ElementSelectionService.Element).Items.GetItemAt(fromItemIndex);
            if (!ElementSelectionService.SelectedItems.Contains(item))
               continue;
            ElementSelectionService.SelectedItems.Remove(item);
         }
      }

      private void ToggleItemSelection()
      {
         if (ElementSelectionService.SelectedItems.Contains(CurrentItemTracker.CurrentItem))
         {
            ElementSelectionService.SelectedItems.Remove(CurrentItemTracker.CurrentItem);
         }
         else
         {
            ElementSelectionService.SelectedItems.Add(CurrentItemTracker.CurrentItem);
         }
      }

      private void UpdateKeyboardStatus()
      {
         ModifierKeys modifiers = InputManager.Current.PrimaryKeyboardDevice.Modifiers;
         ctrlIsDown = (modifiers.HasFlag(ModifierKeys.Control));
         shiftIsDown = (modifiers.HasFlag(ModifierKeys.Shift));
      }
   }

}
