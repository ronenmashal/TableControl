using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class CancelableEventArgs : EventArgs
   {
      /// <summary>
      /// Gets or sets a value denoting whether the operation represented by the event
      /// is marked as canceled by the handler.
      /// </summary>
      public bool Canceled { get; set; }

      /// <summary>
      /// Gets a value denoting whether the event source will adhere to the cancellation
      /// status denoted by the Canceled property.
      /// </summary>
      public bool IsCancelable { get; private set; }

      /// <summary>
      /// Instantiates a new CancelableRouteEventArgs for the specified routed event
      /// with source as the event's original source.
      /// </summary>
      /// <param name="source">The original source of the event.</param>
      /// <param name="isCancelable">Denotes whether event hanlders can cancel the operations following the event.</param>
      public CancelableEventArgs(bool isCancelable = true)
      {
         IsCancelable = isCancelable;
         Canceled = false;
      }
   }
}
