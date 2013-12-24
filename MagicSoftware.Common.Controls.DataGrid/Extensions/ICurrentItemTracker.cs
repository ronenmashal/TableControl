using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentItemProvider : INotifyPropertyChanged
   {
      object CurrentItem { get; }
      int CurrentItemIndex { get; }
   }
}
