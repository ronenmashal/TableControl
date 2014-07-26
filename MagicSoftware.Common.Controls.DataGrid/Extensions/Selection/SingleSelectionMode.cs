using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions.Selection
{
   internal class SingleSelectionMode : SelectionMode
   {
      public override void Enter()
      {
         log.Debug("Entering single selection mode");
      }

      public override void Leave()
      {
      }

      public override void OnCurrentItemChanged()
      {
         if (!Element.IsKeyboardFocusWithin)
            return;

         log.Debug("Changing the selected item.");
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
