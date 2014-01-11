using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using MagicSoftware.Common.Controls.Proxies;
using System.Diagnostics;
using MagicSoftware.Common.Utils;
using System.Collections;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedServiceAttribute(typeof(ICurrentItemService))]
   class DataGridCurrentItemService : CurrentItemServiceBase, ICurrentItemService, IUIService
   {
      // TODO: SHould be ItemsControlPRoxy or FrameworkElementProxy.
      private EnhancedDGProxy dgProxy;

      DependencyPropertyChangeListener currentItemPropertyChangeListener;
      DataGrid DataGridElement;
      ICollectionView itemsView;
      AutoResetFlag isSelfInducedChange = new AutoResetFlag();

      public DataGridCurrentItemService()
      {

      }

      [Obsolete("You should use the empty constructor.")]
      public DataGridCurrentItemService(DataGrid dataGrid): base(dataGrid)
      {
         SetElement(dataGrid);
      }

      public void SetElement(DataGrid dataGrid)
      {
         base.SetElement(dataGrid);
         this.DataGridElement = dataGrid;
         this.dgProxy = (EnhancedDGProxy)FrameworkElementProxy.GetProxy(dataGrid);
         Debug.Assert(dgProxy != null, "The attached element must have a proxy");

         if (DataGridElement.IsLoaded)
            AttachItemsViewToItemsSource();
         else
         {
            RoutedEventHandler handler = null;
            handler = (sender, e) =>
            {
               DataGridElement.Loaded -= handler;
               AttachItemsViewToItemsSource();
            };
            DataGridElement.Loaded += handler;
         }
      }

      /// <summary>
      /// Create the most optimized collection view for the grid's items source.
      /// </summary>
      void AttachItemsViewToItemsSource()
      {
         currentItemPropertyChangeListener = new DependencyPropertyChangeListener(DataGridElement, DataGrid.CurrentItemProperty, DataGrid_CurrentItemChanged);

         if (itemsView != null)
            UnregisterItemsViewEventHandlers();

         if (DataGridElement.ItemsSource is IList)
            itemsView = new ListCollectionView(DataGridElement.ItemsSource as IList);
         else
            itemsView = new CollectionView(DataGridElement.ItemsSource);

         itemsView.MoveCurrentTo(DataGridElement.CurrentItem);

         RegisterItemsViewEventHandlers();
      }

      void RegisterItemsViewEventHandlers()
      {
         itemsView.CurrentChanging += ItemsView_CurrentChanging;
         itemsView.CurrentChanged += ItemsView_CurrentChanged;
      }

      void UnregisterItemsViewEventHandlers()
      {
         itemsView.CurrentChanging -= ItemsView_CurrentChanging;
         itemsView.CurrentChanged -= ItemsView_CurrentChanged;
      }

      public void Dispose()
      {
         currentItemPropertyChangeListener.Dispose();
      }

      void ItemsView_CurrentChanging(object sender, CurrentChangingEventArgs e)
      {
         if (!isSelfInducedChange.IsSet)
         {
            RaiseNonCancelablePreviewCurrentChangingEvent(null);
         }
         else
         {
            bool canceled;
            RaisePreviewCurrentChangingEvent(null, out canceled);
            e.Cancel = canceled;
         }
      }

      void ItemsView_CurrentChanged(object sender, EventArgs e)
      {
         using (isSelfInducedChange.Set())
         {
            DataGridElement.CurrentItem = itemsView.CurrentItem;
         }
         RaiseCurrentChangedEvent();
      }


      private void DataGrid_CurrentItemChanged(object sender, EventArgs args)
      {
         if (!isSelfInducedChange.IsSet)
         {
            itemsView.MoveCurrentTo(DataGridElement.CurrentItem);
         }
      }

      public override object CurrentItem
      {
         get
         {
            return itemsView.CurrentItem;
         }
      }

      public override int CurrentPosition
      {
         get
         {
            return itemsView.CurrentPosition;
         }
      }


      #region ICurrentItemProvider Members


      public override bool MoveCurrentTo(object item)
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentTo(item);
      }

      public override bool MoveCurrentToFirst()
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentToFirst();
      }

      public override bool MoveCurrentToNext()
      {
         return MoveCurrentToRelativePosition(+1);
      }

      public override bool MoveCurrentToPrevious()
      {
         if (itemsView.IsCurrentAfterLast)
            return MoveCurrentToLast();

         return MoveCurrentToRelativePosition(-1);
      }

      public override bool MoveCurrentToLast()
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentToLast();
      }

      public override bool MoveCurrentToPosition(int position)
      {
         if (position < -1)
            return false;

         if (itemsView.IsCurrentAfterLast)
            return false;

         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentToPosition(position);
      }

      public override bool MoveCurrentToRelativePosition(int offset)
      {
         int newPosition = CurrentPosition + offset;
         return MoveCurrentToPosition(newPosition);
      }

      #endregion




      #region IUIService Members

      void IUIService.SetElement(UIElement element)
      {
         SetElement((DataGrid)element);
      }

      #endregion
   }

}
