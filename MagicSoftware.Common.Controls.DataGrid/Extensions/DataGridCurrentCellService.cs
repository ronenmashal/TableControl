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
      public event CancelableRoutedEventHandler PreviewCurrentCellChanging;
      public event RoutedEventHandler CurrentCellChanged;

      private System.Windows.Controls.DataGrid dataGrid;

      ICurrentItemService currentRowService;
      ICurrentItemService currentCellInRowService;

      public void SetElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         currentRowService = UIServiceProvider.GetService<ICurrentItemService>(element);
         UpdateCurrentCell();
         currentRowService.CurrentChanged += CurrentRowService_CurrentChanged;
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
         throw new NotImplementedException();
      }

      public bool MoveUp(int distance)
      {
         throw new NotImplementedException();
      }

      public bool MoveDown(int distance)
      {
         throw new NotImplementedException();
      }

      public bool MoveLeft(int distance)
      {
         throw new NotImplementedException();
      }

      public bool MoveRight(int distance)
      {
         throw new NotImplementedException();
      }

      #region IDisposable Members

      public void Dispose()
      {
         throw new NotImplementedException();
      }

      #endregion

   }
}
