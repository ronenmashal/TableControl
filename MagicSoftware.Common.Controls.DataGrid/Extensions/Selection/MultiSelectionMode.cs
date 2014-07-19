using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

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

}
