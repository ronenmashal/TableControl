using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICellEnumerationService))]
   internal class DataGridStandardRowCellEnumerationService : ICellEnumerationService, IUIService
   {
      private DataGrid owningGrid = null;

      private DataGridRow rowElement = null;

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

      public void AttachToElement(FrameworkElement element)
      {
         rowElement = element as DataGridRow;
         //rowElement.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => owningGrid = UIUtils.GetAncestor<DataGrid>(rowElement)));
         if (rowElement == null)
            throw new ArgumentException("Must be attached to DataGridRow");
      }

      public void DetachFromElement(FrameworkElement element)
      {
         rowElement = null;
         owningGrid = null;
      }

      public virtual bool IsAttached { get { return rowElement != null; } }


      #endregion IUIService Members

      #region ICellEnumerationService Members

      public int CellCount
      {
         get { return OwningGrid.Columns.Count; }
      }

      public int CurrentCellIndex
      {
         get { return OwningGrid.CurrentColumn.DisplayIndex; }
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