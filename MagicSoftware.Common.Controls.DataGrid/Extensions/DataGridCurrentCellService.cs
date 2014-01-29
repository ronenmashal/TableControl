using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Table.CellTypes;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridCurrentCellService : ICurrentCellService, IUIService
   {
      #region CurrentCellChanging event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler<PreviewChangeEventArgs> PreviewCurrentCellChanging;

      /// <summary>
      /// Raises the PreviewCurrentCellChangingEvent, allowing the handlers to cancel it,
      /// returning the result in 'canceled'
      /// </summary>
      /// 
      /// <param name="canceled">Returns whether any of the event handlers canceled the event.</param>
      public void RaisePreviewCurrentCellChangingEvent(UniversalCellInfo newValue, out bool canceled)
      {
         canceled = false;
         if (PreviewCurrentCellChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentCell, newValue, true);
            PreviewCurrentCellChanging(this, eventArgs);
            canceled = eventArgs.Canceled;
         }
      }

      /// <summary>
      /// Raises the PreviewCurrentCellChangingEvent without allowing canceling of the event.
      /// </summary>
      public void RaiseNonCancelablePreviewCurrentCellChangingEvent(UniversalCellInfo newValue)
      {
         if (isSelfInducedCellChange.IsSet)
            return;
         if (PreviewCurrentCellChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentCell, newValue, false);
            PreviewCurrentCellChanging(this, eventArgs);
         }
      }

      #endregion

      #region CurrentCellChanged event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler CurrentCellChanged;

      public void RaiseCurrentCellChangedEvent()
      {
         if (CurrentCellChanged != null)
            CurrentCellChanged(this, new EventArgs());
      }

      #endregion


      private System.Windows.Controls.DataGrid dataGrid;

      /// <summary>
      /// Determines whether the current cell change is of an external origin, i.e. the 
      /// cell position was changed by another component; or the change is caused by
      /// this class (self induced).
      /// </summary>
      protected readonly AutoResetFlag isSelfInducedCellChange = new AutoResetFlag();

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         //currentRowService = UIServiceProvider.GetService<ICurrentItemService>(element);
         dataGrid.CurrentCellChanged += DataGrid_CurrentCellChanged;
         dataGrid.ColumnDisplayIndexChanged += DataGrid_ColumnDisplayIndexChanged;
         UpdateCurrentCell();
      }

      public void DetachFromElement(FrameworkElement element)
      {
         dataGrid.CurrentCellChanged -= DataGrid_CurrentCellChanged;
         dataGrid.ColumnDisplayIndexChanged -= DataGrid_ColumnDisplayIndexChanged;
      }

      void DataGrid_CurrentCellChanged(object sender, EventArgs e)
      {
         if (!isSelfInducedCellChange.IsSet)
            RaiseNonCancelablePreviewCurrentCellChangingEvent(ConvertDataGridCellInfo(dataGrid.CurrentCell));
         UpdateCurrentCell();
         RaiseCurrentCellChangedEvent();
      }

      void DataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
      {
         UpdateCurrentCell();
      }


      void UpdateCurrentCell()
      {
         CurrentCell = ConvertDataGridCellInfo(dataGrid.CurrentCell);
      }

      UniversalCellInfo ConvertDataGridCellInfo(DataGridCellInfo dgCellInfo)
      {
         int currentColumnIndex = -1;
         if (dgCellInfo.Column != null)
            currentColumnIndex = dataGrid.CurrentCell.Column.DisplayIndex;

         object currentItem = null;
         if (dgCellInfo.Item != DependencyProperty.UnsetValue)
            currentItem = dataGrid.CurrentCell.Item;

         return new UniversalCellInfo(currentItem, currentColumnIndex);
      }

      public UniversalCellInfo CurrentCell
      {
         get;
         private set;
      }

      public bool IsCellVisible
      {
         get
         {
            return false;
         }
      }

      public FrameworkElement CurrentCellElement
      {
         get { return null; }
      }

      public bool MoveTo(UniversalCellInfo targetCell)
      {
         bool canceled;
         RaisePreviewCurrentCellChangingEvent(targetCell, out canceled);
         if (canceled)
            return false;

         using (isSelfInducedCellChange.Set())
            dataGrid.CurrentCell = new DataGridCellInfo(targetCell.Item, dataGrid.ColumnFromDisplayIndex(targetCell.CellIndex));
         return true;
      }

      public bool MoveUp(uint distance)
      {
         return false;
      }

      public bool MoveDown(uint distance)
      {
         int newIndex = GetCurrentItemIndex() + (int)distance;
         int maxIndex = dataGrid.Items.Count - 1;
         if (newIndex > maxIndex)
            return false;

         return MoveTo(new UniversalCellInfo(dataGrid.Items[newIndex], CurrentCell.CellIndex));
      }

      public bool MoveLeft(uint distance)
      {
         return false;
      }

      public bool MoveRight(uint distance)
      {
         return false;
      }

      #region IDisposable Members

      public void Dispose()
      {

      }

      #endregion

      private int GetCurrentItemIndex()
      {
         if (CurrentCell.Item == null)
            return -1;

         return dataGrid.Items.IndexOf(CurrentCell.Item);
      }
   }
}
