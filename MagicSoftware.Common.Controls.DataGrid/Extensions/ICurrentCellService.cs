using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentCellService
   {
      event EventHandler<PreviewChangeEventArgs> PreviewCurrentCellChanging;
      event EventHandler CurrentCellChanged;

      UniversalCellInfo CurrentCell { get; }
      bool IsCellVisible { get; }
      FrameworkElement CurrentCellElement { get; }
      
      bool MoveTo(UniversalCellInfo targetCell);
      bool MoveUp(uint distance);
      bool MoveDown(uint distance);
      bool MoveLeft(uint distance);
      bool MoveRight(uint distance);
   }
}
