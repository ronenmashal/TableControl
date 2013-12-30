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
   class DataGridAsCurrentItemProvider : ObservableObject, ICurrentItemProvider
   {
      // TODO: SHould be ItemsControlPRoxy or FrameworkElementProxy.
      private EnhancedDGProxy dgProxy;

      //TODO: Move up to ItemsControlProxy.
      #region CurrentChanging attached event.

      public static readonly RoutedEvent PreviewCurrentChangingEvent = EventManager.RegisterRoutedEvent("PreviewCurrentChanging", RoutingStrategy.Tunnel, typeof(CancelableRoutedEventArgs), typeof(DataGridAsCurrentItemProvider));

      Dictionary<CancelableRoutedEventHandler, RoutedEventHandler> currentChangingEventHandlers = new Dictionary<CancelableRoutedEventHandler, RoutedEventHandler>();

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event CancelableRoutedEventHandler PreviewCurrentChanging
      {
         add
         {
            var handler = new RoutedEventHandler((obj, args) => { value(obj, (CancelableRoutedEventArgs)args); });
            dgProxy.AddRoutedEventHandler(PreviewCurrentChangingEvent, handler);
            currentChangingEventHandlers.Add(value, handler);
         }
         remove
         {
            RoutedEventHandler handler;
            if (currentChangingEventHandlers.TryGetValue(value, out handler))
               dgProxy.RemoveRoutedEventHandler(PreviewCurrentChangingEvent, handler);
         }
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent, allowing the handlers to cancel it,
      /// returning the result in 'canceled'
      /// </summary>
      /// 
      /// <param name="canceled">Returns whether any of the event handlers canceled the event.</param>
      private void RaisePreviewCurrentChangingEvent(out bool canceled)
      {
         CancelableRoutedEventArgs eventArgs = new CancelableRoutedEventArgs(PreviewCurrentChangingEvent, dgProxy.ElementAsEventSource);
         dgProxy.RaiseEvent(eventArgs);
         canceled = eventArgs.Canceled;
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent without allowing canceling of the event.
      /// </summary>
      private void RaiseNonCancelablePreviewCurrentChangingEvent()
      {
         CancelableRoutedEventArgs eventArgs = new CancelableRoutedEventArgs(PreviewCurrentChangingEvent, dgProxy.ElementAsEventSource, false);
         dgProxy.RaiseEvent(eventArgs);
      }

      #endregion

      //TODO: Move up to ItemsControlProxy.
      #region CurrentChanging attached event.

      public static readonly RoutedEvent CurrentChangedEvent = EventManager.RegisterRoutedEvent("CurrentChanged", RoutingStrategy.Tunnel, typeof(RoutedEventArgs), typeof(DataGridAsCurrentItemProvider));

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event RoutedEventHandler CurrentChanged
      {
         add { dgProxy.AddRoutedEventHandler(CurrentChangedEvent, value); }
         remove { dgProxy.RemoveRoutedEventHandler(CurrentChangedEvent, value); }
      }

      private void RaiseCurrentChangedEvent()
      {
         RoutedEventArgs eventArgs = new RoutedEventArgs(CurrentChangedEvent, dgProxy.ElementAsEventSource);
         dgProxy.RaiseEvent(eventArgs);
      }

      #endregion

      DependencyPropertyChangeListener currentItemPropertyChangeListener;
      DataGrid DataGridElement;
      ICollectionView itemsView;
      AutoResetFlag isSelfInducedChange = new AutoResetFlag();


      public DataGridAsCurrentItemProvider(DataGrid dataGrid)
      {
         this.DataGridElement = dataGrid;
         this.dgProxy = (EnhancedDGProxy)FrameworkElementProxy.GetProxy(dataGrid);
         Debug.Assert(dgProxy != null, "The attached element must have a proxy");
         currentItemPropertyChangeListener = new DependencyPropertyChangeListener(DataGridElement, DataGrid.CurrentItemProperty, DataGrid_CurrentItemChanged);

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
            RaiseNonCancelablePreviewCurrentChangingEvent();
         }
         else
         {
            bool canceled;
            RaisePreviewCurrentChangingEvent(out canceled);
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
         OnPropertyChanged("CurrentItem");
         OnPropertyChanged("CurrentPosition");
      }


      private void DataGrid_CurrentItemChanged(object sender, EventArgs args)
      {
         if (!isSelfInducedChange.IsSet)
         {
            itemsView.MoveCurrentTo(DataGridElement.CurrentItem);
         }
      }

      public object CurrentItem
      {
         get
         {
            return itemsView.CurrentItem;
         }
      }

      public int CurrentPosition
      {
         get
         {
            return itemsView.CurrentPosition;
         }
      }


      #region ICurrentItemProvider Members


      public bool MoveCurrentTo(object item)
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentTo(item);
      }

      public bool MoveCurrentToFirst()
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentToFirst();
      }

      public bool MoveCurrentToNext()
      {
         return MoveCurrentToRelativePosition(+1);
      }

      public bool MoveCurrentToPrevious()
      {
         return MoveCurrentToRelativePosition(-1);
      }

      public bool MoveCurrentToLast()
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentToLast();
      }

      public bool MoveCurrentToPosition(int position)
      {
         using (isSelfInducedChange.Set())
            return itemsView.MoveCurrentToPosition(position);
      }

      public bool MoveCurrentToRelativePosition(int offset)
      {
         int newPosition = CurrentPosition + offset;
         return MoveCurrentToPosition(newPosition);
      }

      #endregion



   }

}
