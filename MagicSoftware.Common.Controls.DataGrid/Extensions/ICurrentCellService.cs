using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentCellService
   {
      event CancelableRoutedEventHandler PreviewCurrentCellChanging;
      event RoutedEventHandler CurrentCellChanged;

      UniformCellInfo CurrentCell { get; }
      
      bool MoveTo(UniformCellInfo targetCellInfo);
      bool MoveUp(int distance);
      bool MoveDown(int distance);
      bool MoveLeft(int distance);
      bool MoveRight(int distance);
   }
}
