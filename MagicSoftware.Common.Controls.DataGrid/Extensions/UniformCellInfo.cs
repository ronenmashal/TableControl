using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICellElementLocator
   {
      UIElement GetCell(UIElement cellContainer);
   }

   class UniformCellInfo
   {
      public object Item { get; private set; }
      public ICellElementLocator CellElementLocator { get; private set; }

      public UniformCellInfo(object item, ICellElementLocator cellElementLocator)
      {
         Item = item;
         CellElementLocator = cellElementLocator;
      }

      public override string ToString()
      {
         return "{CellInfo: Item " + Item + ", locator " + CellElementLocator + "}";
      }
   }
}
