using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MagicSoftware.Common.Utils;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Implementation of ICurrentCellService for DataGrid. The implementation operates only
   /// on accessible cells - i.e. cells that have already been loaded. It cannot move to a cell
   /// placed on an item that was not loaded (due to virtualization, for example).
   /// </summary>
   [ImplementedService(typeof(ICurrentCellService))]
   internal class DataGridCurrentCellService : ICurrentCellService, IUIService
   {
      #region CurrentCellChanging event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler<PreviewChangeEventArgs> PreviewCurrentCellChanging;

      /// <summary>
      /// Raises the PreviewCurrentCellChangingEvent without allowing canceling of the event.
      /// </summary>
      public void RaiseNonCancelablePreviewCurrentCellChangingEvent(UniversalCellInfo newValue)
      {
         if (PreviewCurrentCellChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentCell, newValue, false);
            PreviewCurrentCellChanging(this, eventArgs);
         }
      }

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

      #endregion CurrentCellChanging event.

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

      #endregion CurrentCellChanged event.

      /// <summary>
      /// Determines whether the current cell change is of an external origin, i.e. the
      /// cell position was changed by another component; or the change is caused by
      /// this class (self induced).
      /// </summary>
      protected readonly AutoResetFlag isSelfInducedCellChange = new AutoResetFlag();

      private System.Windows.Controls.DataGrid dataGrid;

      public UniversalCellInfo CurrentCell
      {
         get;
         private set;
      }

      public FrameworkElement CurrentCellElement
      {
         get { return null; }
      }

      public bool IsCellVisible
      {
         get
         {
            return false;
         }
      }

      private ICellEnumerationService CurrentRowCellEnumerationService { get; set; }

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

      public bool MoveDown(uint distance)
      {
         int newIndex = IndexOf(CurrentCell.Item) + (int)distance;
         int maxIndex = dataGrid.Items.Count - 1;
         if (newIndex > maxIndex)
            return false;

         return MoveTo(new UniversalCellInfo(dataGrid.Items[newIndex], CurrentCell.CellIndex));
      }

      public bool MoveLeft(uint distance)
      {
         int newCellIndex = CurrentCell.CellIndex - (int)distance;
         if (newCellIndex >= 0)
            return MoveTo(new UniversalCellInfo(CurrentCell.Item, newCellIndex));
         else
            return false;
      }

      public bool MoveRight(uint distance)
      {
         int newCellIndex = CurrentCell.CellIndex + (int)distance;
         var rowEnumSvc = GetRowEnumerationServiceForItem(CurrentCell.Item);
         if (newCellIndex < rowEnumSvc.CellCount)
            return MoveTo(new UniversalCellInfo(CurrentCell.Item, newCellIndex));
         else
            return false;
      }

      public bool MoveTo(UniversalCellInfo targetCell)
      {
         bool canceled;

         UpdateCurrentCell();

         if (!CanMoveTo(targetCell))
            return false;

         RaisePreviewCurrentCellChangingEvent(targetCell, out canceled);
         if (canceled)
            return false;

         using (isSelfInducedCellChange.Set())
         {
            if (targetCell.CellIndex >= dataGrid.Columns.Count)
               return false;
            dataGrid.CurrentCell = new DataGridCellInfo(targetCell.Item, dataGrid.ColumnFromDisplayIndex(targetCell.CellIndex));
         }

         return CurrentCell.Equals(targetCell);
      }

      public bool MoveToBottom()
      {
         var lastItem = dataGrid.Items[dataGrid.Items.Count - 1];
         if (object.ReferenceEquals(CurrentCell.Item, lastItem))
            return true;
         return MoveTo(new UniversalCellInfo(lastItem, dataGrid.CurrentColumn.DisplayIndex));
      }

      public bool MoveToLeftMost()
      {
         return MoveTo(new UniversalCellInfo(CurrentCell.Item, 0));
      }

      public bool MoveToRightMost()
      {
         return MoveTo(new UniversalCellInfo(CurrentCell.Item, CurrentRowCellEnumerationService.CellCount - 1));
      }

      public bool MoveToTop()
      {
         var firstItem = dataGrid.Items[0];
         if (object.ReferenceEquals(CurrentCell.Item, firstItem))
            return true;
         return MoveTo(new UniversalCellInfo(firstItem, dataGrid.CurrentColumn.DisplayIndex));
      }

      public bool MoveUp(uint distance)
      {
         int newIndex = IndexOf(CurrentCell.Item) - (int)distance;
         if (newIndex < 0)
            return false;

         return MoveTo(new UniversalCellInfo(dataGrid.Items[newIndex], CurrentCell.CellIndex));
      }

      private bool CanMoveTo(UniversalCellInfo targetCell)
      {
         if (IndexOf(targetCell.Item) < 0)
            return false;

         var targetRowCellEnumerationService = GetRowEnumerationServiceForItem(targetCell.Item);
         if (targetRowCellEnumerationService == null)
            return false;

         if (targetCell.CellIndex < 0 || targetCell.CellIndex >= targetRowCellEnumerationService.CellCount)
            return false;

         return true;
      }

      private void DataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
      {
         UpdateCurrentCell();
      }

      private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
      {
         if (!isSelfInducedCellChange.IsSet)
            RaiseNonCancelablePreviewCurrentCellChangingEvent(CurrentCell);
         UpdateCurrentCell();
         RaiseCurrentCellChangedEvent();
      }

      private UIElement ForceContainerGeneration(object item)
      {
         int itemIndex = IndexOf(item);
         UIElement container = null;
         bool isNewlyRealized = false;
         IItemContainerGenerator generator = dataGrid.ItemContainerGenerator;
         GeneratorPosition gp = generator.GeneratorPositionFromIndex(itemIndex);
         using (generator.StartAt(gp, GeneratorDirection.Forward, true))
         {
            isNewlyRealized = false;
            container = generator.GenerateNext(out isNewlyRealized) as UIElement;
         }
         return container;
      }

      private FrameworkElement CurrentItemContainer
      {
         get { return dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.CurrentItem) as FrameworkElement; }
      }

      private ICellEnumerationService GetRowEnumerationServiceForItem(object item)
      {
         var currentRow = CurrentItemContainer as DataGridRow;
         if (currentRow == null)
            return null;

         var service = UIServiceProvider.GetService<ICellEnumerationService>(currentRow);
         //((IUIService)service).AttachToElement(currentRow);

         return service;
      }

      private int IndexOf(object item)
      {
         if (item == null)
            return -1;

         return dataGrid.Items.IndexOf(item);
      }

      private void UpdateCurrentCell()
      {
         CurrentRowCellEnumerationService = GetRowEnumerationServiceForItem(dataGrid.CurrentItem);
         if (CurrentRowCellEnumerationService != null)
            CurrentCell = CurrentRowCellEnumerationService.GetCurrentCellInfo();
      }

      #region IDisposable Members

      public void Dispose()
      {
      }

      #endregion IDisposable Members
   }

   internal class EmptyRowCellEnumerationService : ICellEnumerationService, IUIService
   {
      #region IUIService Members

      public void AttachToElement(FrameworkElement element)
      {
      }

      public void DetachFromElement(FrameworkElement element)
      {
      }

      #endregion IUIService Members

      #region IDisposable Members

      public void Dispose()
      {
      }

      #endregion IDisposable Members

      #region ICellEnumerationService Members

      public int CellCount
      {
         get { return 0; }
      }

      public FrameworkElement GetCellAt(int index)
      {
         throw new ArgumentOutOfRangeException("An empty row has no cells in it.");
      }

      #endregion ICellEnumerationService Members

      public int CurrentCellIndex
      {
         get { throw new NotImplementedException(); }
      }

      public UniversalCellInfo GetCurrentCellInfo()
      {
         throw new NotImplementedException();
      }
   }
}