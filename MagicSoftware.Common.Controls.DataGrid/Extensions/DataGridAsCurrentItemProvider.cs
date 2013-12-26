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

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridAsCurrentItemProvider : ObservableObject, ICurrentItemProvider, IWeakEventListener
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

      private void RaisePreviewCurrentChangingEvent(out bool canceled)
      {
         CancelableRoutedEventArgs eventArgs = new CancelableRoutedEventArgs(PreviewCurrentChangingEvent, dgProxy.ElementAsEventSource);
         dgProxy.RaiseEvent(eventArgs);
         canceled = eventArgs.Canceled;
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
      CollectionView currentItemView;


      public DataGridAsCurrentItemProvider(DataGrid dataGrid)
      {
         this.DataGridElement = dataGrid;
         this.dgProxy = (EnhancedDGProxy)FrameworkElementProxy.GetProxy(dataGrid);
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
            currentItemView = new CollectionView(DataGridElement.ItemsSource);
            currentItemView.CurrentChanged += currentItemView_CurrentChanged;
            currentItemPropDesc.AddValueChanged(DataGridElement, DataGrid_CurrentItemChanged);
         }
      }

      public void Dispose()
      {
         if (dgProxy.ItemsView != null)
         {
            currentItemView.CurrentChanged -= currentItemView_CurrentChanged;
            currentItemPropDesc.RemoveValueChanged(DataGridElement, DataGrid_CurrentItemChanged);
         }
      }

      void currentItemView_CurrentChanged(object sender, EventArgs e)
      {
         DataGridElement.CurrentItem = currentItemView.CurrentItem;
         OnPropertyChanged("CurrentItem");
         RaiseCurrentChangedEvent();
      }


      public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
      {
         OnPropertyChanged("CurrentItem");
         return true;
      }



      public bool MoveCurrent(ICollectionViewMoveAction moveAction)
      {
         bool canceled;
         RaisePreviewCurrentChangingEvent(out canceled);

         if (canceled)
            return false;

         return moveAction.Move(currentItemView);
      }

      private void DataGrid_CurrentItemChanged(object sender, EventArgs args)
      {
         currentItemView.MoveCurrentTo(DataGridElement.CurrentItem);
      }

      public object CurrentItem
      {
         get
         {
            if (currentItemView == null)
               return null;
            return currentItemView.CurrentItem;
         }
      }

      public int CurrentPosition
      {
         get
         {
            return currentItemView.CurrentPosition;
         }
         set
         {
            currentItemView.MoveCurrentToPosition(value);
         }
      }

   }

}
