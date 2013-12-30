using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICurrentItemProvider
   {
      event CancelableRoutedEventHandler PreviewCurrentChanging;
      event RoutedEventHandler CurrentChanged;

      object CurrentItem { get; }
      int CurrentPosition { get; }

      bool MoveCurrentTo(object item);
      bool MoveCurrentToFirst();
      bool MoveCurrentToNext();
      bool MoveCurrentToPrevious();
      bool MoveCurrentToLast();
      bool MoveCurrentToPosition(int position);
      bool MoveCurrentToRelativePosition(int offset);
   }
}
