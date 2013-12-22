using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table
{
   public class VirtualTextTableCell : VirtualTableCell
   {
      static VirtualTextTableCell()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualTextTableCell), new FrameworkPropertyMetadata(typeof(VirtualTextTableCell)));
      }

   }
}
