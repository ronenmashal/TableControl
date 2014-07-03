using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICellEnumerationService))]
   internal class DataGridStandardRowCellEnumerationService : ICellEnumerationService, IUIService
   {
      private DataGrid owningGrid = null;

      private DataGridRow rowElement = null;

      public object ServiceGroupIdentifier
      {
         get { return "__default row type__"; }
      }

      private DataGrid OwningGrid
      {
         get
         {
            if (owningGrid == null && rowElement != null)
               owningGrid = UIUtils.GetAncestor<DataGrid>(rowElement);
            return owningGrid;
         }
      }

      #region IUIService Members

      public virtual bool IsAttached { get { return rowElement != null; } }

      public void AttachToElement(FrameworkElement element)
      {
         Debug.Assert(rowElement == null, this.GetType().Name + " is already attached.");
         rowElement = element as DataGridRow;
         if (rowElement == null)
            throw new ArgumentException("Must be attached to DataGridRow");
      }

      public void DetachFromElement(FrameworkElement element)
      {
         rowElement = null;
         owningGrid = null;
      }

      #endregion IUIService Members

      #region ICellEnumerationService Members

      public int CellCount
      {
         get { return OwningGrid.Columns.Count; }
      }

      public int CurrentCellIndex
      {
         get 
         {
            if (OwningGrid.CurrentColumn == null)
               return 0;
            return OwningGrid.CurrentColumn.DisplayIndex; 
         }
      }

      public FrameworkElement GetCellAt(int index)
      {
         if (index < 0 || index >= CellCount)
            throw new IndexOutOfRangeException("Argument index should be between 0 and " + CellCount);
         return OwningGrid.ColumnFromDisplayIndex(index).GetCellContent(rowElement);
      }

      public UniversalCellInfo GetCurrentCellInfo()
      {
         if (OwningGrid == null)
            return new UniversalCellInfo(null, -1);
         return ConvertDataGridCellInfo(OwningGrid.CurrentCell);
      }

      #endregion ICellEnumerationService Members

      #region IDisposable Members

      public void Dispose()
      {
         DetachFromElement(rowElement);
      }

      #endregion IDisposable Members

      public bool MoveToCell(int cellIndex)
      {
         OwningGrid.CurrentCell = new DataGridCellInfo(rowElement.Item, OwningGrid.ColumnFromDisplayIndex(cellIndex));
         return OwningGrid.CurrentCell.IsValid;
      }

      private UniversalCellInfo ConvertDataGridCellInfo(DataGridCellInfo dgCellInfo)
      {
         int currentColumnIndex = -1;
         if (dgCellInfo.Column != null)
            currentColumnIndex = OwningGrid.CurrentCell.Column.DisplayIndex;

         object currentItem = null;
         if (dgCellInfo.Item != DependencyProperty.UnsetValue)
            currentItem = OwningGrid.CurrentCell.Item;

         return new UniversalCellInfo(currentItem, currentColumnIndex);
      }
   }
}