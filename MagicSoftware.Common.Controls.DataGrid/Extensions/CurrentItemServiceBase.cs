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
      FrameworkElement element;
      protected readonly AutoResetFlag inhibitChangeEvents = new AutoResetFlag();

      #region CurrentChanging event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler<PreviewChangeEventArgs> PreviewCurrentChanging;

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
         if (PreviewCurrentChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentItem, newValue, true);
            PreviewCurrentChanging(this, eventArgs);
            canceled = eventArgs.Canceled;
         }
      }

      /// <summary>
      /// Raises the PreviewCurrentChangingEvent without allowing canceling of the event.
      /// </summary>
      public void RaiseNonCancelablePreviewCurrentChangingEvent(object newValue)
      {
         if (inhibitChangeEvents.IsSet)
            return;
         if (PreviewCurrentChanging != null)
         {
            var eventArgs = new PreviewChangeEventArgs(CurrentItem, newValue, false);
            PreviewCurrentChanging(this, eventArgs);
         }
      }

      #endregion

      #region CurrentChanged event.

      /// <summary>
      /// Event raised before changing the 'current item' indicator on the items control.
      /// </summary>
      public event EventHandler CurrentChanged;

      public void RaiseCurrentChangedEvent()
      {
         if (inhibitChangeEvents.IsSet)
            return;
         if (CurrentChanged != null)
            CurrentChanged(this, new EventArgs());
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

      public virtual bool IsAttached { get { return element != null; } }

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

