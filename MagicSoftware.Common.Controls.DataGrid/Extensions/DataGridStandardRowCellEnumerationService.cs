using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICellEnumerationService))]
   internal class DataGridStandardRowCellEnumerationService : CellEnumerationServiceBase
   {
      public DataGridStandardRowCellEnumerationService()
         : base("_default_")
      {
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<DataGridCell>());
      }
   }
}