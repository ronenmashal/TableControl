using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Proxies;
using MagicSoftware.Common.Controls.Table.CellTypes;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class CustomRowCurrentItemService : CurrentItemServiceBase
   {
      const string CurrentPositionIdentifier = "CustomRow.CurrentPosition";

      List<VirtualTableCell> cells;

      public CustomRowCurrentItemService(DataGridRow row)
         : base(row)
      {
         // Enumerate the virtual table cells in the row.
         cells = UIUtils.GetVisualChildren<VirtualTableCell>(row, (cell) => true);

         if (!sharedObjectsService.HasSharedObject(CurrentPositionIdentifier))
         {
            if (cells.Count > 0)
               SetCurrentPosition(0);
            else
               SetCurrentPosition(-1);
         }
      }

      public override object CurrentItem
      {
         get { return GetItemAt(CurrentPosition); }
      }

      public override int CurrentPosition
      {
         get { return (int)sharedObjectsService.GetSharedObject(CurrentPositionIdentifier); }
      }

      void SetCurrentPosition(int value)
      {
         sharedObjectsService.SetSharedObject(CurrentPositionIdentifier, value);
      }

      private VirtualTableCell GetItemAt(int position)
      {
         if (position < 0 || position >= cells.Count)
            return null;
         return cells[position];
      }

      public override bool MoveCurrentTo(object item)
      {
         if (item == null)
            return false;

         var cell = UIUtils.GetAncestor<VirtualTableCell>(item as UIElement);
         if (cell == null)
            return false;

         int cellIndex = cells.IndexOf(cell);
         return MoveCurrentToPosition(cellIndex);
      }

      public override bool MoveCurrentToFirst()
      {
         return MoveCurrentToPosition(0);
      }

      public override bool MoveCurrentToNext()
      {
         return MoveCurrentToRelativePosition(+1);
      }

      public override bool MoveCurrentToPrevious()
      {
         return MoveCurrentToRelativePosition(-1);
      }

      public override bool MoveCurrentToLast()
      {
         return MoveCurrentToPosition(cells.Count - 1);
      }

      public override bool MoveCurrentToPosition(int position)
      {
         if (position == CurrentPosition)
            return CurrentItem != null;

         object newItem = GetItemAt(position);
         // Raise the preview changing event.
         bool canceled;
         RaisePreviewCurrentChangingEvent(newItem, out canceled);
         if (canceled)
            return CurrentItem != null;

         SetCurrentPosition(position);

         RaiseCurrentChangedEvent();

         return CurrentItem != null;
      }

      public override bool MoveCurrentToRelativePosition(int offset)
      {
         return MoveCurrentToPosition(CurrentPosition + offset);
      }
   }
}

