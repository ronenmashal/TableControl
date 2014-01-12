using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;
using System.Diagnostics;
using System.Windows;
using MagicSoftware.Common.Controls.Proxies;
using LogLevel = log4net.Core.Level;
using MagicSoftware.Common.Utils;
using System.Windows.Media;
using MagicSoftware.Common.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridNavigationExtender : ElementExtenderBase<DataGrid>
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      protected EnhancedDGProxy DataGridProxy { get { return (EnhancedDGProxy)TargetElementProxy; } }
      //protected IEditingItemsControlProxy EditProxy { get; private set; }

      ICurrentItemService topContainerCurrentItemService;
      ICurrentItemService itemContainerCurrentItemService;

      protected override void Setup()
      {
         //EditProxy = DataGridProxy.GetAdapter<IEditingItemsControlProxy>();
         topContainerCurrentItemService = UIServiceProvider.GetService<ICurrentItemService>(TargetElement);

         TargetElement.PreviewKeyDown += TargetElement_PreviewKeyDown;
         TargetElement.PreviewMouseDown += TargetElement_PreviewMouseDown;
      }

      protected override void Cleanup()
      {
         TargetElement.PreviewKeyDown -= TargetElement_PreviewKeyDown;
         TargetElement.PreviewMouseDown -= TargetElement_PreviewMouseDown;
      }

      protected void TargetElement_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         log.LogMessage(LogLevel.Finer, "Preview key down on {0}: {1}, {2}", sender, e.Key, e.OriginalSource);
         if (IsVerticalNavigationKey(e.Key))
            MoveToLineByKeyboard(e);
         else if (IsHorizontalNavigationKey(e.Key))
            MoveToFieldByKeyboard(e);
      }

      private void MoveToFieldByKeyboard(KeyEventArgs e)
      {
         switch (e.Key)
         {
            case Key.Tab:
               var container = DataGridProxy.CurrentItemContainer();
               itemContainerCurrentItemService = UIServiceProvider.GetService<ICurrentItemService>(container);
               int offset = 1;
               if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                  offset = -1;
               itemContainerCurrentItemService.MoveCurrentToRelativePosition(offset);
               e.Handled = true;
               break;
         }
      }

      void MoveToLineByKeyboard(KeyEventArgs eventArgs)
      {
         log.DebugFormat("Beginning move on {0} using key \"{1}\"", eventArgs.Source, eventArgs.Key);
         bool bMoved;
         switch (eventArgs.Key)
         {
            case Key.Up:
               bMoved = topContainerCurrentItemService.MoveCurrentToPrevious();
               break;

            case Key.Down:
               bMoved = topContainerCurrentItemService.MoveCurrentToNext();
               break;

            case Key.PageUp:
               bMoved = topContainerCurrentItemService.MoveCurrentToRelativePosition(-DataGridProxy.RowsPerPage);
               break;

            case Key.PageDown:
               bMoved = topContainerCurrentItemService.MoveCurrentToRelativePosition(DataGridProxy.RowsPerPage);
               break;

            case Key.Home:
               bMoved = topContainerCurrentItemService.MoveCurrentToFirst();
               break;

            case Key.End:
               bMoved = topContainerCurrentItemService.MoveCurrentToLast();
               break;

            default:
               return;
         }
         eventArgs.Handled = true;
         DataGridProxy.ScrollIntoView(topContainerCurrentItemService.CurrentItem);
      }

      void TargetElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         Trace.WriteLine(String.Format("Mouse down on {0}: {1}, {2}", sender, e.OriginalSource, e.ClickCount));

         bool bClickedWithinFocusedElement = (VisualTreeHelper.HitTest(Keyboard.FocusedElement as Visual, e.GetPosition(Keyboard.FocusedElement)) != null);

         if (bClickedWithinFocusedElement)
            return;

         DataGridRow clickedRow = UIUtils.GetAncestor<DataGridRow>(e.OriginalSource as Visual) as DataGridRow;
         if (clickedRow == null)
            return;

         //if (EditProxy.IsEditing && !TargetElement.CommitEdit(DataGridEditingUnit.Row, true))
         //{
         //   e.Handled = true;
         //   return;
         //}

         topContainerCurrentItemService.MoveCurrentTo(clickedRow.Item);
      }

      public static bool IsNavigationKey(Key key)
      {
         return IsVerticalNavigationKey(key) || IsHorizontalNavigationKey(key);
      }

      public static bool IsVerticalNavigationKey(Key key)
      {
         switch (key)
         {
            case Key.Up:
            case Key.Down:
            case Key.PageUp:
            case Key.PageDown:
            case Key.Home:
            case Key.End:
               return true;
         }
         return false;
      }

      public static bool IsHorizontalNavigationKey(Key key)
      {
         switch (key)
         {
            case Key.Left:
            case Key.Right:
            case Key.Tab:
               return true;
         }

         return false;
      }

      public static bool IsShiftKey(Key key)
      {
         return key == Key.LeftShift || key == Key.RightShift;
      }

   }
}
