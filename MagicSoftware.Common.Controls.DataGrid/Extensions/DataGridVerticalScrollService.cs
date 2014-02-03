using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(IVerticalScrollService))]
   class DataGridVerticalScrollService : IUIService, IVerticalScrollService
   {
      DataGrid element;
      ScrollViewer scrollViewer;

      #region IUIService Members

      public void AttachToElement(FrameworkElement element)
      {
         this.element = (DataGrid)element;
         EnsureInvocationAfterLoad(element, () => { scrollViewer = UIUtils.GetVisualChild<ScrollViewer>(element); });
      }

      public void DetachFromElement(FrameworkElement element)
      {
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         scrollViewer = null;
         element = null;
      }

      #endregion

      #region IVerticalScrollService Members

      public bool ScrollTo(object item)
      {
         element.ScrollIntoView(item);
         FrameworkElement itemContainer = null;
         element.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { itemContainer = (FrameworkElement)element.ItemContainerGenerator.ContainerFromItem(item); }));
         return itemContainer != null && itemContainer.IsVisible;
      }

      public int ItemsPerPage
      {
         get { return (int)scrollViewer.ViewportHeight; }
      }

      #endregion

      void EnsureInvocationAfterLoad(FrameworkElement target, Action action)
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
