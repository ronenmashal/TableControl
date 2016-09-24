using System.Collections;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class ListBoxMultiSelectionService : MultiSelectionService<ListBox>
   {
      private int selectionChangedHandlersCount = 0;

      public override event SelectionChangedEventHandler SelectionChanged
      {
         add { Element.SelectionChanged += value; selectionChangedHandlersCount++; }
         remove { Element.SelectionChanged -= value; selectionChangedHandlersCount--; }
      }

      public override IList SelectedItems
      {
         get { return Element.SelectedItems; }
      }

      public override void ClearSelection()
      {
         Element.SelectedItems.Clear();
      }

      public override int SelectedIndex
      {
         get { return Element.SelectedIndex; }
         set { Element.SelectedIndex = value; }
      }

      public override object SelectedItem
      {
         get { return Element.SelectedItem; }
         set { Element.SelectedItem = value; }
      }

   }
}