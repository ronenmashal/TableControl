using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface ICollectionViewMoveAction
   {
      bool Move(ICollectionView itemsView);
   }

   public class MoveCurrentToPositionAction : ICollectionViewMoveAction
   {
      public int NewPosition { get; set; }

      public bool Move(ICollectionView itemsView)
      {
         return itemsView.MoveCurrentToPosition(NewPosition);
      }
   }

   public class MoveCurrentToRelativePosition : ICollectionViewMoveAction
   {
      public int PositionOffset { get; set; }
      public int UpperBoundary { get; set; }
      public int LowerBoundary { get; set; }

      public MoveCurrentToRelativePosition()
      {
         UpperBoundary = int.MaxValue;
         LowerBoundary = 0;
      }

      public bool Move(ICollectionView itemsView)
      {
         int newPosition = itemsView.CurrentPosition + PositionOffset;
         if (newPosition < 0)
            newPosition = 0;
         if (newPosition > UpperBoundary)
            newPosition = UpperBoundary;
         return itemsView.MoveCurrentToPosition(newPosition);
      }
   }

   public class MoveCurrentToItemAction : ICollectionViewMoveAction
   {
      public object Item { get; set; }

      public bool Move(ICollectionView itemsView)
      {
         return itemsView.MoveCurrentTo(Item);
      }
   }
}
