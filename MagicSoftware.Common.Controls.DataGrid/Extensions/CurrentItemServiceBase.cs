using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Proxies;
using MagicSoftware.Common.Controls.Table.CellTypes;
using System;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedServiceAttribute(typeof(ICurrentItemService))]
   public abstract class CurrentItemServiceBase : ICurrentItemService, IUIService
   {
      #region CurrentChanging attached event.

      public static readonly RoutedEvent PreviewCurrentChangingEvent = EventManager.RegisterRoutedEvent("PreviewCurrentChanging", RoutingStrategy.Tunnel, typeof(CancelableRoutedEventArgs), typeof(CurrentItemServiceBase));

      Dictionary<CancelableRoutedEventHandler, RoutedEventHandler> currentChangingEventHandlers = new Dictionary<CancelableRoutedEventHandler, RoutedEventHandler>();

      FrameworkElement element;
      protected readonly AutoResetFlag inhibitChangeEvents = new AutoResetFlag();

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event CancelableRoutedEventHandler PreviewCurrentChanging
      {
         add
         {
            var handler = new RoutedEventHandler((obj, args) => { value(obj, (CancelableRoutedEventArgs)args); });
            element.AddHandler(PreviewCurrentChangingEvent, handler);
            currentChangingEventHandlers.Add(value, handler);
         }
         remove
         {
            RoutedEventHandler handler;
            if (currentChangingEventHandlers.TryGetValue(value, out handler))
               element.RemoveHandler(PreviewCurrentChangingEvent, handler);
         }
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent, allowing the handlers to cancel it,
      /// returning the result in 'canceled'
      /// </summary>
      /// 
      /// <param name="canceled">Returns whether any of the event handlers canceled the event.</param>
      public void RaisePreviewCurrentChangingEvent(object newValue, out bool canceled)
      {
         canceled = false;
         if (inhibitChangeEvents.IsSet)
            return;
         CancelableRoutedEventArgs eventArgs = new PreviewChangeEventArgs(PreviewCurrentChangingEvent, element, CurrentItem, newValue, true);
         element.RaiseEvent(eventArgs);
         canceled = eventArgs.Canceled;
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent without allowing canceling of the event.
      /// </summary>
      public void RaiseNonCancelablePreviewCurrentChangingEvent(object newValue)
      {
         if (inhibitChangeEvents.IsSet)
            return;
         CancelableRoutedEventArgs eventArgs = new PreviewChangeEventArgs(PreviewCurrentChangingEvent, element, CurrentItem, newValue, false);
         element.RaiseEvent(eventArgs);
      }

      #endregion

      #region CurrentChanged attached event.

      public static readonly RoutedEvent CurrentChangedEvent = EventManager.RegisterRoutedEvent("CurrentChanged", RoutingStrategy.Tunnel, typeof(RoutedEventArgs), typeof(CurrentItemServiceBase));

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event RoutedEventHandler CurrentChanged
      {
         add { element.AddHandler(CurrentChangedEvent, value); }
         remove { element.RemoveHandler(CurrentChangedEvent, value); }
      }

      public void RaiseCurrentChangedEvent()
      {
         if (inhibitChangeEvents.IsSet)
            return;
         RoutedEventArgs eventArgs = new RoutedEventArgs(CurrentChangedEvent, element);
         element.RaiseEvent(eventArgs);
      }

      #endregion


      protected SharedObjectsService sharedObjectsService { get; private set; }

      public CurrentItemServiceBase()
      {
      }

      public virtual void AttachToElement(FrameworkElement servedElement)
      {
         sharedObjectsService = UIServiceProvider.GetService<SharedObjectsService>(servedElement);
         element = servedElement;
      }

      public abstract void DetachFromElement(FrameworkElement element);

      public abstract object CurrentItem { get; }
      public abstract int CurrentPosition { get; }

      public abstract bool MoveCurrentTo(object item);
      public abstract bool MoveCurrentToFirst();
      public abstract bool MoveCurrentToNext();
      public abstract bool MoveCurrentToPrevious();
      public abstract bool MoveCurrentToLast();
      public abstract bool MoveCurrentToPosition(int position);
      public abstract bool MoveCurrentToRelativePosition(int offset);

      public IDisposable InhibitChangeEvents()
      {
         return inhibitChangeEvents.Set();
      }

      #region IDisposable Members

      public void Dispose()
      {
         DetachFromElement(element);
         element = null;
         sharedObjectsService = null;
      }

      #endregion
   }
}

