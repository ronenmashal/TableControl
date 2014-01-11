using System;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class PreviewChangeEventArgs : CancelableRoutedEventArgs
   {
      public object OldValue { get; private set; }
      public object NewValue { get; private set; }

      public PreviewChangeEventArgs(RoutedEvent previewChangeEvent, object source, object oldValue, object newValue, bool isCancelable)
         : base(previewChangeEvent, source, isCancelable)
      {
         OldValue = oldValue;
         NewValue = newValue;
      }
   }
}

