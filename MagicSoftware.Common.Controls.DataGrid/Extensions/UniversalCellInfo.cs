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

      public override bool Equals(object obj)
      {
         if (!(obj is UniversalCellInfo))
            return false;

         UniversalCellInfo other = (UniversalCellInfo)obj;
         return object.ReferenceEquals(this.Item, other.Item) && (this.CellIndex == other.CellIndex);
      }

      public override int GetHashCode()
      {
         int hash = 27;
         if (Item != null)
            hash = hash * 47 + Item.GetHashCode();
         hash = hash * 47 + CellIndex;
         return hash;
      }
   }
}
