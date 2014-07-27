using System;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.Extensions;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(IElementEditStateService))]
   internal class DataGridEditStateService : IElementEditStateService, IUIService
   {
      private DataGrid dataGrid;
      private ICommandRegulationService commandRegulator;

      public bool IsAttached
      {
         get { throw new NotImplementedException(); }
      }

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         dataGrid.BeginningEdit += dataGrid_BeginningEdit;
         dataGrid.CellEditEnding += dataGrid_CellEditEnding;
         dataGrid.RowEditEnding += dataGrid_RowEditEnding;

         UIServiceProvider.AddServiceProviderFullyAttachedHandler(dataGrid, ServiceProvider_FullyAttached);
      }

      public void DetachFromElement(System.Windows.FrameworkElement element)
      {
         throw new NotImplementedException();
      }

      public void Dispose()
      {
         dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
         dataGrid.CellEditEnding -= dataGrid_CellEditEnding;
         dataGrid.RowEditEnding -= dataGrid_RowEditEnding;
         CurrentEdit = null;
         this.dataGrid = null;
      }

      private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
      {
         CurrentEdit = e.Row.Item;
         IsEditing = true;
      }

      private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
      }

      private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
      {
         CurrentEdit = null;
         IsEditing = false;
      }

      private void ServiceProvider_FullyAttached(object obj, EventArgs args)
      {
         commandRegulator = UIServiceProvider.GetService<ICommandRegulationService>(dataGrid);
         var currentItemService = UIServiceProvider.GetService<ICurrentCellService>(dataGrid);
         var cell = currentItemService.CurrentCellElement as DataGridCell;
         if (cell != null)
         {
            IsEditing = cell.IsEditing;
         }
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

      #endregion IEditingItemsControlProxy Members

      #region IElementEditStateProxy Members

      public bool BeginEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.BeginEditCommand, null);
         return IsEditing;
      }

      public bool CancelEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.CancelEditCommand, DataGridEditingUnit.Row);
         return !IsEditing;
      }

      public bool CommitEdit()
      {
         commandRegulator.ExecuteCommand(DataGrid.CommitEditCommand, DataGridEditingUnit.Row);
         return !IsEditing;
      }

      #endregion IElementEditStateProxy Members
   }
}