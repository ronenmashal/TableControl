using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace _DGTester
{
   class RowStyleSelector : StyleSelector
   {
      public override Style SelectStyle(object item, DependencyObject container)
      {
         Trace.WriteLine(String.Format("Choosing a style for item {0} on container {1}", item, container));
         FrameworkElement containerElement = container as FrameworkElement;
         if (containerElement != null)
         {
            Style resource = containerElement.TryFindResource(item.GetType()) as Style;
            if (resource != null)
            {
               Trace.WriteLine("Proper style was found");
               return resource;
            }
         }

         Trace.WriteLine("Did not find a proper style.");
         return null;
      }
   }
}
