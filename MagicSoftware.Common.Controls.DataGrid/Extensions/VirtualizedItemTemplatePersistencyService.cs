using System;
using System.Windows;
using System.Windows.Controls;
using log4net;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Ensures that the template assigned to an item is persisted in a virtualizing panel.
   /// When scrolling items in a virtualizing panel, item containers that went off the viewport
   /// may be recycled and used for items that came into the viewport. The container's template
   /// may need to be changed when the item container is loaded, because the container retains the
   /// last template that was assigned to it.
   /// </summary>
   internal class VirtualizedItemTemplatePersistencyService : IUIService
   {
      public static readonly DependencyProperty IsCustomRowProperty =
          DependencyProperty.RegisterAttached("IsCustomRow", typeof(bool), typeof(VirtualizedItemTemplatePersistencyService), new UIPropertyMetadata(false));

      private DataGrid dataGrid;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public virtual bool IsAttached { get { return dataGrid != null; } }

      public static bool GetIsCustomRow(DependencyObject obj)
      {
         return (bool)obj.GetValue(IsCustomRowProperty);
      }

      public static void SetIsCustomRow(DependencyObject obj, bool value)
      {
         obj.SetValue(IsCustomRowProperty, value);
      }

      public void AttachToElement(FrameworkElement element)
      {
         this.dataGrid = (DataGrid)element;
         dataGrid.LoadingRow += DataGrid_LoadingRow;
      }

      public void DetachFromElement(FrameworkElement element)
      {
         if (dataGrid == null)
         {
            log.WarnFormat("Detaching extender {0} from {1} twice", this, element);
            return;
         }
         dataGrid.LoadingRow -= DataGrid_LoadingRow;
         dataGrid = null;
      }

      public void Dispose()
      {
         if (dataGrid != null)
            DetachFromElement(dataGrid);
      }

      private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs eventArgs)
      {
         log.DebugFormat("Loading row for item {0}", eventArgs.Row.Item);
         DataGridRow row = eventArgs.Row;
         if (dataGrid.RowStyleSelector != null)
         {
            Style rowStyle = dataGrid.RowStyleSelector.SelectStyle(eventArgs.Row.Item, eventArgs.Row);
            if (rowStyle == null)
            {
               var rowStyleKey = FrameworkElementAccessor.GetDefaultStyleKeyProperty(row);
               rowStyle = row.FindResource(rowStyleKey) as Style;
            }
            log.DebugFormat("Applying row style: {0}", rowStyle);
            row.Style = rowStyle;
         }
      }

      private class FrameworkElementAccessor : FrameworkElement
      {
         public static object GetDefaultStyleKeyProperty(DependencyObject element)
         {
            return (FrameworkElement)element.GetValue(FrameworkElement.DefaultStyleKeyProperty);
         }
      }
   }
}