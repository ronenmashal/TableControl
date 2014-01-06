using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class CurrentChangedEventService
   {
      #region CurrentChanging attached event.

      public static readonly RoutedEvent CurrentChangedEvent = EventManager.RegisterRoutedEvent("CurrentChanged", RoutingStrategy.Tunnel, typeof(RoutedEventArgs), typeof(CurrentChangedEventService));

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event RoutedEventHandler CurrentChanged
      {
         add { proxy.AddRoutedEventHandler(CurrentChangedEvent, value); }
         remove { proxy.RemoveRoutedEventHandler(CurrentChangedEvent, value); }
      }

      public void RaiseCurrentChangedEvent()
      {
         RoutedEventArgs eventArgs = new RoutedEventArgs(CurrentChangedEvent, proxy.ElementAsEventSource);
         proxy.RaiseEvent(eventArgs);
      }

      #endregion

      ImprovedItemsControlProxy proxy;

      public CurrentChangedEventService(ImprovedItemsControlProxy proxy)
      {
         this.proxy = proxy;
      }
   }
}
