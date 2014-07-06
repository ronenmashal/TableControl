using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(IVerticalScrollService))]
   internal class DataGridVerticalScrollService : IUIService, IVerticalScrollService
   {
      private DataGrid element;
      private ScrollViewer scrollViewer;

      #region IUIService Members

      public virtual bool IsAttached { get { return element != null; } }

      public void AttachToElement(FrameworkElement element)
      {
         this.element = (DataGrid)element;
         EnsureInvocationAfterLoad(element, () => { scrollViewer = UIUtils.GetVisualChild<ScrollViewer>(element); });
      }

      public void DetachFromElement(FrameworkElement element)
      {
      }

      #endregion IUIService Members

      #region IDisposable Members

      public void Dispose()
      {
         scrollViewer = null;
         element = null;
      }

      #endregion IDisposable Members

      #region IVerticalScrollService Members

      public int ItemsPerPage
      {
         get { return (int)scrollViewer.ViewportHeight; }
      }

      public bool ScrollTo(object item)
      {
         element.ScrollIntoView(item);
         FrameworkElement itemContainer = null;
         element.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { itemContainer = (FrameworkElement)element.ItemContainerGenerator.ContainerFromItem(item); }));
         return itemContainer != null && itemContainer.IsVisible;
      }

      #endregion IVerticalScrollService Members

      public bool ScrollDown(uint distance)
      {
         if (element.Items.Count == 0)
            return false;
         double currentItemIndex = element.Items.IndexOf(element.CurrentItem);
         int targetItemIndex = Math.Min((int)(currentItemIndex + distance), element.Items.Count - 1);
         object targetItem = element.Items[targetItemIndex];
         return ScrollTo(targetItem);
      }

      public bool ScrollToBottom()
      {
         element.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => scrollViewer.ScrollToBottom()));
         return true;
      }

      public bool ScrollToTop()
      {
         scrollViewer.ScrollToTop();
         return true;
      }

      public bool ScrollUp(uint distance)
      {
         if (element.Items.Count == 0)
            return false;
         double currentItemIndex = element.Items.IndexOf(element.CurrentItem);
         int targetItemIndex = Math.Max((int)(currentItemIndex - distance), 0);
         object targetItem = element.Items[targetItemIndex];
         return ScrollTo(targetItem);
      }

      private void EnsureInvocationAfterLoad(FrameworkElement target, Action action)
      {
         if (target.IsLoaded)
            action();
         else
         {
            RoutedEventHandler handler = null;
            handler = (sender, args) =>
            {
               target.Loaded += handler;
               action();
            };
            target.Loaded -= handler;
         }
      }
   }
}