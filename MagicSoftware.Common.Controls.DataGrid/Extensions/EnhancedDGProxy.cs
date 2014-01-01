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
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class ImprovedItemsControlProxy : ItemsControlProxy
   {
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

      public void AddRoutedEventHandler(RoutedEvent routedEvent, RoutedEventHandler handler)
      {
         ProxiedElement.AddHandler(routedEvent, handler);
      }

      public void RemoveRoutedEventHandler(RoutedEvent routedEvent, RoutedEventHandler handler)
      {
         ProxiedElement.RemoveHandler(routedEvent, handler);
      }


      public object ElementAsEventSource { get { return ProxiedElement; } }

      //TODO: Move to FrameworkElementProxy.
      /// <summary>
      /// Gets the proxied element's dispatcher.
      /// </summary>
      /// <returns>Dispatcher for UI operations.</returns>
      internal Dispatcher GetDispatcher()
      {
         return this.ProxiedElement.Dispatcher;
      }

      internal void RaiseEvent(RoutedEventArgs eventArgs)
      {
         ProxiedElement.RaiseEvent(eventArgs);
      }

      public FrameworkElementProxy GetItemContainerProxy(object item)
      {
         var container = this.ContainerFromItem(item);
         if (container == null)
            return null;

         return GetItemContainerProxy(container);
      }

      public FrameworkElementProxy GetItemContainerProxy(FrameworkElement container)
      {
         var proxy = FrameworkElementProxy.GetProxy(container);
         if (proxy == null)
         {
            proxy = CreateItemContainerProxy(container);
            FrameworkElementProxy.SetProxy(container, proxy);
         }
         return proxy;
      }

      protected virtual FrameworkElementProxy CreateItemContainerProxy(FrameworkElement itemContainer)
      {
         var itemContainerProxy = new FrameworkElementProxy();
         return itemContainerProxy;
      }
   }

   public class EnhancedDGProxy : ImprovedItemsControlProxy
   {
      private DataGrid DataGridElement { get { return (DataGrid)ProxiedElement; } }

      public override void ScrollIntoView(object item)
      {
         if (item != null)
            DataGridElement.ScrollIntoView(item);
      }

      ICurrentItemProvider currentItemProvider = null;

      protected override object GetAdapter(Type adapterType)
      {
         if (adapterType == typeof(ICurrentItemProvider))
         {
            if (currentItemProvider == null)
               currentItemProvider = new DataGridAsCurrentItemProvider(DataGridElement);
            return currentItemProvider;
         }

         if (adapterType == typeof(IElementEditStateProxy))
            return new DataGridAsEditingAdapter(DataGridElement, this);

         return base.GetAdapter(adapterType);
      }

      public object SelectedItemContainer()
      {
         if (DataGridElement.SelectedIndex >= 0)
            return DataGridElement.ItemContainerGenerator.ContainerFromIndex(DataGridElement.SelectedIndex);

         else return null;
      }

      protected override FrameworkElementProxy CreateItemContainerProxy(FrameworkElement itemContainer)
      {
         if (itemContainer is DataGridRow)
         {
            return new ItemsContainerProxy();
         }
         return base.CreateItemContainerProxy(itemContainer);
      }
   }

   class ItemsContainerProxy : FrameworkElementProxy
   {
      protected override void Initialize()
      {
         base.Initialize();
      }

      protected override object GetAdapter(Type adapterType)
      {
         if (ProxiedElement is DataGridRow)
         {
            if (adapterType == typeof(ICurrentItemProvider))
               return new DataGridRowAsCurrentItemProvider(ProxiedElement as DataGridRow);
         }
         return base.GetAdapter(adapterType);
      }
   }

   class DataGridRowAsCurrentItemProvider : ICurrentItemProvider
   {
      public event CancelableRoutedEventHandler PreviewCurrentChanging;

      public event RoutedEventHandler CurrentChanged;
      private DataGridRow dataGridRow;
      private DataGrid owner;

      public DataGridRowAsCurrentItemProvider(DataGridRow dataGridRow)
      {
         this.dataGridRow = dataGridRow;
         owner = UIUtils.GetAncestor<DataGrid>(dataGridRow);
      }

      public object CurrentItem
      {
         get
         {
            if (owner.CurrentColumn == null)
               return null;
            return owner.CurrentColumn.GetCellContent(dataGridRow); 
         }
      }

      public int CurrentPosition
      {
         get 
         {
            if (owner.CurrentColumn == null)
               return -1;
            return owner.CurrentColumn.DisplayIndex; 
         }
      }

      bool MoveToCell(DataGridCell cell)
      {
         //HIGH: Should raise preview change event.

         DataGridCellInfo newCellInfo = new DataGridCellInfo(cell);
         owner.CurrentCell = newCellInfo;
         return owner.CurrentCell.Column == cell.Column;

         //HIGH: Should raise changed event.
      }

      public bool MoveCurrentTo(object item)
      {
         var cell = UIUtils.GetAncestor<DataGridCell>(item as UIElement);
         return MoveToCell(cell);
      }

      public bool MoveCurrentToFirst()
      {
         return MoveCurrentToPosition(0);
      }

      public bool MoveCurrentToNext()
      {
         return MoveCurrentToRelativePosition(1);
      }

      public bool MoveCurrentToPrevious()
      {
         return MoveCurrentToRelativePosition(-1);
      }

      public bool MoveCurrentToLast()
      {
         return MoveCurrentToPosition(owner.Columns.Count - 1);
      }

      public bool MoveCurrentToPosition(int position)
      {
         var targetElement = owner.ColumnFromDisplayIndex(position).GetCellContent(dataGridRow);
         return MoveCurrentTo(targetElement);
      }

      public bool MoveCurrentToRelativePosition(int offset)
      {
         int nextColumnIndex = 0;
         var currentColumn = owner.CurrentColumn;
         if (currentColumn != null)
            nextColumnIndex = currentColumn.DisplayIndex + offset;
         if (nextColumnIndex < 0)
            nextColumnIndex = 0;
         if (nextColumnIndex >= owner.Columns.Count)
            nextColumnIndex = owner.Columns.Count - 1;
         var targetElement = owner.ColumnFromDisplayIndex(nextColumnIndex).GetCellContent(dataGridRow);
         return MoveCurrentTo(targetElement);
      }
   }

}
