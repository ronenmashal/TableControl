using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface IMoveAction
   {
      bool ExecuteOn(object actionOperand);
   }

   public interface IMoveActionsFactory
   {
      IMoveAction GetMoveCurrentToAction(object targetItem);
      IMoveAction GetMoveCurrentToPositionAction(int position);
      IMoveAction GetMoveCurrentToRelativePositionAction(int offset, int lowerBoundary = -1, int upperBoundary = int.MaxValue);
      IMoveAction GetMoveToFirstItemAction();
      IMoveAction GetMoveToLastItemAction();
      //IMoveAction GetMoveToNextAction();
      //IMoveAction GetMoveToPreviosAction();
   }

   public abstract class MoveActionBase<T> : IMoveAction
   {
      public abstract bool Move(T view);

      #region IMoveAction Members

      bool IMoveAction.ExecuteOn(object actionOperand)
      {
         return Move((T)actionOperand);
      }

      #endregion
   }

   public class CollectionViewMoveCurrentToPositionAction : MoveActionBase<ICollectionView>
   {
      public int NewPosition { get; set; }

      public override bool Move(ICollectionView itemsView)
      {
         return itemsView.MoveCurrentToPosition(NewPosition);
      }
   }

   public class CollectionViewMoveCurrentToRelativePositionAction : MoveActionBase<ICollectionView>
   {
      public int PositionOffset { get; set; }
      public int UpperBoundary { get; set; }
      public int LowerBoundary { get; set; }

      public CollectionViewMoveCurrentToRelativePositionAction()
      {
         UpperBoundary = int.MaxValue;
         LowerBoundary = 0;
      }

      public override bool Move(ICollectionView itemsView)
      {
         int newPosition = itemsView.CurrentPosition + PositionOffset;
         if (newPosition < 0)
            newPosition = 0;
         if (newPosition > UpperBoundary)
            newPosition = UpperBoundary;
         return itemsView.MoveCurrentToPosition(newPosition);
      }
   }

   public class CollectionViewMoveCurrentToItemAction : MoveActionBase<ICollectionView>
   {
      public object Item { get; set; }

      public override bool Move(ICollectionView itemsView)
      {
         return itemsView.MoveCurrentTo(Item);
      }
   }

   class CollectionViewMoveCurrentToFirst : MoveActionBase<ICollectionView>
   {
      public override bool Move(ICollectionView view)
      {
         return view.MoveCurrentToFirst();
      }
   }

   class CollectionViewMoveCurrentToLast : MoveActionBase<ICollectionView>
   {
      public override bool Move(ICollectionView view)
      {
         return view.MoveCurrentToLast();
      }
   }


   public class CollectionViewMoveActionsFactory : IMoveActionsFactory
   {

      #region IMoveActionsFactory Members

      public IMoveAction GetMoveCurrentToAction(object targetItem)
      {
         return new CollectionViewMoveCurrentToItemAction() { Item = targetItem };
      }

      public IMoveAction GetMoveCurrentToPositionAction(int position)
      {
         return new CollectionViewMoveCurrentToPositionAction() { NewPosition = position };
      }

      public IMoveAction GetMoveCurrentToRelativePositionAction(int offset, int lowerBoundary, int upperBoundary)
      {
         return new CollectionViewMoveCurrentToRelativePositionAction() { PositionOffset = offset, LowerBoundary = lowerBoundary, UpperBoundary = upperBoundary};
      }

      public IMoveAction GetMoveToFirstItemAction()
      {
         return new CollectionViewMoveCurrentToPositionAction() { NewPosition = 0 };
      }

      public IMoveAction GetMoveToLastItemAction()
      {
         return new CollectionViewMoveCurrentToPositionAction() { NewPosition = int.MaxValue };
      }

      #endregion

      public static IMoveActionsFactory Instance { get; private set; }

      static CollectionViewMoveActionsFactory()
      {
         Instance = new CollectionViewMoveActionsFactory();
      }
   }
}
