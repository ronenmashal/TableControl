using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Proxies;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridFocusManagementExtender : ElementExtenderBase<DataGrid>
   {
      public static IFocusManager GetFocusManager(DependencyObject obj)
      {
         return (IFocusManager)obj.GetValue(FocusManagerProperty);
      }

      public static void SetFocusManager(DependencyObject obj, IFocusManager value)
      {
         obj.SetValue(FocusManagerProperty, value);
      }

      // Using a DependencyProperty as the backing store for FocusManager.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty FocusManagerProperty =
          DependencyProperty.RegisterAttached("FocusManager", typeof(IFocusManager), typeof(DataGridFocusManagementExtender), new UIPropertyMetadata(null));


      protected EnhancedDGProxy DataGridProxy { get { return (EnhancedDGProxy)TargetElementProxy; } }
      protected ICurrentItemProvider CurrentItemTracker { get; private set; }
      CurrentChangedEventService currentChangedService;

      protected override void Setup()
      {
         currentChangedService = DataGridProxy.GetAdapter<CurrentChangedEventService>();
         currentChangedService.CurrentChanged += CurrentItemTracker_CurrentChanged;
         CurrentItemTracker = DataGridProxy.GetAdapter<ICurrentItemProvider>();
         //CurrentItemTracker.PreviewCurrentChanging += CurrentItemTracker_PreviewCurrentChanging;
         //CurrentItemTracker.CurrentChanged += CurrentItemTracker_CurrentChanged;
      }

      protected override void Cleanup()
      {
         //CurrentItemTracker.PreviewCurrentChanging -= CurrentItemTracker_PreviewCurrentChanging;
         CurrentItemTracker.CurrentChanged -= CurrentItemTracker_CurrentChanged;
         currentChangedService.CurrentChanged -= CurrentItemTracker_CurrentChanged;
      }

      //ICurrentItemProvider previousLineCurrentItemContainer = null;

      //void CurrentItemTracker_PreviewCurrentChanging(object sender, CancelableRoutedEventArgs eventArgs)
      //{
      //   var currentItemContainer = DataGridProxy.CurrentItemContainer();
      //   if (currentItemContainer != null)
      //   {
      //      var currentItemContainerProxy = DataGridProxy.GetItemContainerProxy(currentItemContainer);
      //      previousLineCurrentItemContainer = currentItemContainerProxy.GetAdapter<ICurrentItemProvider>();
      //   }
      //}

      void CurrentItemTracker_CurrentChanged(object sender, RoutedEventArgs e)
      {
         var currentItemContainer = DataGridProxy.CurrentItemContainer();
         if (currentItemContainer == null)
            return;
         var currentItemContainerProxy = DataGridProxy.GetItemContainerProxy(currentItemContainer);
         ICurrentItemProvider currentItemService = currentItemContainerProxy.GetAdapter<ICurrentItemProvider>();
         //if (previousLineCurrentItemContainer != null)
         //{
         //   if (previousLineCurrentItemContainer.CurrentPosition > 0)
         //      container.MoveCurrentToPosition(previousLineCurrentItemContainer.CurrentPosition);
         //   else
         //      container.MoveCurrentToPosition(0);
         //}
         //else
         //{
         //   container.MoveCurrentToFirst();
         //}
         if (currentItemService.CurrentItem != null)
         {
            ((UIElement)currentItemService.CurrentItem).Focus();
         }
      }
   }
}
