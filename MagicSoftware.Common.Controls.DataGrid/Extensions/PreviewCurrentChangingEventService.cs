using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class PreviewCurrentChangingEventService : IDisposable
   {
      #region CurrentChanging attached event.

      public static readonly RoutedEvent PreviewCurrentChangingEvent = EventManager.RegisterRoutedEvent("PreviewCurrentChanging", RoutingStrategy.Tunnel, typeof(CancelableRoutedEventArgs), typeof(PreviewCurrentChangingEventService));

      Dictionary<CancelableRoutedEventHandler, RoutedEventHandler> currentChangingEventHandlers = new Dictionary<CancelableRoutedEventHandler, RoutedEventHandler>();

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event CancelableRoutedEventHandler PreviewCurrentChanging
      {
         add
         {
            var handler = new RoutedEventHandler((obj, args) => { value(obj, (CancelableRoutedEventArgs)args); });
            dgProxy.AddRoutedEventHandler(PreviewCurrentChangingEvent, handler);
            currentChangingEventHandlers.Add(value, handler);
         }
         remove
         {
            RoutedEventHandler handler;
            if (currentChangingEventHandlers.TryGetValue(value, out handler))
               dgProxy.RemoveRoutedEventHandler(PreviewCurrentChangingEvent, handler);
         }
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent, allowing the handlers to cancel it,
      /// returning the result in 'canceled'
      /// </summary>
      /// 
      /// <param name="canceled">Returns whether any of the event handlers canceled the event.</param>
      public void RaisePreviewCurrentChangingEvent(object oldValue, object newValue, out bool canceled)
      {
         CancelableRoutedEventArgs eventArgs = new PreviewCurrentChangingEventArgs(dgProxy.ElementAsEventSource, oldValue, newValue, true);
         dgProxy.RaiseEvent(eventArgs);
         canceled = eventArgs.Canceled;
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent without allowing canceling of the event.
      /// </summary>
      public void RaiseNonCancelablePreviewCurrentChangingEvent(object oldValue, object newValue)
      {
         CancelableRoutedEventArgs eventArgs = new PreviewCurrentChangingEventArgs(dgProxy.ElementAsEventSource, oldValue, newValue, false);
         dgProxy.RaiseEvent(eventArgs);
      }

      #endregion
      
      ImprovedItemsControlProxy dgProxy;

      public PreviewCurrentChangingEventService(ImprovedItemsControlProxy proxy)
      {
         dgProxy = proxy;
      }

      #region IDisposable Members

      public void Dispose()
      {
         
      }

      #endregion
   }

   class PreviewCurrentChangingEventArgs : CancelableRoutedEventArgs
   {
      public object OldValue { get; private set; }
      public object NewValue { get; private set; }

      public PreviewCurrentChangingEventArgs(object source, object oldValue, object newValue, bool isCancelable)
         : base(PreviewCurrentChangingEventService.PreviewCurrentChangingEvent, source, isCancelable)
      {
         OldValue = oldValue;
         NewValue = newValue;
      }
   }


}
