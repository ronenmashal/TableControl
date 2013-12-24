using System;
using MagicSoftware.Common.Controls.Proxies;
using MagicSoftware.Common.Controls.Table.Extensions;
using System.Windows.Controls;

class DataGridAsEditingAdapter : IElementEditStateProxy
{
   EnhancedDGProxy dataGridProxy;
   DataGrid dataGrid;

   public DataGridAsEditingAdapter(DataGrid dataGrid, EnhancedDGProxy dataGridProxy)
   {
      this.dataGrid = dataGrid;
      dataGrid.BeginningEdit += dataGrid_BeginningEdit;
      dataGrid.CellEditEnding += dataGrid_CellEditEnding;
      dataGrid.RowEditEnding += dataGrid_RowEditEnding;
      this.dataGridProxy = dataGridProxy;
      var row = dataGridProxy.SelectedItemContainer() as DataGridRow;
      if (row != null)
      {
         IsEditing = row.IsEditing;
      }
   }

   public void Dispose()
   {
      dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
      dataGrid.CellEditEnding -= dataGrid_CellEditEnding;
      dataGrid.RowEditEnding -= dataGrid_RowEditEnding;
      this.dataGrid = null;
   }

   void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
   {
      CurrentEdit = null;
      IsEditing = false;
   }

   void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
   {
      CurrentEdit = null;
      IsEditing = false;
   }

   void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
   {
      CurrentEdit = e.Row.Item;
      IsEditing = true;
   }

   #region IEditingItemsControlProxy Members

   public object CurrentEdit { get; private set; }
   public bool IsEditing { get; private set; }

   public void NotifyBeginEdit()
   {
      throw new NotImplementedException();
   }

   public void NotifyCancelEdit()
   {
      throw new NotImplementedException();
   }

   public void NotifyCommitEdit()
   {
      throw new NotImplementedException();
   }

   #endregion

   #region IElementEditStateProxy Members

   public bool BeginEdit()
   {
      dataGridProxy.ExecuteCommand(DataGrid.BeginEditCommand, null);
      return IsEditing;
   }

   public bool CommitEdit()
   {
      dataGridProxy.ExecuteCommand(DataGrid.CommitEditCommand, null);
      return !IsEditing;
   }

   public bool CancelEdit()
   {
      dataGridProxy.ExecuteCommand(DataGrid.CancelEditCommand, null);
      return !IsEditing;
   }

   #endregion
}

