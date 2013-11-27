using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Input;
using MagicSoftware.Common.Controls.ProxiesX;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   public class DataGridMouseHandler : MouseHandlerBase
   {
      private DataGridProxy DataGridProxy { get { return Proxy as DataGridProxy; } }

      protected override void Initialize()
      {
         RegisterMouseEvents(AttachedElement);
      }

      public override void DetachFromElement()
      {
         UnregisterMouseEvents(AttachedElement);
      }

      protected virtual void RegisterMouseEvents(UIElement element)
      {
         if (element != null)
         {
            element.PreviewMouseDown += element_PreviewMouseDown;
         }
      }

      protected virtual void UnregisterMouseEvents(UIElement element)
      {
         if (element != null)
         {
            element.PreviewMouseDown -= element_PreviewMouseDown;
         }
      }


      void element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         Trace.WriteLine(String.Format("Mouse down on {0}: {1}, {2}", sender, e.OriginalSource, e.ClickCount));

         bool bClickedWithinFocusedElement = (VisualTreeHelper.HitTest(Keyboard.FocusedElement as Visual, e.GetPosition(Keyboard.FocusedElement)) != null);
         
         if (bClickedWithinFocusedElement)
            return;

         DataGridRow clickedRow = UIUtils.GetAncestor<DataGridRow>(e.OriginalSource as Visual) as DataGridRow;
         if (clickedRow == null)
            return;

         if (DataGridProxy.IsInEdit && !DataGridProxy.CommitEdit(DataGridEditingUnit.Row, true))
         {
            e.Handled = true;
            return;
         }

         DataGridProxy.Items.MoveCurrentTo(clickedRow.Item);
      }

   }
}
