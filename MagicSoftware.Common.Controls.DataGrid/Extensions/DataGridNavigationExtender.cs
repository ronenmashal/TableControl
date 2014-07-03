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

      //protected EnhancedDGProxy DataGridProxy { get { return (EnhancedDGProxy)TargetElementProxy; } }
      //protected IEditingItemsControlProxy EditProxy { get; private set; }
      //ICurrentItemService itemContainerCurrentItemService;
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
         //EditProxy = DataGridProxy.GetAdapter<IEditingItemsControlProxy>();
         currentCellService = UIServiceProvider.GetService<ICurrentCellService>(TargetElement);
         Debug.Assert(currentCellService != null);

         inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         Debug.Assert(inputService != null);

         scrollService = UIServiceProvider.GetService<IVerticalScrollService>(TargetElement);
         Debug.Assert(scrollService != null);

         RegisterActionGesture(AsLineKeyAction(MoveLineDown), new KeyGesturesFactory(Key.Down, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         RegisterActionGesture(AsLineKeyAction(MoveLineUp), new KeyGesturesFactory(Key.Up, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         RegisterActionGesture(AsLineKeyAction(MovePageDown), new KeyGesturesFactory(Key.PageDown, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         RegisterActionGesture(AsLineKeyAction(MovePageUp), new KeyGesturesFactory(Key.PageUp, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));
         RegisterActionGesture(AsLineKeyAction(MoveToTop), new KeyGesturesFactory(Key.Home, ModifierKeys.Control));
         RegisterActionGesture(AsLineKeyAction(MoveToBottom), new KeyGesturesFactory(Key.End, ModifierKeys.Control));

         RegisterActionGesture(AsFieldKeyAction(MoveRight), new KeyGesturesFactory(Key.Tab));
         RegisterActionGesture(AsFieldKeyAction(MoveRight), new KeyGesturesFactory(Key.Right, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));

         RegisterActionGesture(AsFieldKeyAction(MoveLeft), new KeyGesturesFactory(Key.Tab, ModifierKeys.Shift));
         RegisterActionGesture(AsFieldKeyAction(MoveLeft), new KeyGesturesFactory(Key.Left, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift)));

         RegisterActionGesture(AsFieldKeyAction(MoveToLeftMost), new KeyGesturesFactory(Key.Home));
         RegisterActionGesture(AsFieldKeyAction(MoveToRightMost), new KeyGesturesFactory(Key.End));

         //TargetElement.PreviewKeyDown += TargetElement_PreviewKeyDown;
         //TargetElement.PreviewMouseDown += TargetElement_PreviewMouseDown;
      }

      private ModifierKeys[] AllCombinationsOf(params ModifierKeys[] modifiers)
      {
         ModifierKeys[] combinations = new ModifierKeys[modifiers.Length * modifiers.Length];
         combinations[0] = ModifierKeys.None;
         int c = 1;
         for (int i = 0; i < modifiers.Length; i++)
         {
            combinations[c++] = modifiers[i];
            for (int j = i + 1; j < modifiers.Length; j++)
            {
               combinations[c++] = modifiers[i] | modifiers[j];
            }
         }
         return combinations;
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

      private void RegisterActionGesture(Action<KeyEventArgs> action, KeyGesturesFactory gesturesFactory)
      {
         foreach (KeyGesture gesture in gesturesFactory.GetGestures())
            inputService.RegisterKeyGestureAction(gesture, action);
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

         //if (EditProxy.IsEditing && !TargetElement.CommitEdit(DataGridEditingUnit.Row, true))
         //{
         //   e.Handled = true;
         //   return;
         //}

         currentCellService.MoveTo(new UniversalCellInfo(clickedRow.Item, 0));
      }
   }

   internal abstract class InputGesturesFactory
   {
      public abstract InputGesture[] GetGestures();
   }

   internal class KeyGesturesFactory : InputGesturesFactory
   {
      private Key key;
      private ModifierKeys[] modifierCombinations;

      public KeyGesturesFactory(Key key, params ModifierKeys[] modifierCombinations)
      {
         this.key = key;
         this.modifierCombinations = modifierCombinations;
      }

      public override InputGesture[] GetGestures()
      {
         InputGesture[] gestures = new InputGesture[modifierCombinations.Length];
         int i = 0;
         foreach (var modifier in modifierCombinations)
            gestures[i++] = new KeyGesture(key, modifier);
         return gestures;
      }
   }
}