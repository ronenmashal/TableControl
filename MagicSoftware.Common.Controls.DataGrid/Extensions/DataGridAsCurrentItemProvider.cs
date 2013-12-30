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
      /// <param name="targetItem">The item on which the current item pointer will refer after the change.</param>
      /// <param name="canceled">Returns whether any of the event handlers canceled the event.</param>
      private void RaisePreviewCurrentChangingEvent(object targetItem, out bool canceled)
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

      DependencyPropertyDescriptor currentItemPropDesc;
      DataGrid DataGridElement;
      //CollectionView currentItemView;
      AutoResetFlag isSelfInducedChange = new AutoResetFlag();


      public DataGridAsCurrentItemProvider(DataGrid dataGrid)
      {
         this.DataGridElement = dataGrid;
         this.dgProxy = (EnhancedDGProxy)FrameworkElementProxy.GetProxy(dataGrid);
         Debug.Assert(dgProxy != null, "The attached element must have a proxy");
         currentItemPropDesc = DependencyPropertyDescriptor.FromProperty(DataGrid.CurrentItemProperty, typeof(DataGrid));

         if (DataGridElement.IsLoaded)
            RegisterEventHandlers();
         else
         {
            RoutedEventHandler handler = null;
            handler = (sender, e) =>
            {
               DataGridElement.Loaded -= handler;
               RegisterEventHandlers();
            };
            DataGridElement.Loaded += handler;
         }
      }

      void RegisterEventHandlers()
      {
         if (DataGridElement.ItemsSource != null)
         {
            //currentItemView = new CollectionView(DataGridElement.ItemsSource);
            //currentItemView.CurrentChanged += currentItemView_CurrentChanged;
            currentItemPropDesc.AddValueChanged(DataGridElement, DataGrid_CurrentItemChanged);
         }
      }

      public void Dispose()
      {
         if (dgProxy.ItemsView != null)
         {
            //currentItemView.CurrentChanged -= currentItemView_CurrentChanged;
            currentItemPropDesc.RemoveValueChanged(DataGridElement, DataGrid_CurrentItemChanged);
         }
      }

      void currentItemView_CurrentChanged(object sender, EventArgs e)
      {
         //DataGridElement.CurrentItem = currentItemView.CurrentItem;
         RaiseCurrentChangedEvent();
         OnPropertyChanged("CurrentItem");
      }


      private void DataGrid_CurrentItemChanged(object sender, EventArgs args)
      {
         if (!isSelfInducedChange.IsSet)
         {
            RaiseNonCancelablePreviewCurrentChangingEvent();
         }
         RaiseCurrentChangedEvent();
         OnPropertyChanged("CurrentItem");
         OnPropertyChanged("CurrentPosition");
      }

      public object CurrentItem
      {
         get
         {
            return DataGridElement.CurrentItem;
         }
      }

      public int CurrentPosition
      {
         get
         {
            if (CurrentItem == null)
               return -1;

            return DataGridElement.Items.IndexOf(CurrentItem);
         }
      }


      #region ICurrentItemProvider Members


      public bool MoveCurrentTo(object item)
      {
         if (object.ReferenceEquals(item, CurrentItem))
            return true;

         bool canceled;
         RaisePreviewCurrentChangingEvent(item, out canceled);

         if (canceled)
            return false;

         using (isSelfInducedChange.Set())
         {
            var currentCell = DataGridElement.CurrentCell;
            var nextColumn = currentCell.Column;
            if (nextColumn == null)
               nextColumn = DataGridElement.ColumnFromDisplayIndex(0);
            var newCell = new DataGridCellInfo(item, nextColumn);
            DataGridElement.CurrentCell = newCell;
            return object.ReferenceEquals(DataGridElement.CurrentItem, item);
         }
      }

      public bool MoveCurrentToFirst()
      {
         if (DataGridElement.Items.Count > 0)
            return MoveCurrentTo(DataGridElement.Items[0]);
         else
            return MoveCurrentTo(null);
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
         return MoveCurrentToPosition(DataGridElement.Items.Count - 1);
      }

      public bool MoveCurrentToPosition(int position)
      {
         Object itemAtPosition = null;
         if (position >= DataGridElement.Items.Count)
            position = DataGridElement.Items.Count - 1;
         if (position >= 0)
            itemAtPosition = DataGridElement.Items.GetItemAt(position);
         return MoveCurrentTo(itemAtPosition);
      }

      public bool MoveCurrentToRelativePosition(int offset)
      {
         var currentItemPosition = -1;
         if (DataGridElement.CurrentItem != null)
         {
            currentItemPosition = DataGridElement.Items.IndexOf(DataGridElement.CurrentItem);
         }
         int newPosition = currentItemPosition + offset;
         return MoveCurrentToPosition(newPosition);
      }

      #endregion
   }

}
