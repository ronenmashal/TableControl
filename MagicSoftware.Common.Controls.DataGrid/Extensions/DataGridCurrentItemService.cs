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
   [DebuggerDisplay("DGCurrentItemService: #{id}")]
   class DataGridCurrentItemService : CurrentItemServiceBase, ICurrentItemService, IUIService
   {
      DependencyPropertyChangeListener currentItemPropertyChangeListener;
      DataGrid dataGrid;
      ICollectionView itemsView;
      AutoResetFlag isSelfInducedChange = new AutoResetFlag();
      bool operationWasCanceled = false;

      int id;
      static int nextId = 1;

      public DataGridCurrentItemService()
      {
         id = nextId;
         nextId++;
      }

      public override void AttachToElement(FrameworkElement element)
      {
         base.AttachToElement(element);
         this.dataGrid = element as DataGrid;

         if (element.IsLoaded)
            AttachItemsViewToItemsSource();
         else
            element.Loaded += DataGridElement_Loaded;
      }

      void DataGridElement_Loaded(object sender, RoutedEventArgs e)
      {
         dataGrid.Loaded -= DataGridElement_Loaded;
         AttachItemsViewToItemsSource();
      }

      public override void DetachFromElement(FrameworkElement element)
      {
         if (dataGrid != null)
            dataGrid.Loaded -= DataGridElement_Loaded;
         UnregisterItemsViewEventHandlers();
         if (currentItemPropertyChangeListener != null)
            currentItemPropertyChangeListener.Dispose();
      }



      /// <summary>
      /// Create the most optimized collection view for the grid's items source.
      /// </summary>
      void AttachItemsViewToItemsSource()
      {
         currentItemPropertyChangeListener = new DependencyPropertyChangeListener(dataGrid, DataGrid.CurrentItemProperty, DataGrid_CurrentItemChanged);

         if (itemsView != null)
            UnregisterItemsViewEventHandlers();

         if (dataGrid.ItemsSource is IList)
            itemsView = new ListCollectionView(dataGrid.ItemsSource as IList);
         else
            itemsView = new CollectionView(dataGrid.ItemsSource);

         itemsView.MoveCurrentTo(dataGrid.CurrentItem);

         RegisterItemsViewEventHandlers();
      }

      void RegisterItemsViewEventHandlers()
      {
         itemsView.CurrentChanging += ItemsView_CurrentChanging;
         itemsView.CurrentChanged += ItemsView_CurrentChanged;
      }

      void UnregisterItemsViewEventHandlers()
      {
         if (itemsView != null)
         {
            itemsView.CurrentChanging -= ItemsView_CurrentChanging;
            itemsView.CurrentChanged -= ItemsView_CurrentChanged;
         }
      }

      void ItemsView_CurrentChanging(object sender, CurrentChangingEventArgs e)
      {
         operationWasCanceled = false;
         if (!isSelfInducedChange.IsSet)
            RaiseNonCancelablePreviewCurrentChangingEvent(null);
         else
         {
            RaisePreviewCurrentChangingEvent(null, out operationWasCanceled);
            e.Cancel = operationWasCanceled;
         }
      }

      void ItemsView_CurrentChanged(object sender, EventArgs e)
      {
         using (isSelfInducedChange.Set())
            dataGrid.CurrentItem = itemsView.CurrentItem;
         if (dataGrid.CurrentItem != null)
            dataGrid.ScrollIntoView(dataGrid.CurrentItem);
         RaiseCurrentChangedEvent();
      }


      private void DataGrid_CurrentItemChanged(object sender, EventArgs args)
      {
         if (!isSelfInducedChange.IsSet)
         {
            itemsView.MoveCurrentTo(dataGrid.CurrentItem);
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

      bool EnsureMoveExecution(Func<bool> moveAction)
      {
         int oldPosition = CurrentPosition;
         if (moveAction())
            return !operationWasCanceled;

         return false;
      }

      public override bool MoveCurrentTo(object item)
      {
         using (isSelfInducedChange.Set())
            return EnsureMoveExecution(() => itemsView.MoveCurrentTo(item));
      }

      public override bool MoveCurrentToFirst()
      {
         using (isSelfInducedChange.Set())
            return EnsureMoveExecution(() => itemsView.MoveCurrentToFirst());
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
            return EnsureMoveExecution(() => itemsView.MoveCurrentToLast());
      }

      public override bool MoveCurrentToPosition(int position)
      {
         if (position > dataGrid.Items.Count)
            position = dataGrid.Items.Count;

         if (position < -1)
            position = -1;

         using (isSelfInducedChange.Set())
            return EnsureMoveExecution(() => itemsView.MoveCurrentToPosition(position));
      }

      public override bool MoveCurrentToRelativePosition(int offset)
      {
         int newPosition = CurrentPosition + offset;
         return MoveCurrentToPosition(newPosition);
      }

      #endregion
   }

}
