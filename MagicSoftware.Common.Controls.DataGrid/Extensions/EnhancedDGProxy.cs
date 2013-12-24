using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Proxies;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class EnhancedDGProxy : DataGridProxy, INotifyPropertyChanged
   {
      //TODO: This should move up to FrameworkElementProxy
      public event PropertyChangedEventHandler PropertyChanged;

      //TODO: Move up to ItemsControlProxy.
      #region CurrentChanging attached event.

      public static readonly RoutedEvent PreviewCurrentChangingEvent = EventManager.RegisterRoutedEvent("PreviewCurrentChanging", RoutingStrategy.Tunnel, typeof(CancelableRoutedEventArgs), typeof(EnhancedDGProxy));

      public static void AddPreviewCurrentChangingEventHandler(DependencyObject d, CancelableRoutedEventHandler handler)
      {
         UIElement uie = d as UIElement;
         if (uie != null)
         {
            uie.AddHandler(EnhancedDGProxy.PreviewCurrentChangingEvent, new RoutedEventHandler((obj, args) => { handler(obj, (CancelableRoutedEventArgs)args); }));
         }
      }

      public static void RemovePreviewCurrentChangingHandler(DependencyObject d, CancelableRoutedEventHandler handler)
      {
         UIElement uie = d as UIElement;
         if (uie != null)
         {
            uie.RemoveHandler(EnhancedDGProxy.PreviewCurrentChangingEvent, handler);
         }
      }

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event CancelableRoutedEventHandler PreviewCurrentChanging
      {
         add { AddPreviewCurrentChangingEventHandler(this.ProxiedElement, value); }
         remove { RemovePreviewCurrentChangingHandler(this.ProxiedElement, value); }
      }

      public void RaisePreviewCurrentChangingEvent(out bool canceled)
      {
         CancelableRoutedEventArgs eventArgs = new CancelableRoutedEventArgs(PreviewCurrentChangingEvent, ProxiedElement);
         ProxiedElement.RaiseEvent(eventArgs);
         canceled = eventArgs.Canceled;
      }

      #endregion

      #region Commands

      public event CanExecuteRoutedEventHandler PreviewCanExecute
      {
         add { CommandManager.AddPreviewCanExecuteHandler(ProxiedElement, value); }
         remove { CommandManager.RemovePreviewCanExecuteHandler(ProxiedElement, value); }
      }

      public void ExecuteCommand(RoutedCommand command, object commandParameter)
      {
         command.Execute(commandParameter, ProxiedElement);
      }

      #endregion

      private DataGrid DataGridElement { get { return (DataGrid)ProxiedElement; } }

      CollectionView currentItemView;
      DependencyPropertyDescriptor currentItemPropDesc;

      protected override void Initialize()
      {
         base.Initialize();
         DataGridElement.Loaded += DataGridElement_Loaded;
         currentItemPropDesc = DependencyPropertyDescriptor.FromProperty(DataGrid.CurrentItemProperty, typeof(DataGrid));
      }

      void DataGridElement_Loaded(object sender, System.Windows.RoutedEventArgs e)
      {
         DataGridElement.Loaded -= DataGridElement_Loaded;
         if (DataGridElement.ItemsSource != null)
         {
            currentItemView = new CollectionView(DataGridElement.ItemsSource);
            currentItemView.CurrentChanged += currentItemView_CurrentChanged;
            currentItemPropDesc.AddValueChanged(DataGridElement, DataGrid_CurrentItemChanged);
         }
      }

      protected override void Cleanup()
      {
         currentItemView.CurrentChanged -= currentItemView_CurrentChanged;
         currentItemPropDesc.RemoveValueChanged(DataGridElement, DataGrid_CurrentItemChanged);
         base.Cleanup();
      }

      void currentItemView_CurrentChanged(object sender, EventArgs e)
      {
         DataGridElement.CurrentItem = currentItemView.CurrentItem;
         OnPropertyChanged("CurrentItem");
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

      public override void ScrollIntoView(object item)
      {
         if (item != null)
            DataGridElement.ScrollIntoView(item);
      }

      public bool MoveCurrent(ICollectionViewMoveAction moveAction)
      {
         bool canceled;
         RaisePreviewCurrentChangingEvent(out canceled);
         
         if (canceled)
            return false;

         return moveAction.Move(currentItemView);
      }

      protected virtual void OnPropertyChanged(string propertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      public ICollectionViewMoveAction GetMoveToFirstItemAction()
      {
         return new MoveCurrentToPositionAction() { NewPosition = 0 };
      }

      public ICollectionViewMoveAction GetMoveToLastItemAction()
      {
         return new MoveCurrentToPositionAction() { NewPosition = DataGridElement.Items.Count - 1 };
      }

      protected override object GetAdapter(Type adapterType)
      {
         if (adapterType == typeof(ICurrentItemProvider))
            return new DGProxyAsCurrentItemProvider(this);

         if (adapterType == typeof(IElementEditStateProxy))
            return new DataGridAsEditingAdapter(DataGridElement, this);

         return base.GetAdapter(adapterType);
      }
   }
}
