using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class DefaultMultiSelectionService : MultiSelectionService<MultiSelector>
   {
      private readonly List<SelectionChangedEventHandler> selectionChangedHandlers = new List<SelectionChangedEventHandler>();

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

      public override IList SelectedItems
      {
         get { return Element.SelectedItems; }
      }

      public override event SelectionChangedEventHandler SelectionChanged
      {
         add
         {
            if (Element != null)
            {
               Element.SelectionChanged += value;
               selectionChangedHandlers.Add(value);
            }
         }
         remove
         {
            if (Element != null)
            {
               Element.SelectionChanged -= value;
               selectionChangedHandlers.Remove(value);
            }
         }
      }

      public override void AddItemToSelection(object item)
      {
         if (!Element.SelectedItems.Contains(item))
            Element.SelectedItems.Add(item);
      }

      public override void ClearSelection()
      {
         Element.SelectedItems.Clear();
      }

      public override void DetachFromElement(FrameworkElement element)
      {
         UnregisterAllSelectionChangedHandlers();
         base.DetachFromElement(element);
      }

      public override bool ItemIsSelected(object item)
      {
         return Element.SelectedItems.Contains(item);
      }

      public override void RemoveItemFromSelection(object item)
      {
         if (Element.SelectedItems.Contains(item))
            Element.SelectedItems.Remove(item);
      }

      private void UnregisterAllSelectionChangedHandlers()
      {
         for (int i = selectionChangedHandlers.Count; i > 0; i--)
         {
            SelectionChanged -= selectionChangedHandlers[0];
         }

         Debug.Assert(selectionChangedHandlers.Count == 0, "Not all handlers were successfully removed.");
      }
   }
}