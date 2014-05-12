using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Utils
{
   public class DataGridRowFormatter : ICustomFormatter
   {
      public string Format(string format, object arg, IFormatProvider formatProvider)
      {
         var row = arg as DataGridRow;
         return String.Format("Row #{0} of item {{{1}}}", row.GetHashCode(), row.Item);
      }
   }
}
