using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface ICellEnumerationService
   {
      int CellCount { get; }

      FrameworkElement GetCellAt(int index);

      int CurrentCellIndex { get; }

      UniversalCellInfo GetCurrentCellInfo();
   }
}