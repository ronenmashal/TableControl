using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class CancelableRoutedEventArgs : RoutedEventArgs
   {
      public bool Canceled { get; set; }
      public CancelableRoutedEventArgs(RoutedEvent routedEvent, object source)
         : base(routedEvent, source)
      {
         Canceled = false;
      }
   }

   public delegate void CancelableRoutedEventHandler(object sender, CancelableRoutedEventArgs eventArgs);
}
