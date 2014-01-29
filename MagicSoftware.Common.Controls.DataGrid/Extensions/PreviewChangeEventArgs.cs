using System;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class PreviewChangeEventArgs : CancelableEventArgs
   {
      public object OldValue { get; private set; }
      public object NewValue { get; private set; }

      public PreviewChangeEventArgs(object oldValue, object newValue, bool isCancelable): base(isCancelable)
      {
         OldValue = oldValue;
         NewValue = newValue;
      }
   }
}

