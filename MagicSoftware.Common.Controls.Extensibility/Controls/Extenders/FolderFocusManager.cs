using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MagicSoftware.Common.Controls.Extenders
{
   public class FolderFocusManager : FocusManagerBase
   {
      ItemsControl container;

      #region IFocusManager Members

      protected override void Initialize()
      {
         container = ItemsControl.ItemsControlFromItemContainer(AttachedElement);
      }

      public override void DetachFromElement()
      {

      }

      protected override void PerformFocusUpdate()
      {
         Selector selector = container as Selector;
         if (selector != null)
            selector.SelectedIndex = selector.Items.CurrentPosition;
      }

      #endregion
   }
}
