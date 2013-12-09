using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using log4net;

namespace _DGTester
{
   class RowStyleSelector : StyleSelector
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public override Style SelectStyle(object item, DependencyObject container)
      {
         log.DebugFormat(String.Format("Choosing a style for item {0} on container {1}", item, container));
         FrameworkElement containerElement = container as FrameworkElement;
         if (containerElement != null)
         {
            Style resource = containerElement.TryFindResource(item.GetType()) as Style;
            if (resource != null)
            {
               log.DebugFormat("Proper style was found");
               return resource;
            }
         }

         log.DebugFormat("Did not find a proper style.");
         return null;
      }
   }
}
