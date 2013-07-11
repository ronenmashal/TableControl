using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace MagicSoftware.Common.Controls.DataGrid
{
   public class EnhancedDataGrid : System.Windows.Controls.DataGrid
   {
      static EnhancedDataGrid()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(EnhancedDataGrid), new FrameworkPropertyMetadata(typeof(EnhancedDataGrid)));
      }

      protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
      {
         Trace.WriteLine("Preparing container for item " + item);
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
   }
}
