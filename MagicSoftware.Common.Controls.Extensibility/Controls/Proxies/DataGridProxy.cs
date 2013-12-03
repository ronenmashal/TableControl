using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel.Composition;

using DataGrid = System.Windows.Controls.DataGrid;

namespace MagicSoftware.Common.Controls.ProxiesX
{
   [Export(typeof(ElementProxy))]
   public class DataGridProxy : SelectorProxy
   {

      private System.Windows.Controls.DataGrid DataGrid { get { return (System.Windows.Controls.DataGrid)Element; } }

      #region DataGrid State Properties

      public bool IsInEdit { get; private set; }
      public bool IsReadOnly
      {
         get
         {
            return DataGrid.IsReadOnly;
         }
      }

      public DataGridRow CurrentRow
      {
         get
         {
            return UIUtils.GetAncestor<DataGridRow>(Keyboard.FocusedElement as UIElement);
         }
      }

      #endregion

      public DataGridProxy(System.Windows.Controls.DataGrid dg) :
         base(dg)
      {
         dg.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(dg_BeginningEdit);
         dg.RowEditEnding += new EventHandler<DataGridRowEditEndingEventArgs>(dg_RowEditEnding);
         dg.LoadingRow += new EventHandler<DataGridRowEventArgs>(dg_LoadingRow);
         dg.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(dg_PreparingCellForEdit);

      }

      void dg_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
      {
         Trace.WriteLine("Preparing cell for edit: " + e.Row.Item);
      }

      void dg_LoadingRow(object sender, DataGridRowEventArgs e)
      {
         Trace.WriteLine("Loading row " + e.Row.Item + " style " + e.Row.Style);
      }

      public IDisposable PreserveEditState()
      {
         return new EditStatePreserver(this);
      }

      protected override void DetachFromElement()
      {
         DataGrid.BeginningEdit -= new EventHandler<DataGridBeginningEditEventArgs>(dg_BeginningEdit);
         DataGrid.RowEditEnding -= new EventHandler<DataGridRowEditEndingEventArgs>(dg_RowEditEnding);
      }


      void dg_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
      {
         IsInEdit = false;
      }

      void dg_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
      {
         IsInEdit = true;
      }


      internal bool CommitEdit(DataGridEditingUnit dataGridEditingUnit, bool exitEditingMode)
      {
         return DataGrid.CommitEdit(dataGridEditingUnit, exitEditingMode);
      }

      internal bool BeginEdit()
      {
         return DataGrid.BeginEdit();
      }

      protected override void ScrollIntoView(object item)
      {
         DataGrid.ScrollIntoView(item);
      }

      public bool MoveToCell(FocusNavigationDirection direction)
      {
         if (CurrentRow == null)
            return false;

         using (PreserveEditState())
         {
            int currentColumnIndex = -1;

            if (DataGrid.CurrentCell != null)
            {
               currentColumnIndex = DataGrid.CurrentCell.Column.DisplayIndex;

               if (!CommitEdit(DataGridEditingUnit.Cell, true))
                  return false;
            }

            switch (direction)
            {
               case FocusNavigationDirection.First:
                  currentColumnIndex = 0;
                  break;

               case FocusNavigationDirection.Last:
                  currentColumnIndex = DataGrid.Columns.Count - 1;
                  break;

               case FocusNavigationDirection.Right:
                  currentColumnIndex = (currentColumnIndex + 1) % DataGrid.Columns.Count;
                  break;

               case FocusNavigationDirection.Left:
                  currentColumnIndex = (currentColumnIndex - 1 + DataGrid.Columns.Count) % DataGrid.Columns.Count;
                  break;

               default:
                  throw new ArgumentException("Argument must specify an absolute horizontal direction: First, Last, Right or Left", "direction");
            }

            DataGridColumn nextColumn = DataGrid.Columns.First((c) => { return c.DisplayIndex == currentColumnIndex; });
            DataGrid.CurrentColumn = nextColumn;
         }

         return true;
      }

      public override bool MoveCurrentToNext()
      {
         bool moved = base.MoveCurrentToNext();
         if (moved)
            DataGrid.CurrentItem = DataGrid.SelectedItem;
         return moved;
      }

      public override bool MoveCurrentToPrevious()
      {
         bool moved = base.MoveCurrentToPrevious();
         if (moved)
            DataGrid.CurrentItem = DataGrid.SelectedItem;
         return moved;
      }


      public object CurrentItem
      {
         get
         {
            return DataGrid.CurrentItem;
         }
         set
         {
            DataGrid.CurrentItem = value;
         }
      }

      private class EditStatePreserver : IDisposable
      {
         DataGridProxy proxy;
         bool wasInEdit;

         public EditStatePreserver(DataGridProxy proxy)
         {
            this.proxy = proxy;
            wasInEdit = proxy.IsInEdit;
         }

         #region IDisposable Members

         public void Dispose()
         {
            if (wasInEdit && !proxy.IsInEdit)
               proxy.BeginEdit();
         }

         #endregion
      }
   }
}
