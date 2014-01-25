using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public struct UniversalCellInfo
   {
      public static readonly UniversalCellInfo Undefined = new UniversalCellInfo(null, -1);

      public readonly object Item;
      public readonly int CellIndex;

      public UniversalCellInfo(object item, int cellIndex)
      {
         Item = item;
         CellIndex = cellIndex;
      }
   }
}
