using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
  class DataGridRowCurrentItemService : CurrentItemServiceBase
   {
      private DataGridRow dataGridRow;
      private DataGrid owner;

      public DataGridRowCurrentItemService(DataGridRow dataGridRow): base(dataGridRow)
      {
         this.dataGridRow = dataGridRow;
         owner = UIUtils.GetAncestor<DataGrid>(dataGridRow);
      }

      public override object CurrentItem
      {
         get
         {
            if (owner.CurrentColumn == null)
               return null;
            return owner.CurrentColumn.GetCellContent(dataGridRow);
         }
      }

      public override int CurrentPosition
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
         bool canceled;
         RaisePreviewCurrentChangingEvent(cell, out canceled);
         if (canceled)
            return CurrentItem != null;

         DataGridCellInfo newCellInfo = new DataGridCellInfo(cell);
         owner.CurrentCell = newCellInfo;

         RaiseCurrentChangedEvent();

         return CurrentItem != null;
      }

      public override bool MoveCurrentTo(object item)
      {
         var cell = UIUtils.GetAncestor<DataGridCell>(item as UIElement);
         return MoveToCell(cell);
      }

      public override bool MoveCurrentToFirst()
      {
         return MoveCurrentToPosition(0);
      }

      public override bool MoveCurrentToNext()
      {
         return MoveCurrentToRelativePosition(1);
      }

      public override bool MoveCurrentToPrevious()
      {
         return MoveCurrentToRelativePosition(-1);
      }

      public override bool MoveCurrentToLast()
      {
         return MoveCurrentToPosition(owner.Columns.Count - 1);
      }

      public override bool MoveCurrentToPosition(int position)
      {
         var targetElement = owner.ColumnFromDisplayIndex(position).GetCellContent(dataGridRow);
         return MoveCurrentTo(targetElement);
      }

      public override bool MoveCurrentToRelativePosition(int offset)
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

