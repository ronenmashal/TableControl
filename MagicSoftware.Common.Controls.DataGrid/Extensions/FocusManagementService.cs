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
      private ICurrentCellService currentCellService;
      private int focusDeferCount = 0;
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
      }

      public IDisposable DeferFocusChanges()
      {
         focusDeferCount++;
         return new DisposalActionCaller(new Action(() =>
         {
            focusDeferCount--;
            if (focusDeferCount == 0)
               UpdateFocus();
         }));
      }

      public void DetachFromElement(FrameworkElement element)
      {
         TargetElement = null;
         currentCellService.CurrentCellChanged -= new EventHandler(currentCellService_CurrentCellChanged);
         currentCellService = null;
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(TargetElement);
      }

      public void UpdateFocus()
      {
         if (focusDeferCount > 0)
            return;

         TargetElement.Dispatcher.Invoke(DispatcherPriority.Input, new Action(() =>
         {
            IElementEditStateService editStateService = null;
            if (currentCellService.CurrentItemContainer != null)
               editStateService = UIServiceProvider.GetService<IElementEditStateService>(currentCellService.CurrentItemContainer, false);

            if (editStateService == null)
               editStateService = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);

            var elementToFocus = currentCellService.CurrentCellContainer as FrameworkElement;
            if (elementToFocus == null)
               return;

            var vte = VisualTreeHelpers.GetVisualTreeEnumerator(elementToFocus, (v) => { return (v is UIElement) && ((UIElement)v).Focusable; }, FocusNavigationDirection.Next);
            while (vte.MoveNext())
            {
               elementToFocus = vte.Current as FrameworkElement;
            }
            log.DebugFormat("Trying to set focus on element {0}", elementToFocus);
            bool isFocused = elementToFocus.Focus();
            log.DebugFormat("Focus result: {0}", isFocused);
         }));
      }

      private void currentCellService_CurrentCellChanged(object sender, EventArgs e)
      {
         UpdateFocus();
      }
   }
}