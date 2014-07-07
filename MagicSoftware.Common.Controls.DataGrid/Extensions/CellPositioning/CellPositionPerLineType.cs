using System;
using System.Collections.Generic;

namespace MagicSoftware.Common.Controls.Table.Extensions.CellPositioning
{
   internal interface ICurrentCellPosition
   {
      int GetCurrentCellIndex(ICellEnumerationService cellContainerService);

      void SetCurrentCellIndex(ICellEnumerationService cellContainerService, int newIndex);
   }

   internal class CellPositionPerLineType : ICurrentCellPosition
   {
      private Dictionary<object, int> currentCellIndexTable = new Dictionary<object, int>();

      private Func<ICellEnumerationService, object> getKey;

      public CellPositionPerLineType(Func<ICellEnumerationService, object> getKey)
      {
         this.getKey = getKey;
      }

      public int GetCurrentCellIndex(ICellEnumerationService cellContainerService)
      {
         object key = getKey(cellContainerService);
         EnsureKeyExists(key);
         return currentCellIndexTable[key];
      }

      public void SetCurrentCellIndex(ICellEnumerationService cellContainerService, int newIndex)
      {
         object key = getKey(cellContainerService);
         EnsureKeyExists(key);
         currentCellIndexTable[key] = newIndex;
      }

      private void EnsureKeyExists(object key)
      {
         if (!currentCellIndexTable.ContainsKey(key))
         {
            currentCellIndexTable.Add(key, 0);
         }
      }
   }
}