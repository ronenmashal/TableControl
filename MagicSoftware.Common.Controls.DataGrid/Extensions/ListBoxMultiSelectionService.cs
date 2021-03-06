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


      public override bool ItemIsSelected(object item)
      {
         return Element.SelectedItems.Contains(item);
      }

      public override void AddItemToSelection(object item)
      {
         if (!Element.SelectedItems.Contains(item))
            Element.SelectedItems.Add(item);
      }

      public override void RemoveItemFromSelection(object item)
      {
         if (Element.SelectedItems.Contains(item))
            Element.SelectedItems.Remove(item);
      }
   }
}