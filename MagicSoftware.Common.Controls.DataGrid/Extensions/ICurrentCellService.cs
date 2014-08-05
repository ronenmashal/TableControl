using System;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Interface for services that track the cell currently in focus.
   /// The implementation is expected to work solely on accessible cells - i.e. cell
   /// elements that have already been loaded. It is up to the caller to ensure the target
   /// cell is loaded by scrolling the underlying element, for example.
   /// </summary>
   internal interface ICurrentCellService
   {
      event EventHandler CurrentCellChanged;

      event EventHandler<PreviewChangeEventArgs> PreviewCurrentCellChanging;

      UniversalCellInfo CurrentCell { get; }

      FrameworkElement CurrentCellContainer { get; }

      FrameworkElement CurrentItemContainer { get; }

      bool IsCellVisible { get; }

      bool MoveDown(uint distance);

      bool MoveLeft(uint distance);

      bool MoveRight(uint distance);

      bool MoveTo(UniversalCellInfo targetCell);

      bool MoveToBottom();

      bool MoveToLeftMost();

      bool MoveToRightMost();

      bool MoveToTop();

      bool MoveUp(uint distance);
   }
}