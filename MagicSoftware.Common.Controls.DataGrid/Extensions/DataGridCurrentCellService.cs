using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Table.CellTypes;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridCurrentCellService : ICurrentCellService, IUIService
   {
      public event CancelableRoutedEventHandler PreviewCurrentCellChanging;
      public event RoutedEventHandler CurrentCellChanged;

      private System.Windows.Controls.DataGrid dataGrid;
      private UniformCellInfo currentCell = null;

      public DataGridCurrentCellService()
      {
      }

      public void SetElement(UIElement element)
      {
         this.dataGrid = (DataGrid)element;
         currentCell = GetCurrentCell();
      }

      UniformCellInfo GetCurrentCell()
      {
         object item = dataGrid.CurrentItem;
         ICellElementLocator cellElementLocator = null;
         if (item != null)
         {
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
            Debug.Assert(row != null);
            if (EnhancedDGProxy.GetIsCustomRow(row))
            {
               cellElementLocator = new FindAncestor<VirtualTableCell>(FocusManager.GetFocusedElement(row));
            }
            else
            {
               cellElementLocator = new FindCellByColumn(dataGrid.CurrentColumn);
            }
         }
         return new UniformCellInfo(item, cellElementLocator);
      }

      public UniformCellInfo CurrentCell
      {
         get { return currentCell; }
      }

      public bool MoveTo(UniformCellInfo targetCellInfo)
      {
         throw new NotImplementedException();
      }

      public bool MoveUp(int distance)
      {
         throw new NotImplementedException();
      }

      public bool MoveDown(int distance)
      {
         throw new NotImplementedException();
      }

      public bool MoveLeft(int distance)
      {
         throw new NotImplementedException();
      }

      public bool MoveRight(int distance)
      {
         throw new NotImplementedException();
      }

      #region IDisposable Members

      public void Dispose()
      {
         throw new NotImplementedException();
      }

      #endregion
   }

   class FindAncestor<T> : ICellElementLocator
      where T : UIElement
   {
      private IInputElement referenceElement;

      public FindAncestor(IInputElement iInputElement)
      {
         this.referenceElement = iInputElement;
      }

      #region ICellElementLocator Members

      public UIElement GetCell(UIElement cellContainer)
      {
         return UIUtils.GetAncestor<T>(referenceElement as UIElement);
      }

      #endregion
   }

   class FindCellByColumn : ICellElementLocator
   {
      private DataGridColumn dataGridColumn;

      public FindCellByColumn(DataGridColumn dataGridColumn)
      {
         this.dataGridColumn = dataGridColumn;
      }
      
      #region ICellElementLocator Members

      public UIElement GetCell(UIElement cellContainer)
      {
         if (dataGridColumn == null)
            return null;

         var content = dataGridColumn.GetCellContent(cellContainer);
         return UIUtils.GetAncestor<DataGridCell>(content);
      }

      #endregion
   }
}
