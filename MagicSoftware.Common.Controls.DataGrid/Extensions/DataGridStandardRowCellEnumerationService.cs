using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICellEnumerationService))]
   internal class DataGridStandardRowCellEnumerationService : CellEnumerationServiceBase
   {
      public DataGridStandardRowCellEnumerationService()
         : base("_default_")
      {
      }

      public override int CellCount
      {
         get { return CountVisibleColumns(OwnerDataGrid); }
      }

      private DataGrid OwnerDataGrid { get { return (DataGrid)Owner; } }

      public override void AttachToElement(FrameworkElement element)
      {
         base.AttachToElement(element);
      }

      public override void DetachFromElement(FrameworkElement element)
      {
         base.DetachFromElement(element);
      }

      public override FrameworkElement GetCellAt(int index)
      {
         var column = OwnerDataGrid.ColumnFromDisplayIndex(index);
         var cellContent = column.GetCellContent(Row);
         var cell = UIUtils.GetAncestor<DataGridCell>(cellContent);
         return cell;
      }

      public override UniversalCellInfo GetCellContaining(DependencyObject dependencyObject)
      {
         var containingCell = UIUtils.GetAncestor<DataGridCell>((UIElement)dependencyObject);
         int cellIndex = -1;
         if (containingCell != null)
         {
            cellIndex = containingCell.Column.DisplayIndex;
         }
         return new UniversalCellInfo(Row.Item, cellIndex);
      }

      public override UniversalCellInfo GetCellInfo(int displayIndex)
      {
         return new UniversalCellInfo(Row.Item, displayIndex);
      }

      public override bool MoveToCell(int cellIndex)
      {
         //OwnerDataGrid.CurrentCell = new DataGridCellInfo(Row.Item, OwnerDataGrid.ColumnFromDisplayIndex(cellIndex));
         //          if (!OwnerDataGrid.CurrentCell.IsValid)
         //             return false;
         return base.MoveToCell(cellIndex);
      }

      public override int GetCellIndex(FrameworkElement cellElement)
      {
         return ((DataGridCell)cellElement).Column.DisplayIndex;
      }

      protected override FrameworkElement GetCellContaining(UIElement element)
      {
         return UIUtils.GetAncestor<DataGridCell>(element);
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<DataGridCell>());
      }

      private int CountVisibleColumns(DataGrid OwnerDataGrid)
      {
         int visibleColumns = 0;
         foreach (var column in OwnerDataGrid.Columns)
         {
            if (column.Visibility == Visibility.Visible)
               visibleColumns++;
         }
         return visibleColumns;
      }
   }
}