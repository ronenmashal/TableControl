using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   static class ControlTraitsFactory
   {
      public static IItemsControlTraits GetTraitsFor(Type controlType)
      {
         // Assert controlType is DataGrid
         return new DataGridTraits();
      }
   }
}
