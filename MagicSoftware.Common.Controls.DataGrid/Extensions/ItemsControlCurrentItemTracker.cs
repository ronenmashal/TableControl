using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class ItemsControlCurrentItemTracker
   {
      public static readonly RoutedEvent CurrentChangingEvent = EventManager.RegisterRoutedEvent("CurrentChanging", RoutingStrategy.Bubble, typeof(CancelableRoutedEventArgs), typeof(ItemsControlCurrentItemTracker));

      public static void AddCurrentChangingEventHandler(DependencyObject d, RoutedEventHandler handler)
      {
         UIElement uie = d as UIElement;
         if (uie != null)
         {
            uie.AddHandler(ItemsControlCurrentItemTracker.CurrentChangingEvent, handler);
         }
      }

      public static void RemoveCurrentChangingHandler(DependencyObject d, RoutedEventHandler handler)
      {
         UIElement uie = d as UIElement;
         if (uie != null)
         {
            uie.RemoveHandler(ItemsControlCurrentItemTracker.CurrentChangingEvent, handler);
         }
      }

      ItemsControl control;
      public ItemsControlCurrentItemTracker(ItemsControl control)
      {
         this.control = control;
         control.PreviewKeyDown += new KeyEventHandler(control_PreviewKeyDown);
      }

      void control_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         switch (e.Key)
         {
            case Key.Down:
            case Key.Up:
               var args = new CancelableRoutedEventArgs(CurrentChangingEvent, control);
               control.RaiseEvent(args);
               if (args.Canceled)
                  e.Handled = true;
               break;
         }
      }


   }
}
