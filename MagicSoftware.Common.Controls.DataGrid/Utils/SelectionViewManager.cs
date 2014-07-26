using System;
using System.Windows.Controls.Primitives;
using log4net;
using MagicSoftware.Common.Controls.Table.Models;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Utils
{
   internal class SelectionViewManager : IDisposable
   {
      private readonly AutoResetFlag isReadingFromView = new AutoResetFlag();
      private readonly AutoResetFlag isUpdatingView = new AutoResetFlag();
      private MultiSelector attachedElement;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private SelectionView selectionView;

      public SelectionViewManager(MultiSelector attachedElement, SelectionView selectionView)
      {
         this.attachedElement = attachedElement;
         this.selectionView = selectionView;
         //selectionView.SelectionChanged += selectionView_SelectionChanged;
      }

      public int CountSelectedItems { get { return selectionView.Count; } }

      public void Dispose()
      {
         //attachedElement.SelectionChanged -= attachedElement_SelectionChanged;
         //selectionView.SelectionChanged -= selectionView_SelectionChanged;
      }

      public void RegisterSelectionChangeEvents()
      {
         //attachedElement.SelectionChanged += attachedElement_SelectionChanged;
      }

      public void SelectItemsOnElement()
      {
         using (isReadingFromView.Set())
         {
            if (selectionView == null)
               return;

            attachedElement.SelectedItems.Clear();
            if (selectionView.Count == 0)
               return;

            var selectionEnumerator = selectionView.GetEnumerator();
            selectionEnumerator.MoveNext();
            attachedElement.SelectedItem = selectionEnumerator.Current;
            while (selectionEnumerator.MoveNext())
            {
               attachedElement.SelectedItems.Add(selectionEnumerator.Current);
            }
         }
      }

      public void UpdateViewFromElement()
      {
         using (isUpdatingView.Set())
         {
            selectionView.SetSelection(attachedElement.SelectedItems);
         }
      }

      private void attachedElement_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
         if (!isReadingFromView.IsSet)
         {
            log.Debug("Selection changed on attached element: Updating view.");
            UpdateViewFromElement();
         }
      }

      void selectionView_SelectionChanged(object sender, EventArgs e)
      {
         if (!isUpdatingView.IsSet)
         {
            log.Debug("Selection changed on view: Updating element.");
            SelectItemsOnElement();
         }
      }
   }
}