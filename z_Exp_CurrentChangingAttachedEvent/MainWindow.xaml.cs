using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace z_Exp_CurrentChangingAttachedEvent
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public static List<MyData> Items = new List<MyData>()
      {
         new MyData() {Str1 = "x", Str2 = "y"},
         new MyData() {Str1 = "a", Str2 = "b"}
      };

      DGCurrentItemTracker tracker;

      public MainWindow()
      {
         InitializeComponent();
         tracker = new DGCurrentItemTracker(dg);
         DGCurrentItemTracker.AddCurrentChangingEventHandler(dg, CurrentChangingHandler);
      }

      void CurrentChangingHandler(object sender, RoutedEventArgs args)
      {
         var cancelArgs = (CancelableRoutedEventArgs)args;
         cancelArgs.Canceled = true;
      }
   }

   public class MyData
   {
      public string Str1 { get; set; }
      public string Str2 { get; set; }
   }

   public class CancelableRoutedEventArgs : RoutedEventArgs
   {
      public bool Canceled { get; set; }
      public CancelableRoutedEventArgs(RoutedEvent routedEvent, object source)
         : base(routedEvent, source)
      {
         Canceled = false;
      }
   }

   public class DGCurrentItemTracker
   {
      public static readonly RoutedEvent CurrentChangingEvent = EventManager.RegisterRoutedEvent("CurrentChanging", RoutingStrategy.Bubble, typeof(CancelableRoutedEventArgs), typeof(DGCurrentItemTracker));

      public static void AddCurrentChangingEventHandler(DependencyObject d, RoutedEventHandler handler)
      {
         UIElement uie = d as UIElement;
         if (uie != null)
         {
            uie.AddHandler(DGCurrentItemTracker.CurrentChangingEvent, handler);
         }
      }

      public static void RemoveCurrentChangingHandler(DependencyObject d, RoutedEventHandler handler)
      {
         UIElement uie = d as UIElement;
         if (uie != null)
         {
            uie.RemoveHandler(DGCurrentItemTracker.CurrentChangingEvent, handler);
         }
      }

      ItemsControl control;
      public DGCurrentItemTracker(ItemsControl control)
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
