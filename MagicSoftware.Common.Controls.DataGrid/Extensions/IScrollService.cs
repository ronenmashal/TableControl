using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface IVerticalScrollService
   {
      bool ScrollTo(object item);
      int ItemsPerPage { get; }
   }
}
