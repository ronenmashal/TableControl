using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(IFocusManagementService))]
   internal class FocusManagementService : IFocusManagementService, IUIService
   {
      private readonly AutoResetFlag isUpdatingFocus = new AutoResetFlag();
      private ICurrentCellService currentCellService;
      private IDisposable deferedFocus = null;
      private int focusDeferCount = 0;
      private bool hasPendingFocus;
      private bool isFirstFocus = true;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public bool IsAttached
      {
         get { return TargetElement != null; }
      }

      protected FrameworkElement TargetElement { get; private set; }

      public void AttachToElement(FrameworkElement element)
      {
         TargetElement = element;
         currentCellService = UIServiceProvider.GetService<ICurrentCellService>(TargetElement);
         currentCellService.CurrentCellChanged += new EventHandler(currentCellService_CurrentCellChanged);

         TargetElement.PreviewGotKeyboardFocus += TargetElement_PreviewGotKeyboardFocus;
         TargetElement.PreviewLostKeyboardFocus += TargetElement_PreviewLostKeyboardFocus;
      }

      public IDisposable DeferFocusUpdate()
      {
         focusDeferCount++;
         log.Debug("Deferring focus updates. Defer count: " + focusDeferCount);
         return new DisposalActionCaller(new Action(() =>
         {
            focusDeferCount--;
            log.Debug("Disposing focus defer. Defer count: " + focusDeferCount);
            if (focusDeferCount == 0 && hasPendingFocus)
               UpdateFocus();
         }));
      }

      public void DetachFromElement(FrameworkElement element)
      {
         if (TargetElement == null)
         {
            log.WarnFormat("Detaching extender {0} from {1} twice", this, element);
            return;
         }

         currentCellService.CurrentCellChanged -= new EventHandler(currentCellService_CurrentCellChanged);
         TargetElement.PreviewLostKeyboardFocus -= TargetElement_PreviewLostKeyboardFocus;
         TargetElement.PreviewGotKeyboardFocus -= TargetElement_PreviewGotKeyboardFocus;
         TargetElement = null;
         currentCellService = null;
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(TargetElement);
      }

      private DispatcherOperation nextFocusUpdateOperation = null;

      public void SetFocusOnTargetElement()
      {
         TargetElement.Focus();
         UpdateFocus();
      }

      public void UpdateFocus()
      {
         if (focusDeferCount > 0)
         {
            hasPendingFocus = true;
            return;
         }

         hasPendingFocus = false;

         if (!TargetElement.IsKeyboardFocusWithin)
         {
            log.DebugFormat("Ignoring UpdateFocus() request on {0}, because it does not have keyboard focus.", TargetElement);
            return;
         }

         log.DebugFormat("Begin updating focus on {0}", TargetElement);

         if (nextFocusUpdateOperation != null)
         {
            nextFocusUpdateOperation.Abort();
         }

         IsRestoringFocusOnElement = isRestoringState.IsSet;

         nextFocusUpdateOperation = TargetElement.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
         {
            using (new DisposalActionCaller(() => IsRestoringFocusOnElement = false))
            {
               log.DebugFormat("Updating focus on {0}", TargetElement);
               using (LoggingExtensions.Indent())
               {
                  //IElementEditStateService editStateService = null;
                  //if (currentCellService.CurrentItemContainer != null)
                  //   editStateService = UIServiceProvider.GetService<IElementEditStateService>(currentCellService.CurrentItemContainer, false);

                  //if (editStateService == null)
                  //   editStateService = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);

                  var elementToFocus = currentCellService.CurrentCellContainer;
                  if (elementToFocus == null)
                     return;

                  if (elementToFocus.IsFocused)
                  {
                     log.DebugFormat("The cell {0} is already in focus.", elementToFocus);
                     return;
                  }

                  var vte = VisualTreeHelpers.GetVisualTreeEnumerator(elementToFocus,
                     (v) => { return (v is UIElement) && ((UIElement) v).Focusable; }, FocusNavigationDirection.Next);
                  while (vte.MoveNext())
                  {
                     elementToFocus = vte.Current as FrameworkElement;
                  }
                  log.DebugFormat("Trying to set focus on element {0}", elementToFocus);
                  using (isUpdatingFocus.Set())
                  {
                     bool isFocused = elementToFocus.Focus();
                     log.DebugFormat("Focus result: {0}, Focused element is {1}", isFocused, Keyboard.FocusedElement);
                  }
                  isFirstFocus = false;
                  nextFocusUpdateOperation = null;
               }
            }
         }));
      }

      public bool IsRestoringFocusOnElement { get; private set; }

      private void currentCellService_CurrentCellChanged(object sender, EventArgs e)
      {
         UpdateFocus();
      }

      readonly AutoResetFlag isRestoringState = new AutoResetFlag();

      private void TargetElement_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
      {
         log.DebugFormat("Regaining keyboard focus: {0}, from {1}, to {2}", sender, e.OldFocus, e.NewFocus);

         //if (isUpdatingFocus.IsSet)
         //   return;

         if (isFirstFocus)
         {
            isFirstFocus = false;
            log.Debug("Got focus for the first time.");
            UpdateFocus();
            return;
         }

         if (isRestoringState.IsSet)
            return;

         using (isRestoringState.Set())
         {
            DependencyObject oldFocus = e.OldFocus as DependencyObject;
            DependencyObject newFocus = e.NewFocus as DependencyObject;

            if (oldFocus != null & newFocus != null)
            {
               if (TargetElement.IsAncestorOf(oldFocus) && TargetElement.IsAncestorOf(newFocus))
               {
                  log.DebugFormat("Focus changed within the same element");
                  // Focus moved within the container.
                  return;
               }
            }

            log.Debug("Restoring state on UI service providers");

            var statePersistentServices = UIServiceProvider.GetAllServices<IStatePersistency>(TargetElement);
            foreach (var service in statePersistentServices)
            {
               service.RestoreState();
            }
         }
      }

      private void TargetElement_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
      {
         log.DebugFormat("Losing keyboard focus: {0}, from {1}, to {2}", sender, e.OldFocus, e.NewFocus);

         DependencyObject oldFocus = e.OldFocus as DependencyObject;
         DependencyObject newFocus = e.NewFocus as DependencyObject;

         if (oldFocus != null & newFocus != null)
         {
            if (TargetElement.IsAncestorOf(oldFocus) && TargetElement.IsAncestorOf(newFocus))
               // Focus moved within the container.
               return;
         }

         log.Debug("Saving state on UI service providers");

         var statePersistentServices = UIServiceProvider.GetAllServices<IStatePersistency>(TargetElement);
         foreach (var service in statePersistentServices)
         {
            service.SaveCurrentState();
         }
      }
   }
}