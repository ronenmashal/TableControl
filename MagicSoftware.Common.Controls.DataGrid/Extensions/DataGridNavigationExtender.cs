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
      protected IEditingItemsControlProxy EditProxy { get; private set; }

      protected override void Setup()
      {
         EditProxy = DataGridProxy.GetAdapter<IEditingItemsControlProxy>();
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
         if (IsNavigationKey(e.Key))
            MoveByKeyboard(e);
      }

      void MoveByKeyboard(KeyEventArgs eventArgs)
      {
         log.DebugFormat("Beginning move on {0} using key \"{1}\"", eventArgs.Source, eventArgs.Key);
         ICollectionViewMoveAction moveAction;
         switch (eventArgs.Key)
         {
            case Key.Up:
            case Key.Down:
            case Key.PageUp:
            case Key.PageDown:
               moveAction = GetRelativeMoveAction(eventArgs.Key);
               break;

            case Key.Home:
               moveAction = DataGridProxy.GetMoveToFirstItemAction();
               break;

            case Key.End:
               moveAction = DataGridProxy.GetMoveToLastItemAction();
               break;

            default:
               return;
         }
         DataGridProxy.MoveCurrent(moveAction);
         eventArgs.Handled = true;
         DataGridProxy.ScrollIntoView(DataGridProxy.CurrentItem);
      }

      ICollectionViewMoveAction GetRelativeMoveAction(Key key)
      {
         var moveAction = new MoveCurrentToRelativePosition() { UpperBoundary = TargetElement.Items.Count - 1 };
         switch (key)
         {
            case Key.Up:
               moveAction.PositionOffset = -1;
               break;

            case Key.Down:
               moveAction.PositionOffset = 1;
               break;

            case Key.PageUp:
               moveAction.PositionOffset = -DataGridProxy.RowsPerPage;
               break;

            case Key.PageDown:
               moveAction.PositionOffset = DataGridProxy.RowsPerPage;
               break;
         }
         return moveAction;
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

         if (EditProxy.IsEditing && !TargetElement.CommitEdit(DataGridEditingUnit.Row, true))
         {
            e.Handled = true;
            return;
         }

         DataGridProxy.MoveCurrent(new MoveCurrentToItemAction() { Item = clickedRow.Item });
      }

      public static bool IsNavigationKey(Key key)
      {
         switch (key)
         {
            case Key.Up:
            case Key.Down:
            case Key.PageUp:
            case Key.PageDown:
            case Key.Home:
            case Key.End:
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
