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
using System.Diagnostics;
using System.Windows.Documents;
using MagicSoftware.Common.Controls.Table.CellTypes;

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
         if (container == null)
            return null;
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

      SharedObjectsService sharedObjectsService = new SharedObjectsService();

      protected override object GetAdapter(Type adapterType)
      {
         if (adapterType == typeof(SharedObjectsService))
            return sharedObjectsService;

         return base.GetAdapter(adapterType);
      }
   }

   public class SharedObjectsService
   {
      Dictionary<string, object> sharedObjects = new Dictionary<string, object>();

      public void SetSharedObject(string identifier, object sharedObject)
      {
         sharedObjects[identifier] = sharedObject;
      }

      public object GetSharedObject(string identifier)
      {
         return sharedObjects[identifier];
      }

      internal bool HasSharedObject(string identifier)
      {
         return sharedObjects.ContainsKey(identifier);
      }
   }

   public class EnhancedDGProxy : ImprovedItemsControlProxy
   {
      private DataGrid DataGridElement { get { return (DataGrid)ProxiedElement; } }

      public static readonly DependencyProperty SelectAllButtonTemplateProperty = DependencyProperty.RegisterAttached("SelectAllButtonTemplate", typeof(Style), typeof(EnhancedDataGrid), new UIPropertyMetadata(null, OnSelectAllButtonTemplateChanged));

      public static Style GetSelectAllButtonTemplate(DataGrid obj)
      {
         return (Style)obj.GetValue(SelectAllButtonTemplateProperty);
      }

      public static void SetSelectAllButtonTemplate(DataGrid obj, Style value)
      {
         obj.SetValue(SelectAllButtonTemplateProperty, value);
      }

      private static void OnSelectAllButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         DataGrid dataGrid = d as DataGrid;
         if (dataGrid == null)
         {
            return;
         }

         EventHandler handler = null;
         handler = delegate
         {
            dataGrid.LayoutUpdated -= handler;
            var button = UIUtils.GetVisualChild<Button>(dataGrid);
            if (button != null)
            {
               Style template = GetSelectAllButtonTemplate(dataGrid);
               button.Style = template;
            }
         };

         dataGrid.LayoutUpdated += handler;
      }

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

      Style defaultRowStyle;

      protected override void Initialize()
      {
         base.Initialize();

         DataGridElement.LoadingRow += DataGridElement_LoadingRow;
         DataGridElement.UnloadingRow += DataGridElement_UnloadingRow;
         defaultRowStyle = DataGridElement.FindResource(typeof(DataGridRow)) as Style;
      }



      public static bool GetIsCustomRow(DependencyObject obj)
      {
         return (bool)obj.GetValue(IsCustomRowProperty);
      }

      public static void SetIsCustomRow(DependencyObject obj, bool value)
      {
         obj.SetValue(IsCustomRowProperty, value);
      }

      // Using a DependencyProperty as the backing store for IsCustomRow.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty IsCustomRowProperty =
          DependencyProperty.RegisterAttached("IsCustomRow", typeof(bool), typeof(EnhancedDGProxy), new UIPropertyMetadata(false));


      void DataGridElement_LoadingRow(object sender, DataGridRowEventArgs eventArgs)
      {
         Trace.WriteLine("Loading row for " + eventArgs.Row.Item);
         if (DataGridElement.RowStyleSelector != null)
         {
            Style rowStyle = DataGridElement.RowStyleSelector.SelectStyle(eventArgs.Row.Item, eventArgs.Row);
            if (rowStyle != null)
               SetIsCustomRow(eventArgs.Row, true);
         }
      }

      void DataGridElement_UnloadingRow(object sender, DataGridRowEventArgs e)
      {
         unloadedRows.Add(e.Row);
         var container = e.Row;
         if (container != null)
         {
            var adornerLayer = AdornerLayer.GetAdornerLayer(container);
            if (adornerLayer != null)
            {
               Adorner[] adorners = adornerLayer.GetAdorners(container);
               if (adorners != null)
               {
                  foreach (var adorner in adorners)
                     adornerLayer.Remove(adorner);
               }
            }
         }
      }

      HashSet<DataGridRow> unloadedRows = new HashSet<DataGridRow>();
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
            {
               var row = ProxiedElement as DataGridRow;
               if (EnhancedDGProxy.GetIsCustomRow(row))
               {
                  return new CustomRowAsCurrentItemProvider(ProxiedElement as DataGridRow);
               }
               return new DataGridRowAsCurrentItemProvider(ProxiedElement as DataGridRow);
            }

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

   class CustomRowAsCurrentItemProvider : ICurrentItemProvider
   {
      const string CurrentPositionIdentifier = "CustomRow.CurrentPosition";

      #region ICurrentItemProvider Members

      public event CancelableRoutedEventHandler PreviewCurrentChanging;

      public event RoutedEventHandler CurrentChanged;

      List<VirtualTableCell> cells;
      SharedObjectsService sharedObjectsService;

      public CustomRowAsCurrentItemProvider(DataGridRow row)
      {
         // Enumerate the virtual table cells in the row.
         cells = UIUtils.GetVisualChildren<VirtualTableCell>(row, (cell) => true);

         // Ensure current position shared value exists in the owner's proxy.
         DataGrid owner = UIUtils.GetAncestor<DataGrid>(row);
         FrameworkElementProxy ownerProxy = FrameworkElementProxy.GetProxy(owner);
         sharedObjectsService = ownerProxy.GetAdapter<SharedObjectsService>();

         if (!sharedObjectsService.HasSharedObject(CurrentPositionIdentifier))
         {
            if (cells.Count > 0)
               CurrentPosition = 0;
            else
               CurrentPosition = -1;
         }
      }

      public object CurrentItem
      {
         get
         {
            if (CurrentPosition < 0 || CurrentPosition >= cells.Count)
               return null;
            return cells[CurrentPosition];
         }
      }

      public int CurrentPosition
      {
         get { return (int)sharedObjectsService.GetSharedObject(CurrentPositionIdentifier); }
         private set { sharedObjectsService.SetSharedObject(CurrentPositionIdentifier, value); }
      }

      public bool MoveCurrentTo(object item)
      {
         if (item == null)
            return false;

         var cell = UIUtils.GetAncestor<VirtualTableCell>(item as UIElement);
         if (cell == null)
            return false;

         int cellIndex = cells.IndexOf(cell);
         return MoveCurrentToPosition(cellIndex);
      }

      public bool MoveCurrentToFirst()
      {
         return MoveCurrentToPosition(0);
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
         return MoveCurrentToPosition(cells.Count - 1);
      }

      public bool MoveCurrentToPosition(int position)
      {
         // Preview
         this.CurrentPosition = position;
         bool result = false;
         if (CurrentItem != null)
         {
            result = ((FrameworkElement)CurrentItem).MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
         }
         // current changed.
         return result;
      }

      public bool MoveCurrentToRelativePosition(int offset)
      {
         return MoveCurrentToPosition(CurrentPosition + offset);
      }

      #endregion
   }
}
