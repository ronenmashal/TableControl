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

      bool MoveCurrent(ICollectionViewMoveAction moveAction);

      object CurrentItem { get; }
      int CurrentPosition { get; }
   }
}
