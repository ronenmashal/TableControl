using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public abstract class GestureMapping<G, E>
      where G : InputGesture
   {
      public G Gesture { get; set; }
      public E MappedValue { get; set; }

      public GestureMapping()
      { }
   }

   class NavigationCommandsMap
   {
      
   }

   enum NavigationOperation
   {
      MoveToNextItem,
      MoveToPreviousItem,
      MoveToNextPage,
      MoveToPreviousPage,
      MoveToNextColumn,
      MoveToPreviousColumn,
      MoveToFirstItem,
      MoveToLastItem,
      MoveToLeftMostColumn,
      MoveToRightMostColumn,
      MoveToFirstItemOnPage,
      MoveToLastItemOnPage
   };

   class NavigationKeyGestureMapping : GestureMapping<KeyGesture, NavigationOperation>
   {

   }
}
