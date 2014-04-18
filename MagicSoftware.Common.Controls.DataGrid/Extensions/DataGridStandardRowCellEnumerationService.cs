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

      #endregion IUIService Members

      #region ICellEnumerationService Members

      public int CellCount
      {
         get { return owningGrid.Columns.Count; }
      }

      public int CurrentCellIndex
      {
         get { return owningGrid.CurrentColumn.DisplayIndex; }
      }

      public FrameworkElement GetCellAt(int index)
      {
         if (index < 0 || index >= CellCount)
            throw new IndexOutOfRangeException("Argument index should be between 0 and " + CellCount);
         return owningGrid.ColumnFromDisplayIndex(index).GetCellContent(rowElement);
      }

      public UniversalCellInfo GetCurrentCellInfo()
      {
         return ConvertDataGridCellInfo(owningGrid.CurrentCell);
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
            currentColumnIndex = owningGrid.CurrentCell.Column.DisplayIndex;

         object currentItem = null;
         if (dgCellInfo.Item != DependencyProperty.UnsetValue)
            currentItem = owningGrid.CurrentCell.Item;

         return new UniversalCellInfo(currentItem, currentColumnIndex);
      }
   }
}