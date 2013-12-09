using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Controls;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.ProxiesX
{
   public abstract class MultiSelectorProxy : SelectorProxy
   {
      protected MultiSelector ProxiedMultiSelector { get { return (MultiSelector)Element; } }
      MultiSelection multiSelection;

      public MultiSelectorProxy(MultiSelector element) : base(element)
      {
         multiSelection = new MultiSelection();
         multiSelection.AttachTo(element);
      }

      protected override void  DetachFromElement()
      {
         multiSelection.Detach();
      }

      public IList SelectedItems
      {
         get
         {
            return multiSelection.GetSelection();
         }
      }

      public void ToggleSelection(object item)
      {
         multiSelection.ToggleSelection(item);
      }

     
      class MultiSelection
      {
         HashSet<object> selectedItemsHash = new HashSet<object>();

         MultiSelector multiSelector;
         AutoResetFlag suppressSelectionChanges = new AutoResetFlag();

         public void AttachTo(MultiSelector multiSelector)
         {
            this.multiSelector = multiSelector;
            multiSelector.SelectionChanged += multiSelector_SelectionChanged;
         }

         public void Detach()
         {
            multiSelector.SelectionChanged -= multiSelector_SelectionChanged;
         }

         void multiSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
            if (suppressSelectionChanges.IsSet)
               return;

            if (e.OriginalSource != sender)
               return;

            foreach (var item in e.RemovedItems)
            {
               selectedItemsHash.Remove(item);
            }
            foreach (var item in e.AddedItems)
            {
               selectedItemsHash.Add(item);
            }
         }

         private void ApplySelection()
         {
            using (suppressSelectionChanges.Set())
            {
               multiSelector.SelectedItems.Clear();
               foreach (var item in selectedItemsHash)
               {
                  multiSelector.SelectedItems.Add(item);
               }
            }
         }

         public IList GetSelection()
         {
            return selectedItemsHash.ToList();
         }


         internal void ToggleSelection(object item)
         {
            if (selectedItemsHash.Contains(item))
            {
               selectedItemsHash.Remove(item);
            }
            else
            {
               selectedItemsHash.Add(item);
            }
            ApplySelection();
         }
      }
   }

}
