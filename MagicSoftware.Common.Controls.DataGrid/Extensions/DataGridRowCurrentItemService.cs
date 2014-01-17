using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridRowCurrentItemService : CurrentItemServiceBase
   {
      private DataGridRow dataGridRow;
      private DataGrid owner;
      private int currentPosition = -1;
      private DataGridCellInfo currentCellInfo;

      public DataGridRowCurrentItemService()
      {
         currentCellInfo = new DataGridCellInfo();
      }

      public DataGridRowCurrentItemService(DataGridRow dataGridRow)
         : base(dataGridRow)
      {
         SetElement(dataGridRow);
      }

      public override void SetElement(FrameworkElement element)
      {
         base.SetElement(element);
         this.dataGridRow = (DataGridRow)element;
         owner = UIUtils.GetAncestor<DataGrid>(dataGridRow);
         UpdateCurrentStateInfo();
         //TODO: CurrentCellChanged???
         owner.CurrentCellChanged += new System.EventHandler<System.EventArgs>(owner_CurrentCellChanged);
         if (owner.CurrentColumn == null)
         {
            owner.CurrentColumn = owner.ColumnFromDisplayIndex(0);
         }
      }

      void owner_CurrentCellChanged(object sender, System.EventArgs e)
      {
         UpdateCurrentStateInfo();
      }

      void UpdateCurrentStateInfo()
      {
         //TODO: CurrentCellChanged - should update when item is changed?
         int newPosition = -1;
         if (owner.CurrentColumn != null)
         {
            newPosition = owner.CurrentColumn.DisplayIndex;
         }
         if (newPosition != currentPosition)
         {
            currentPosition = newPosition;
            currentCellInfo = owner.CurrentCell;
         }
      }

      public override object CurrentItem
      {
         get
         {
            if (currentCellInfo.Column == null || object.ReferenceEquals(currentCellInfo.Item, dataGridRow.Item))
            {
               return null;
            }
            return UIUtils.GetAncestor<DataGridCell>(currentCellInfo.Column.GetCellContent(dataGridRow));
         }
      }

      public override int CurrentPosition
      {
         get { return currentPosition; }
      }

      bool MoveToCell(DataGridCell cell)
      {
         DataGridCellInfo newCellInfo = new DataGridCellInfo(cell);
         IAdaptedCellInfo newAdaptedCellInfo = DataGridAdaptedCellInfo.FromDataGridCellInfo(newCellInfo);
         bool canceled;
         RaisePreviewCurrentChangingEvent(newAdaptedCellInfo, out canceled);
         if (canceled)
            return CurrentItem != null;

         owner.CurrentCell = newCellInfo;
         owner.ScrollIntoView(owner.CurrentCell.Item);

         owner.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => RaiseCurrentChangedEvent()));

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

   interface IAdaptedCellInfo
   {
      object Item { get; }
      object Column { get; }
      int DisplayIndex { get; }
      int ItemIndex { get; }
   }

   class DataGridAdaptedCellInfo : IAdaptedCellInfo
   {
      #region IAdaptedCellInfo Members

      public object Item { get; private set; }
      public object Column { get; private set; }
      public int DisplayIndex { get; private set; }
      public int ItemIndex { get; private set; }

      #endregion

      public static IAdaptedCellInfo FromDataGridCell(DataGridCell cell)
      {
         return FromDataGridCellInfo(new DataGridCellInfo(cell));
      }

      public static IAdaptedCellInfo FromDataGridCellInfo(DataGridCellInfo cellInfo)
      {
         DataGridAdaptedCellInfo adaptedCellInfo = new DataGridAdaptedCellInfo();
         adaptedCellInfo.Item = cellInfo.Item;
         adaptedCellInfo.DisplayIndex = cellInfo.Column.DisplayIndex;
         adaptedCellInfo.Column = cellInfo.Column;
         return adaptedCellInfo;
      }
   }

}

