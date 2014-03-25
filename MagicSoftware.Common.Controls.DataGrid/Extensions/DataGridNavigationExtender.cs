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

      ICurrentCellService topContainerCurrentItemService;
      //ICurrentItemService itemContainerCurrentItemService;
      InputService inputService;


      protected override void Setup()
      {
         //EditProxy = DataGridProxy.GetAdapter<IEditingItemsControlProxy>();
         topContainerCurrentItemService = UIServiceProvider.GetService<ICurrentCellService>(TargetElement);
         Debug.Assert(topContainerCurrentItemService != null);

         inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         Debug.Assert(inputService != null);

         RegisterActionGesture(MoveToLineByKeyboard, Key.Down, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift));
         RegisterActionGesture(MoveToLineByKeyboard, Key.Up, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift));
         RegisterActionGesture(MoveToLineByKeyboard, Key.PageDown, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift));
         RegisterActionGesture(MoveToLineByKeyboard, Key.PageUp, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift));
         RegisterActionGesture(MoveToLineByKeyboard, Key.Home, ModifierKeys.Control);
         RegisterActionGesture(MoveToLineByKeyboard, Key.End, ModifierKeys.Control);

         RegisterActionGesture(MoveToFieldByKeyboard, Key.Tab, ModifierKeys.Shift);
         RegisterActionGesture(MoveToFieldByKeyboard, Key.Left, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift));
         RegisterActionGesture(MoveToFieldByKeyboard, Key.Right, AllCombinationsOf(ModifierKeys.Control, ModifierKeys.Shift));
         RegisterActionGesture(MoveToFieldByKeyboard, Key.Home);
         RegisterActionGesture(MoveToFieldByKeyboard, Key.End);

         //TargetElement.PreviewKeyDown += TargetElement_PreviewKeyDown;
         //TargetElement.PreviewMouseDown += TargetElement_PreviewMouseDown;
      }

      ModifierKeys[] AllCombinationsOf(params ModifierKeys[] modifiers)
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

      void RegisterActionGesture(Action<KeyEventArgs> action, Key key)
      {
         RegisterActionGesture(action, key, ModifierKeys.None);
      }

      void RegisterActionGesture(Action<KeyEventArgs> action, Key key, params ModifierKeys[] modifierCombinations)
      {
         foreach (var modifier in modifierCombinations)
            inputService.RegisterKeyGestureAction(new KeyGesture(key, modifier), action);
      }

      protected override void Cleanup()
      {
         TargetElement.PreviewMouseDown -= TargetElement_PreviewMouseDown;
      }

      protected void TargetElement_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         if (IsHorizontalNavigationKey(e.Key))
            MoveToFieldByKeyboard(e);
      }

      private void MoveToFieldByKeyboard(KeyEventArgs e)
      {
         log.DebugFormat("Beginning move on {0} using key \"{1}\"", e.Source, e.Key);
         switch (e.Key)
         {
            case Key.Tab:
               if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                  topContainerCurrentItemService.MoveLeft(1);
               else
                  topContainerCurrentItemService.MoveRight(1);
               e.Handled = true;
               break;

            case Key.Left:
               e.Handled = true;
               if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                  topContainerCurrentItemService.MoveToLeftMost();
               else if (Keyboard.Modifiers.HasFlag(ModifierKeys.None))
                  topContainerCurrentItemService.MoveLeft(1);
               else
                  e.Handled = false;
               break;

            case Key.Right:
               e.Handled = true;
               if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                  topContainerCurrentItemService.MoveToRightMost();
               else if (Keyboard.Modifiers.HasFlag(ModifierKeys.None))
                  topContainerCurrentItemService.MoveRight(1);
               else
                  e.Handled = false;
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
               bMoved = topContainerCurrentItemService.MoveUp(1);
               break;

            case Key.Down:
               bMoved = topContainerCurrentItemService.MoveDown(1);
               break;

            case Key.PageUp:
               bMoved = topContainerCurrentItemService.MoveUp((uint)DataGridProxy.RowsPerPage);
               break;

            case Key.PageDown:
               bMoved = topContainerCurrentItemService.MoveDown((uint)DataGridProxy.RowsPerPage);
               break;

            case Key.Home:
               bMoved = topContainerCurrentItemService.MoveToTop();
               break;

            case Key.End:
               bMoved = topContainerCurrentItemService.MoveToBottom();
               break;

            default:
               return;
         }
         eventArgs.Handled = true;
         DataGridProxy.ScrollIntoView(topContainerCurrentItemService.CurrentCell.Item);
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

         topContainerCurrentItemService.MoveTo(new UniversalCellInfo(clickedRow.Item, 0));
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
