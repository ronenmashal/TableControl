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
         : base("_default_", new DataGridTraits())
      {
      }

      private DataGrid OwnerDataGrid { get { return (DataGrid)Owner; } }

      public override void AttachToElement(FrameworkElement element)
      {
         base.AttachToElement(element);
      }

      public override bool MoveToCell(int cellIndex)
      {
         OwnerDataGrid.CurrentCell = new DataGridCellInfo(Row.Item, OwnerDataGrid.ColumnFromDisplayIndex(cellIndex));
         if (!OwnerDataGrid.CurrentCell.IsValid)
            return false;
         return base.MoveToCell(cellIndex);
      }

      public override void UpdateCurrentCellIndex()
      {
         CurrentCellIndex = OwnerDataGrid.CurrentCell.Column.DisplayIndex;
      }

      protected override FrameworkElement GetCellContaining(UIElement element)
      {
         return UIUtils.GetAncestor<DataGridCell>(element);
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<DataGridCell>());
      }
   }
}