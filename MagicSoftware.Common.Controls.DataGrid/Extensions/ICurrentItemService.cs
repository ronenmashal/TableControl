using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentItemService
   {
      event CancelableRoutedEventHandler PreviewCurrentChanging;
      event RoutedEventHandler CurrentChanged;

      object CurrentItem { get; }
      int CurrentPosition { get; }

      /// <summary>
      /// Moves the CurrentItem to the specified item.
      /// </summary>
      /// <param name="item">The item to set the CurrentItem to.</param>
      /// <returns><b>True</b> if the resulting CurrentItem is not null and is set to the item specified by the argument.</returns>
      bool MoveCurrentTo(object item);

      /// <summary>
      /// Moves the CurrentItem to the first item on the list. If no such item exist, i.e. the list is empty, the
      /// CurrentItem will be set to null.
      /// </summary>
      /// <returns><b>True</b> if CurrentItem is not null.</returns>
      bool MoveCurrentToFirst();

      /// <summary>
      /// Moves the CurrentItem to the last item on the list. If no such item exist, i.e. the list is empty, the
      /// CurrentItem will be set to null.
      /// </summary>
      /// <returns><b>True</b> if CurrentItem is not null.</returns>
      bool MoveCurrentToNext();

      /// <summary>
      /// Moves CurrentItem to the item the precedes it in the list. If no such item exist, i.e. the list is empty, 
      /// or CurrentItem is on the first item, CurrentItem will be set to null.
      /// </summary>
      /// <returns><b>True</b> if CurrentItem is not null.</returns>
      bool MoveCurrentToPrevious();

      /// <summary>
      /// Moves CurrentItem to the item the follows it in the list. If no such item exist, i.e. the list is empty, 
      /// or CurrentItem is on the last item, CurrentItem will be set to null.
      /// </summary>
      /// <returns><b>True</b> if CurrentItem is not null.</returns>
      bool MoveCurrentToLast();

      /// <summary>
      /// Moves CurrentItem to the item in the specified position.
      /// </summary>
      /// <param name="position">The position to which to move the item.</param>
      /// <returns><b>True</b> if CurrentItem is not null.</returns>
      bool MoveCurrentToPosition(int position);

      /// <summary>
      /// Moves CurrentItem to the item in the position which is at the given distance (offset) 
      /// from the current position.
      /// </summary>
      /// <returns><b>True</b> if CurrentItem is not null.</returns>
      bool MoveCurrentToRelativePosition(int offset);
   }
}
