using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentCellService
   {
      UniversalCellInfo CurrentCell { get; }
      bool IsCellVisible { get; }
      FrameworkElement CurrentCellElement { get; }
      
      bool MoveTo(UniversalCellInfo targetCell);
      bool MoveUp(int distance);
      bool MoveDown(int distance);
      bool MoveLeft(int distance);
      bool MoveRight(int distance);
   }
}
