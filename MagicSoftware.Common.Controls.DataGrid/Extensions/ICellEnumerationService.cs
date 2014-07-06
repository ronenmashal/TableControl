using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface ICellEnumerationService
   {
      int CellCount { get; }

      int CurrentCellIndex { get; }

      object ServiceGroupIdentifier { get; }

      FrameworkElement GetCellAt(int index);

      UniversalCellInfo GetCellContaining(DependencyObject dependencyObject);

      UniversalCellInfo GetCurrentCellInfo();

      bool MoveToCell(int cellIndex);

      void UpdateCurrentCellIndex();
   }
}