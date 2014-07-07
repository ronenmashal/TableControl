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
      private ISelectionView selectionView;

      public SelectionViewManager(MultiSelector attachedElement, ISelectionView selectionView)
      {
         this.attachedElement = attachedElement;
         this.selectionView = selectionView;
         attachedElement.SelectionChanged += attachedElement_SelectionChanged;
         selectionView.PropertyChanged += selectionView_PropertyChanged;
         if (!attachedElement.IsLoaded)
            attachedElement.Loaded += attachedElement_Loaded;
      }

      public void Dispose()
      {
         attachedElement.SelectionChanged -= attachedElement_SelectionChanged;
         selectionView.PropertyChanged -= selectionView_PropertyChanged;
      }

      private void attachedElement_Loaded(object sender, System.Windows.RoutedEventArgs e)
      {
         attachedElement.Loaded -= attachedElement_Loaded;
         UpdateSelectionFromSelectionView();
      }

      private void attachedElement_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
         if (!isReadingFromView.IsSet)
         {
            using (isUpdatingView.Set())
            {
               log.Debug("Selection changed. " + attachedElement.SelectedItems.Count);
               object[] selection = new object[attachedElement.SelectedItems.Count];
               int i = 0;
               foreach (var item in attachedElement.SelectedItems)
               {
                  selection[i++] = item;
               }
               selectionView.Selection = selection;
            }
         }
      }

      private void selectionView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         if (!isUpdatingView.IsSet)
         {
            UpdateSelectionFromSelectionView();
         }
      }

      private void UpdateSelectionFromSelectionView()
      {
         using (isReadingFromView.Set())
         {
            if (selectionView == null)
               return;

            attachedElement.SelectedItems.Clear();
            if (selectionView.Selection == null)
               return;

            foreach (var item in selectionView.Selection)
            {
               attachedElement.SelectedItems.Add(item);
            }
         }
      }
   }
}