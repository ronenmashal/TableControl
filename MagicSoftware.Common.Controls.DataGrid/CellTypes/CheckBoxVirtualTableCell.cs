using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.CellTypes
{
   public class CheckBoxVirtualTableCell : VirtualTableCell
   {
      static CheckBoxVirtualTableCell()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxVirtualTableCell), new FrameworkPropertyMetadata(typeof(CheckBoxVirtualTableCell)));
      }
   }
}
