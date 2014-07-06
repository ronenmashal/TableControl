using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface IItemsControlTraits
   {
      void SetCurrentCell(ItemsControl control, UniversalCellInfo cellInfo);
   }

   public class DataGridTraits : IItemsControlTraits
   {
      public void SetCurrentCell(ItemsControl control, UniversalCellInfo cellInfo)
      {
         var dg = (DataGrid)control;
         dg.CurrentCell = new DataGridCellInfo(cellInfo.Item, dg.ColumnFromDisplayIndex(cellInfo.CellIndex));
      }
   }
}
