using System;
using System.Collections;
using System.Collections.Generic;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Models
{
   public class SelectionView
   {
      private readonly AutoResetFlag suppressSelectionChangeEvents = new AutoResetFlag();
      private List<object> selection = new List<object>();

      public event EventHandler SelectionChanged;

      public int Count
      {
         get { return selection.Count; }
      }

      public bool IsSynchronized
      {
         get { return false; }
      }

      public object SyncRoot
      {
         get { return null; }
      }

      public void ClearSelection()
      {
         selection.Clear();
         OnSelectionChanged();
      }

      public void CopyTo(Array array, int index)
      {
         selection.CopyTo((object[])array, index);
      }

      public IEnumerator GetEnumerator()
      {
         return selection.GetEnumerator();
      }

      public void SelectAll(IEnumerable<object> sourceCollection)
      {
         selection.Clear();
         selection.AddRange(sourceCollection);
         OnSelectionChanged();
      }

      public void SelectItem(object item)
      {
         selection.Add(item);
         OnSelectionChanged();
      }

      public void SetSelection(ICollection selection)
      {
         using (suppressSelectionChangeEvents.Set())
         {
            ClearSelection();
            foreach (var item in selection)
            {
               SelectItem(item);
            }
         }
         OnSelectionChanged();
      }

      public void UnselectItem(object item)
      {
         selection.Remove(item);
         OnSelectionChanged();
      }

      private void OnSelectionChanged()
      {
         if (SelectionChanged != null && !suppressSelectionChangeEvents.IsSet)
            SelectionChanged(this, new EventArgs());
      }
   }
}