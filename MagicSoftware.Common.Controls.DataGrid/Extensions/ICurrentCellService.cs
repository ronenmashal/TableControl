using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Interface for services that track the cell currently in focus.
   /// The implementation is expected to work solely on accessible cells - i.e. cell 
   /// elements that have already been loaded. It is up to the caller to ensure the target
   /// cell is loaded by scrolling the underlying element, for example.
   /// </summary>
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
