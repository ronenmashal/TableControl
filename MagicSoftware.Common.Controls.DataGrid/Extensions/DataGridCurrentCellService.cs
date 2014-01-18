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

      ICurrentItemService currentRowService;
      ICurrentItemService currentCellInRowService;

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         currentRowService = UIServiceProvider.GetService<ICurrentItemService>(element);
         UpdateCurrentCell();
         
      }

      void CurrentRowService_CurrentChanged(object sender, RoutedEventArgs e)
      {
         UpdateCurrentCell();
      }

      void UpdateCurrentCell()
      {
         DataGridRow row = null;
         object item = currentRowService.CurrentItem;
         if (item != null)
         {
            row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
         }
         if (row != null)
            currentCellInRowService = UIServiceProvider.GetService<ICurrentItemService>(row);
         else
            currentCellInRowService = null;
      }

      public FrameworkElement CurrentCell
      {
         get
         {
            if (currentCellInRowService == null)
               return null;
            return (FrameworkElement)currentCellInRowService.CurrentItem;
         }
      }

      public bool IsCellVisible
      {
         get
         {
            if (CurrentCell == null)
               return false;

            return CurrentCell.IsVisible;
         }
      }

      public object CurrentCellItem
      {
         get { return currentRowService.CurrentItem; }
      }

      public bool MoveTo(FrameworkElement targetElement)
      {
         var targetRow = UIUtils.GetAncestor<DataGridRow>(targetElement);
         if (targetRow == null)
            return false;

         if (!currentRowService.MoveCurrentTo(targetRow.Item))
            return false;

         if (currentCellInRowService != null)
            return currentCellInRowService.MoveCurrentTo(targetElement);

         return true;
      }

      public bool MoveUp(int distance)
      {
         return currentRowService.MoveCurrentToRelativePosition(-distance);
      }

      public bool MoveDown(int distance)
      {
         return currentRowService.MoveCurrentToRelativePosition(distance);
      }

      public bool MoveLeft(int distance)
      {
         if (currentCellInRowService == null)
            return false;

         return currentCellInRowService.MoveCurrentToRelativePosition(-distance);
      }

      public bool MoveRight(int distance)
      {
         if (currentCellInRowService == null)
            return false;

         return currentCellInRowService.MoveCurrentToRelativePosition(distance);
      }

      #region IDisposable Members

      public void Dispose()
      {
         throw new NotImplementedException();
      }

      #endregion

      public IDisposable ConsolidateChangeEvents()
      {
         return new ConsolidatedEventHandling();
      }

      class ConsolidatedEventHandling : IDisposable
      {

         #region IDisposable Members

         public void Dispose()
         {
            throw new NotImplementedException();
         }

         #endregion
      }

   }
}
