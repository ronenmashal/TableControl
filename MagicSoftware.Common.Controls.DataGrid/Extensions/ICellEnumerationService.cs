using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface ICellEnumerationService
   {
      int CellCount { get; }

      int CurrentCellIndex { get; }

      FrameworkElement GetCellAt(int index);

      UniversalCellInfo GetCurrentCellInfo();

      bool MoveToCell(int cellIndex);
   }
}