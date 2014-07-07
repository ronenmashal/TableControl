using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface ICellEnumerationService
   {
      int CellCount { get; }

      object ServiceGroupIdentifier { get; }

      FrameworkElement GetCellAt(int index);

      UniversalCellInfo GetCellContaining(DependencyObject dependencyObject);

      UniversalCellInfo GetCellInfo(int displayIndex);

      bool MoveToCell(int cellIndex);

      int GetCellIndex(FrameworkElement cellElement);
   }
}