using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Table.CellTypes;
using MagicSoftware.Common.Utils;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Implementation of ICurrentCellService for DataGrid. The implementation operates only 
   /// on accessible cells - i.e. cells that have already been loaded. It cannot move to a cell
   /// placed on an item that was not loaded (due to virtualization, for example).
   /// </summary>
   class DataGridCurrentCellService : ICurrentCellService, IUIService
   {
      #region CurrentCellChanging event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler<PreviewChangeEventArgs> PreviewCurrentCellChanging;

      /// <summary>
      /// Raises the PreviewCurrentCellChangingEvent, allowing the handlers to cancel it,
      /// returning the result in 'canceled'
      /// </summary>
      /// 
      /// <param name="canceled">Returns whether any of the event handlers canceled the event.</param>
      public void RaisePreviewCurrentCellChangingEvent(UniversalCellInfo newValue, out bool canceled)
      {
         canceled = false;
         if (PreviewCurrentCellChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentCell, newValue, true);
            PreviewCurrentCellChanging(this, eventArgs);
            canceled = eventArgs.Canceled;
         }
      }

      /// <summary>
      /// Raises the PreviewCurrentCellChangingEvent without allowing canceling of the event.
      /// </summary>
      public void RaiseNonCancelablePreviewCurrentCellChangingEvent(UniversalCellInfo newValue)
      {
         if (PreviewCurrentCellChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentCell, newValue, false);
            PreviewCurrentCellChanging(this, eventArgs);
         }
      }

      #endregion

      #region CurrentCellChanged event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler CurrentCellChanged;

      public void RaiseCurrentCellChangedEvent()
      {
         if (CurrentCellChanged != null)
            CurrentCellChanged(this, new EventArgs());
      }

      #endregion


      private System.Windows.Controls.DataGrid dataGrid;

      /// <summary>
      /// Determines whether the current cell change is of an external origin, i.e. the 
      /// cell position was changed by another component; or the change is caused by
      /// this class (self induced).
      /// </summary>
      protected readonly AutoResetFlag isSelfInducedCellChange = new AutoResetFlag();

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         //currentRowService = UIServiceProvider.GetService<ICurrentItemService>(element);
         dataGrid.CurrentCellChanged += DataGrid_CurrentCellChanged;
         dataGrid.ColumnDisplayIndexChanged += DataGrid_ColumnDisplayIndexChanged;
         UpdateCurrentCell();
      }

      public void DetachFromElement(FrameworkElement element)
      {
         dataGrid.CurrentCellChanged -= DataGrid_CurrentCellChanged;
         dataGrid.ColumnDisplayIndexChanged -= DataGrid_ColumnDisplayIndexChanged;
      }

      void DataGrid_CurrentCellChanged(object sender, EventArgs e)
      {
         if (!isSelfInducedCellChange.IsSet)
            RaiseNonCancelablePreviewCurrentCellChangingEvent(ConvertDataGridCellInfo(dataGrid.CurrentCell));
         UpdateCurrentCell();
         RaiseCurrentCellChangedEvent();
      }

      void DataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
      {
         UpdateCurrentCell();
      }


      void UpdateCurrentCell()
      {
         CurrentCell = ConvertDataGridCellInfo(dataGrid.CurrentCell);
         CurrentRowCellEnumerationService = GetRowEnumerationServiceForItem(CurrentCell.Item);
      }

      ICellEnumerationService CurrentRowCellEnumerationService { get; set; }

      UniversalCellInfo ConvertDataGridCellInfo(DataGridCellInfo dgCellInfo)
      {
         int currentColumnIndex = -1;
         if (dgCellInfo.Column != null)
            currentColumnIndex = dataGrid.CurrentCell.Column.DisplayIndex;

         object currentItem = null;
         if (dgCellInfo.Item != DependencyProperty.UnsetValue)
            currentItem = dataGrid.CurrentCell.Item;

         return new UniversalCellInfo(currentItem, currentColumnIndex);
      }

      public UniversalCellInfo CurrentCell
      {
         get;
         private set;
      }

      public bool IsCellVisible
      {
         get
         {
            return false;
         }
      }

      public FrameworkElement CurrentCellElement
      {
         get { return null; }
      }

      public bool MoveTo(UniversalCellInfo targetCell)
      {
         bool canceled;

         if (!CanMoveTo(targetCell))
            return false;

         RaisePreviewCurrentCellChangingEvent(targetCell, out canceled);
         if (canceled)
            return false;

         using (isSelfInducedCellChange.Set())
         {
            if (targetCell.CellIndex >= dataGrid.Columns.Count)
               return false;
            dataGrid.CurrentCell = new DataGridCellInfo(targetCell.Item, dataGrid.ColumnFromDisplayIndex(targetCell.CellIndex));
         }
         
         return CurrentCell.Equals(targetCell);
      }

      public bool MoveUp(uint distance)
      {
         return false;
      }

      public bool MoveDown(uint distance)
      {
         int newIndex = IndexOf(CurrentCell.Item) + (int)distance;
         int maxIndex = dataGrid.Items.Count - 1;
         if (newIndex > maxIndex)
            return false;

         return MoveTo(new UniversalCellInfo(dataGrid.Items[newIndex], CurrentCell.CellIndex));
      }

      public bool MoveLeft(uint distance)
      {
         return false;
      }

      public bool MoveRight(uint distance)
      {
         return false;
      }

      #region IDisposable Members

      public void Dispose()
      {

      }

      #endregion



      private bool CanMoveTo(UniversalCellInfo targetCell)
      {
         if (IndexOf(targetCell.Item) < 0)
            return false;

         var targetRowCellEnumerationService = GetRowEnumerationServiceForItem(targetCell.Item);
         if (targetRowCellEnumerationService == null)
            return false;

         if (targetCell.CellIndex < 0 || targetCell.CellIndex >= targetRowCellEnumerationService.CellCount)
            return false;

         return true;
      }

      private ICellEnumerationService GetRowEnumerationServiceForItem(object item)
      {
         var currentRow = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
         if (currentRow == null)
            return null;

         //HIGH: This should be done using UIServiceProvider
         var service = new DataGridStandardRowCellEnumerationService();
         ((IUIService)service).AttachToElement(currentRow);

         return service;
      }


      private int IndexOf(object item)
      {
         if (item == null)
            return -1;

         return dataGrid.Items.IndexOf(item);
      }

      private UIElement ForceContainerGeneration(object item)
      {
         int itemIndex = IndexOf(item);
         UIElement container = null;
         bool isNewlyRealized = false;
         IItemContainerGenerator generator = dataGrid.ItemContainerGenerator;
         GeneratorPosition gp = generator.GeneratorPositionFromIndex(itemIndex);
         using (generator.StartAt(gp, GeneratorDirection.Forward, true))
         {
            isNewlyRealized = false;
            container = generator.GenerateNext(out isNewlyRealized) as UIElement;
         }
         return container;
      }
   }

   interface ICellEnumerationService
   {
      FrameworkElement GetCellAt(int index);
      int CellCount { get; }
   }

   class EmptyRowCellEnumerationService : ICellEnumerationService, IUIService
   {
      #region IUIService Members

      public void AttachToElement(FrameworkElement element)
      {
      }

      public void DetachFromElement(FrameworkElement element)
      {
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
      }

      #endregion

      #region ICellEnumerationService Members

      public FrameworkElement GetCellAt(int index)
      {
         throw new ArgumentOutOfRangeException("An empty row has no cells in it.");
      }

      public int CellCount
      {
         get { return 0; }
      }

      #endregion
   }

   class DataGridStandardRowCellEnumerationService : ICellEnumerationService, IUIService
   {
      DataGridRow rowElement = null;
      DataGrid owningGrid = null;

      #region IUIService Members

      public void AttachToElement(FrameworkElement element)
      {
         rowElement = element as DataGridRow;
         rowElement.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => owningGrid = UIUtils.GetAncestor<DataGrid>(rowElement)));
         if (rowElement == null)
            throw new ArgumentException("Must be attached to DataGridRow");
      }

      public void DetachFromElement(FrameworkElement element)
      {
         rowElement = null;
         owningGrid = null;
      }

      #endregion

      #region ICellEnumerationService Members

      public FrameworkElement GetCellAt(int index)
      {
         if (index < 0 || index >= CellCount)
            throw new IndexOutOfRangeException("Argument index should be between 0 and " + CellCount);
         return owningGrid.ColumnFromDisplayIndex(index).GetCellContent(rowElement);
      }

      public int CellCount
      {
         get { return owningGrid.Columns.Count; }
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         DetachFromElement(rowElement);
      }

      #endregion
   }
}
