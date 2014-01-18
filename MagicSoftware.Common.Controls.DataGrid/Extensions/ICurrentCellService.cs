using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentCellService
   {
      FrameworkElement CurrentCell { get; }
      bool IsCellVisible { get; }
      object CurrentCellItem { get; }
      
      /// <summary>
      /// Sets the underlying element so its 'current cell' is the cell, which is 
      /// targetElement or is the container of targetElement.
      /// </summary>
      /// <param name="targetElement">The element contained in the cell or the element representing the cell itself.</param>
      /// <returns>The method returns <b>true</b> if the underlying control successfully moved its current cell pointer to the target element.
      /// Otherwise the method returns <b>false</b>.</returns>
      bool MoveTo(FrameworkElement targetElement);
      bool MoveUp(int distance);
      bool MoveDown(int distance);
      bool MoveLeft(int distance);
      bool MoveRight(int distance);
   }
}
