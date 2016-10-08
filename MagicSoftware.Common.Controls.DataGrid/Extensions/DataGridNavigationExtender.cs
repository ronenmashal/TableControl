using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal class DataGridNavigationExtender : ElementExtenderBase<DataGrid>
   {
      private ICurrentCellService currentCellService;

      private InputService inputService;

      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private IVerticalScrollService scrollService;

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

      public static bool IsNavigationKey(Key key)
      {
         return IsVerticalNavigationKey(key) || IsHorizontalNavigationKey(key);
      }

      public static bool IsShiftKey(Key key)
      {
         return key == Key.LeftShift || key == Key.RightShift;
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

      protected override void Cleanup()
      {
         TargetElement.PreviewMouseDown -= TargetElement_PreviewMouseDown;
      }

      protected override void Setup()
      {
         currentCellService = UIServiceProvider.GetService<ICurrentCellService>(TargetElement);
         Debug.Assert(currentCellService != null);

         inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         Debug.Assert(inputService != null);

         scrollService = UIServiceProvider.GetService<IVerticalScrollService>(TargetElement);
         Debug.Assert(scrollService != null);

         inputService.RegisterKeyActionGestures(AsLineKeyAction(MoveLineDown), new KeyGesturesFactory(Key.Down, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         inputService.RegisterKeyActionGestures(AsLineKeyAction(MoveLineUp), new KeyGesturesFactory(Key.Up, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         inputService.RegisterKeyActionGestures(AsLineKeyAction(MovePageDown), new KeyGesturesFactory(Key.PageDown, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         inputService.RegisterKeyActionGestures(AsLineKeyAction(MovePageUp), new KeyGesturesFactory(Key.PageUp, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         inputService.RegisterKeyActionGestures(AsLineKeyAction(MoveToTop), new KeyGesturesFactory(Key.Home, ModifierKeys.Control));
         inputService.RegisterKeyActionGestures(AsLineKeyAction(MoveToBottom), new KeyGesturesFactory(Key.End, ModifierKeys.Control));

         inputService.RegisterKeyActionGestures(AsFieldKeyAction(MoveRight), new KeyGesturesFactory(Key.Tab));
         inputService.RegisterKeyActionGestures(AsFieldKeyAction(MoveRight), new KeyGesturesFactory(Key.Right, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));

         inputService.RegisterKeyActionGestures(AsFieldKeyAction(MoveLeft), new KeyGesturesFactory(Key.Tab, ModifierKeys.Shift));
         inputService.RegisterKeyActionGestures(AsFieldKeyAction(MoveLeft), new KeyGesturesFactory(Key.Left, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));

         inputService.RegisterKeyActionGestures(AsFieldKeyAction(MoveToLeftMost), new KeyGesturesFactory(Key.Home));
         inputService.RegisterKeyActionGestures(AsFieldKeyAction(MoveToRightMost), new KeyGesturesFactory(Key.End));

         inputService.RegisterMouseActionGestures(MouseClicked, new MouseGesturesFactory(MouseAction.LeftClick, InputGesturesFactory.AllCombinationsOf(ModifierKeys.Shift)));
      }

      private Action<KeyEventArgs> AsFieldKeyAction(Action fieldAction)
      {
         return new Action<KeyEventArgs>((args) =>
            {
               log.DebugFormat("Beginning move on {0} using key \"{1}\"", args.Source, args.Key);
               fieldAction();
               args.Handled = true;
            });
      }

      private Action<KeyEventArgs> AsLineKeyAction(Action lineAction)
      {
         return new Action<KeyEventArgs>((args) =>
            {
               log.DebugFormat("Beginning move on {0} using key \"{1}\"", args.Source, args.Key);
               lineAction();
               scrollService.ScrollTo(currentCellService.CurrentCell.Item);
               args.Handled = true;
            });
      }

      private void MouseClicked(MouseEventArgs eventArgs)
      {
         log.Debug("Mouse was clicked on " + eventArgs.OriginalSource);
         HitTestResult hitTestResult = VisualTreeHelper.HitTest(TargetElement, Mouse.GetPosition(TargetElement));
         var row = UIUtils.GetAncestor<DataGridRow>((Visual)hitTestResult.VisualHit);
         if (row != null)
         {
            var rowEnumSvc = UIServiceProvider.GetService<ICellEnumerationService>(row);
            UniversalCellInfo cellInfo = rowEnumSvc.GetCellContaining(hitTestResult.VisualHit);
            if (!currentCellService.CurrentCell.Equals(cellInfo))
            {
               currentCellService.MoveTo(cellInfo);
               eventArgs.Handled = true;
            }
         }
      }

      private void MoveLeft()
      {
         currentCellService.MoveLeft(1);
      }

      private void MoveLineDown()
      {
         currentCellService.MoveDown(1);
      }

      private void MoveLineUp()
      {
         currentCellService.MoveUp(1);
      }

      private void MovePageDown()
      {
         scrollService.ScrollDown((uint)scrollService.ItemsPerPage);
         currentCellService.MoveDown((uint)scrollService.ItemsPerPage);
      }

      private void MovePageUp()
      {
         scrollService.ScrollUp((uint)scrollService.ItemsPerPage);
         currentCellService.MoveUp((uint)scrollService.ItemsPerPage);
      }

      private void MoveRight()
      {
         currentCellService.MoveRight(1);
      }

      private void MoveToBottom()
      {
         scrollService.ScrollToBottom();
         currentCellService.MoveToBottom();
      }

      private void MoveToLeftMost()
      {
         currentCellService.MoveToLeftMost();
      }

      private void MoveToRightMost()
      {
         currentCellService.MoveToRightMost();
      }

      private void MoveToTop()
      {
         scrollService.ScrollToTop();
         currentCellService.MoveToTop();
      }

      private void TargetElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         Trace.WriteLine(String.Format("Mouse down on {0}: {1}, {2}", sender, e.OriginalSource, e.ClickCount));

         bool bClickedWithinFocusedElement = (VisualTreeHelper.HitTest(Keyboard.FocusedElement as Visual, e.GetPosition(Keyboard.FocusedElement)) != null);

         if (bClickedWithinFocusedElement)
            return;

         DataGridRow clickedRow = UIUtils.GetAncestor<DataGridRow>(e.OriginalSource as Visual) as DataGridRow;
         if (clickedRow == null)
            return;

         currentCellService.MoveTo(new UniversalCellInfo(clickedRow.Item, 0));
      }
   }

}