using System;
using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(IElementEditStateService))]
   internal class DataGridEditStateService : IElementEditStateService, IUIService
   {
      private ICommandRegulationService commandRegulator;
      private DataGrid dataGrid;

      public bool IsAttached
      {
         get { return dataGrid != null; }
      }

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         dataGrid.BeginningEdit += dataGrid_BeginningEdit;
         dataGrid.CellEditEnding += dataGrid_CellEditEnding;
         dataGrid.RowEditEnding += dataGrid_RowEditEnding;
         dataGrid.PreparingCellForEdit += dataGrid_PreparingCellForEdit;

         UIServiceProvider.AddServiceProviderFullyAttachedHandler(dataGrid, ServiceProvider_FullyAttached);
      }

      public void DetachFromElement(System.Windows.FrameworkElement element)
      {
         dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
         dataGrid.CellEditEnding -= dataGrid_CellEditEnding;
         dataGrid.RowEditEnding -= dataGrid_RowEditEnding;
         CurrentEdit = null;
         this.dataGrid = null;
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(dataGrid);
      }

      private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
      {
         CurrentEdit = e.Row.Item;
      }

      private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
      }

      private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
      {
      }

      private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
      {
         CurrentEdit = null;
      }

      private void ServiceProvider_FullyAttached(object obj, EventArgs args)
      {
         commandRegulator = UIServiceProvider.GetService<ICommandRegulationService>(dataGrid);
      }

      #region IEditingItemsControlProxy Members

      public object CurrentEdit { get; private set; }

      public bool IsEditingField
      {
         get
         {
            var currentItemService = UIServiceProvider.GetService<ICurrentCellService>(dataGrid);
            var cell = currentItemService.CurrentCellContainer as DataGridCell;
            if (cell != null)
            {
               return cell.IsEditing;
            }

            return false;
         }
      }

      public bool IsEditingItem
      {
         get
         {
            var currentItemService = UIServiceProvider.GetService<ICurrentCellService>(dataGrid);
            var row = (DataGridRow)currentItemService.CurrentItemContainer;
            if (row == null)
               return false;

            return row.IsEditing;
         }
      }

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

      #endregion IEditingItemsControlProxy Members

      #region IElementEditStateProxy Members

      public bool BeginFieldEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.BeginEditCommand, DataGridEditingUnit.Cell);
         return IsEditingField;
      }

      public bool BeginItemEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.BeginEditCommand, null);
         return IsEditingField;
      }

      public bool CancelFieldEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.CancelEditCommand, DataGridEditingUnit.Cell);
         return !IsEditingField;
      }

      public bool CancelItemEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.CancelEditCommand, DataGridEditingUnit.Row);
         return !IsEditingField;
      }

      public bool CommitFieldEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.CommitEditCommand, DataGridEditingUnit.Cell);
         return !IsEditingField;
      }

      public bool CommitItemEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.CommitEditCommand, DataGridEditingUnit.Row);
         return !IsEditingField;
      }

      #endregion IElementEditStateProxy Members
   }
}