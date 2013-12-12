using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Documents;
using log4net;

namespace MagicSoftware.Common.Controls.Table
{
   public class Table : System.Windows.Controls.DataGrid
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      static Table()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(Table), new FrameworkPropertyMetadata(typeof(Table)));
      }

      protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
      {
         log.DebugFormat("Preparing container for item {0}", item);
         base.PrepareContainerForItemOverride(element, item);
         if (RowStyleSelector != null)
         {
            Style rowStyle = RowStyleSelector.SelectStyle(item, element);
            if (rowStyle == null)
            {
               var rowStyleKey = element.GetValue(FrameworkElement.DefaultStyleKeyProperty);
               rowStyle = ((FrameworkElement)element).FindResource(rowStyleKey) as Style;
            }
            ((DataGridRow)element).Style = rowStyle;
         }
      }

      protected override void ClearContainerForItemOverride(DependencyObject element, object item)
      {
         base.ClearContainerForItemOverride(element, item);

         if (this.EnableRowVirtualization)
         {
            var container = element as DataGridRow;
            if (container != null)
            {
               var adornerLayer = AdornerLayer.GetAdornerLayer(container);
               if (adornerLayer != null)
               {
                  Adorner[] adorners = adornerLayer.GetAdorners(container);
                  if (adorners != null)
                  {
                     foreach (var adorner in adorners)
                        adornerLayer.Remove(adorner);
                  }
               }
            }
         }
      }
   }
}
