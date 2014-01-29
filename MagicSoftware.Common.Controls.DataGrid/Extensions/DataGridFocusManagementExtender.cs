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
      protected ICurrentItemService CurrentItemTracker { get; private set; }

      protected override void Setup()
      {
         //TODO: replace following with ICurrentCellService.
         //currentChangedService.CurrentChanged += CurrentItemTracker_CurrentChanged;
         CurrentItemTracker = DataGridProxy.GetAdapter<ICurrentItemService>();
      }

      protected override void Cleanup()
      {
         CurrentItemTracker.CurrentChanged -= CurrentItemTracker_CurrentChanged;
         //TODO: ICurrentCellService
      }

      void CurrentItemTracker_CurrentChanged(object sender, EventArgs e)
      {
         var currentItemContainer = DataGridProxy.CurrentItemContainer();
         if (currentItemContainer == null)
            return;

         var currentItemContainerProxy = DataGridProxy.GetItemContainerProxy(currentItemContainer);
         ICurrentItemService currentItemService = currentItemContainerProxy.GetAdapter<ICurrentItemService>();
         if (currentItemService.CurrentItem != null)
         {
            ((UIElement)currentItemService.CurrentItem).Focus();
         }
      }
   }
}
