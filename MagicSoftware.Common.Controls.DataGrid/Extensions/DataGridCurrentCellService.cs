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
      private System.Windows.Controls.DataGrid dataGrid;

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         //currentRowService = UIServiceProvider.GetService<ICurrentItemService>(element);
         dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
         UpdateCurrentCell();
      }

      public void DetachFromElement(FrameworkElement element)
      {

      }

      void DataGrid_CurrentCellChanged(object sender, EventArgs e)
      {
         UpdateCurrentCell();
      }

      void UpdateCurrentCell()
      {
         int currentColumnIndex = -1;
         if (dataGrid.CurrentCell.Column != null)
            currentColumnIndex = dataGrid.CurrentCell.Column.DisplayIndex;

         object currentItem = null;
         if (dataGrid.CurrentCell.Item != DependencyProperty.UnsetValue)
            currentItem = dataGrid.CurrentCell.Item;

         CurrentCell = new UniversalCellInfo(currentItem, currentColumnIndex);
      }

      public UniversalCellInfo CurrentCell
      {
         get; private set;
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
         //var targetRow = UIUtils.GetAncestor<DataGridRow>(targetElement);
         //if (targetRow == null)
         //   return false;

         //if (!currentRowService.MoveCurrentTo(targetRow.Item))
         //   return false;

         //if (currentCellInRowService != null)
         //   return currentCellInRowService.MoveCurrentTo(targetElement);

         //return true;

         return false;
      }

      public bool MoveUp(int distance)
      {
         return false;
      }

      public bool MoveDown(int distance)
      {
         return false;
      }

      public bool MoveLeft(int distance)
      {
         return false;
      }

      public bool MoveRight(int distance)
      {
         return false;
      }

      #region IDisposable Members

      public void Dispose()
      {
         
      }

      #endregion
   }
}
