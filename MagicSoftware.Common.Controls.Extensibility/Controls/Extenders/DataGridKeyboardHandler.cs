using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;
using log4net;
using LogLevel = log4net.Core.Level;
using System.Windows;
using System.Windows.Media;
using MagicSoftware.Common.Utils;
using System.ComponentModel;
using MagicSoftware.Common.Controls.ProxiesX;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   public class DataGridKeyboardHandler : SelectorKeyboardHandlerBase
   {
      DataGridProxy DataGridProxy { get { return (DataGridProxy)Proxy; } }

      public DataGridKeyboardHandler()
      {
         log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      }

      protected override void RegisterKeyboardEvents(UIElement element)
      {
         Debug.Assert(element is System.Windows.Controls.DataGrid);
         base.RegisterKeyboardEvents(element);
         //element.KeyDown += new KeyEventHandler(element_KeyDown);
         element.AddHandler(FrameworkElement.KeyDownEvent, new RoutedEventHandler(element_KeyDown), true);
      }

      protected override void UnregisterKeyboardEvents(UIElement element)
      {
         base.UnregisterKeyboardEvents(element);
         element.RemoveHandler(FrameworkElement.KeyDownEvent, new RoutedEventHandler(element_KeyDown));
      }

      void element_KeyDown(object sender, RoutedEventArgs e)
      {
         EndMove();
      }

      protected override void HandlePreviewKeyDown(object sender, KeyEventArgs e)
      {
         log.DebugFormat("Preview key down on {0}: {1}, {2}", sender, e.Key, e.OriginalSource);

         switch (e.Key)
         {
            case Key.Tab:
               if (DataGridProxy.IsInEdit)
               {
                  if (!DataGridProxy.CommitEdit(DataGridEditingUnit.Cell, false))
                     e.Handled = true;
               }
               break;

            default:
               base.HandlePreviewKeyDown(sender, e);
               break;
         }
      }

      object itemBeforeMoving;
      void BeginMove()
      {
         itemBeforeMoving = this.DataGridProxy.CurrentItem;
      }

      void EndMove()
      {
         object currentItem = this.DataGridProxy.CurrentItem;
         if (object.Equals(currentItem, itemBeforeMoving))
         {
            // Failed to move.
            DataGridProxy.CurrentItem = DataGridProxy.SelectedItem;
         }
      }

      //   ///// <summary>
      //   ///// handle keyboard events, implement non-standard navigation
      //   ///// </summary>
      //   ///// <param name="e"></param>
      //   //protected override void OnPreviewKeyDown(KeyEventArgs e)
      //   //{

      //   if (e.Key.IsPrintable() && log.IsDebugEnabled)
      //   {
      //      log.DebugFormat("Preview key down on: {0}", e.OriginalSource);
      //   }

      //   DataGridRow currentRow = UIUtils.GetAncestor<DataGridRow>(Keyboard.FocusedElement as Visual);

      //   if (currentRow != null)
      //   {
      //      switch (e.Key)
      //      {
      //         case Key.Tab:
      //            // Ensure the current item can actually be seen.
      //            DataGridProxy.EnsureCurrentItemIsVisible();
      //            OnPreviewTABKeyDown(e);
      //            break;

      //         case Key.F2:
      //            if (!DataGridProxy.IsInEdit)
      //            {
      //               DataGridProxy.BeginEdit();
      //            }
      //            break;

      //         case Key.Enter:
      //            OnPreviewENTERKeyDown(e);
      //            break;

      //         case Key.Up:
      //            OnPreviewUPKeyDown(e);
      //            break;

      //         case Key.Down:
      //            OnPreviewDOWNKeyDown(e);
      //            break;

      //         case Key.Left:
      //            OnPreviewLEFTKeyDown(e);
      //            break;

      //         case Key.Right:
      //            OnPreviewRIGHTKeyDown(e);
      //            break;

      //         case Key.Home:
      //            OnPreviewHOMEKeyDown(e);
      //            break;

      //         case Key.End:
      //            OnPreviewENDKeyDown(e);
      //            break;

      //         case Key.PageDown:
      //            OnPreviewPAGEDOWNKeyDown(e);
      //            break;

      //         case Key.PageUp:
      //            OnPreviewPAGEUPKeyDown(e);
      //            break;

      //         default:
      //            //canBeginEditing = false;
      //            break;
      //      }
      //      //if (!e.Handled)
      //      //   base.OnPreviewKeyDown(e);
      //   }
      //}

      /// <summary>
      /// text input preview - stop the processing of characters if not in edit mode
      /// </summary>
      /// <param name="e"></param>
      //protected override void OnPreviewTextInput(TextCompositionEventArgs e)
      //{
      //   // We don't want to enter edit mode for ordinary characters
      //   // TODO - activate incremental locate from here
      //   if (!DataGridProxy.IsInEdit)
      //   {
      //      log.Debug("Blocking text input because DataGrid is not in edit mode.");
      //      e.Handled = true;
      //   }
      //}

      /// <summary>
      /// DOWN arrow should move to next row even if in edit state
      /// DOWN arrow should also move to headers
      /// CTRL + DOWN - move to next folder
      /// </summary>
      /// <param name="e"></param>
      //protected override void OnPreviewDOWNKeyDown(KeyEventArgs e)
      //{
      //   if (!DataGridProxy.CommitEdit(DataGridEditingUnit.Row, true))
      //      e.Handled = true;
      //   if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      //   {
      //      //if (GroupsNavigator != null)
      //      //{
      //      //   GroupsNavigator.MoveCurrentToNextGroup(Items);
      //      //}
      //      e.Handled = true;
      //   }
      //   else
      //   {
      //      if (DataGridProxy.Items.CurrentPosition < DataGridProxy.Items.Count - 1)
      //         DataGridProxy.Items.MoveCurrentToNext();
      //      e.Handled = true;
      //   }
      //}

      /// <summary>
      /// UP arrow should move to previous row even if in edit state
      /// UP arrow should also move to headers
      /// CTRL + UP - move to preceding header 
      /// </summary>
      /// <param name="e"></param>
      //protected override void OnPreviewUPKeyDown(KeyEventArgs e)
      //{
      //   base.OnPreviewDOWNKeyDown(e);
      //   if (!DataGridProxy.CommitEdit(DataGridEditingUnit.Row, true))
      //      e.Handled = true;
      //   else
      //      if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      //      {
      //         //if (GroupsNavigator != null)
      //         //{
      //         //   GroupsNavigator.MoveCurrentToPreviousGroup(Items);
      //         //}
      //         e.Handled = true;
      //      }
      //      else
      //      {
      //         if (DataGridProxy.Items.CurrentPosition > 0)
      //            DataGridProxy.Items.MoveCurrentToPrevious();
      //         e.Handled = true;
      //      }
      //}

      ///// <summary>
      ///// TAB at end of line or SHIFT-TAB at beginning of line should remain on same line
      ///// </summary>
      ///// <param name="e"></param>
      //protected virtual void OnPreviewTABKeyDown(KeyEventArgs e)
      //{
      //   Boolean shiftIsPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
      //   Boolean controlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
      //   if (controlPressed)
      //   {
      //      log.Debug("Control is pressed --> Not handling TAB key.");
      //      return;
      //   }

      //   DataGridProxy.MoveToCell(shiftIsPressed ? FocusNavigationDirection.Left : FocusNavigationDirection.Right);

      //   e.Handled = true;
      //}

      ///// <summary>
      ///// ENTER should change edit state
      ///// </summary>
      ///// <param name="e"></param>
      //protected virtual void OnPreviewENTERKeyDown(KeyEventArgs e)
      //{
      //   if (DataGridProxy.IsReadOnly)
      //      return;

      //   // switch the edit state
      //   if (DataGridProxy.IsInEdit)
      //   {
      //      DataGridProxy.CommitEdit(DataGridEditingUnit.Cell, true);
      //   }
      //   else
      //   {
      //      DataGridProxy.BeginEdit();
      //   }
      //   e.Handled = true;
      //}

      ///// <summary>
      ///// CTRL + Home - exit edit mode, move to 1st header
      ///// </summary>
      //protected virtual void OnPreviewHOMEKeyDown(KeyEventArgs e)
      //{
      //   // exit the edit state, allows the move to previous/next line
      //   if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      //   {
      //      DataGridProxy.Items.MoveCurrentToFirst();
      //      ///DataGridProxy.MoveToFirstLine();
      //      e.Handled = true;
      //   }
      //   else if (Keyboard.Modifiers == ModifierKeys.None && !DataGridProxy.IsInEdit)
      //   {
      //      DataGridProxy.MoveToCell(FocusNavigationDirection.First);
      //      e.Handled = true;
      //   }
      //}

      ///// <summary>
      ///// CTRL + End - exit edit mode, allow the move to start/end
      ///// </summary>
      //protected virtual void OnPreviewENDKeyDown(KeyEventArgs e)
      //{
      //   // exit the edit state, allows the move to previous/next line
      //   if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      //   {
      //      DataGridProxy.Items.MoveCurrentToLast();
      //      e.Handled = true;
      //   }
      //   else if (Keyboard.Modifiers == ModifierKeys.None && !DataGridProxy.IsInEdit)
      //   {
      //      DataGridProxy.MoveToCell(FocusNavigationDirection.Last);
      //      e.Handled = true;
      //   }
      //}

      ///// <summary>
      ///// Overrides the Grid's Page Down behavior, in order to ensure that the focus will be set
      ///// correctly on rows whose style is other than the default grid style.
      ///// </summary>
      ///// <param name="e"></param>
      //protected virtual void OnPreviewPAGEDOWNKeyDown(KeyEventArgs e)
      //{
      //   // Ensure the data-grid has a scroll bar.
      //   if (DataGridProxy.RowsPerPage > -1)
      //   {
      //      int currentRowIndex = DataGridProxy.Items.CurrentPosition;
      //      int currentRowViewOffset = currentRowIndex - DataGridProxy.TopMostRowIndex;

      //      DataGridProxy.ScrollPage(FocusNavigationDirection.Down);
      //      DataGridProxy.InvokeOnRender(new Action(() =>
      //      {
      //         // Calculate the row to go to. 
      //         //int nextRow = DataGridProxy.TopMostRowIndex + currentRowViewOffset;
      //         int nextRow = currentRowIndex + DataGridProxy.RowsPerPage;
      //         // Ensure the row is within the data items' range.
      //         if (nextRow >= DataGridProxy.Items.Count)
      //         {
      //            nextRow = DataGridProxy.Items.Count - 1;
      //         }
      //         // Move the grid to the calculated row index.
      //         DataGridProxy.Items.MoveCurrentToPosition(nextRow);
      //      }));


      //      e.Handled = true;
      //   }
      //}

      ///// <summary>
      ///// Overrides the Grid's Page Up behavior, in order to ensure that the focus will be set
      ///// correctly on rows whose style is other than the default grid style.
      ///// </summary>
      ///// <param name="e"></param>
      //protected virtual void OnPreviewPAGEUPKeyDown(KeyEventArgs e)
      //{
      //   // Ensure the data-grid has a scroll bar.
      //   if (DataGridProxy.RowsPerPage > -1)
      //   {
      //      int currentRowIndex = DataGridProxy.Items.CurrentPosition;
      //      int currentRowViewOffset = currentRowIndex - DataGridProxy.TopMostRowIndex;

      //      DataGridProxy.ScrollPage(FocusNavigationDirection.Up);
      //      DataGridProxy.InvokeOnRender(new Action(() =>
      //      {
      //         // Calculate the row to go to. 
      //         //int nextRow = DataGridProxy.TopMostRowIndex + currentRowViewOffset;
      //         int nextRow = currentRowIndex - DataGridProxy.RowsPerPage;

      //         // Ensure the row is within the data items' range.
      //         if (nextRow < 0)
      //         {
      //            nextRow = 0;
      //         }

      //         // Move the grid to the calculated row index.
      //         DataGridProxy.Items.MoveCurrentToPosition(nextRow);
      //      }));

      //      e.Handled = true;
      //   }
      //}


      ///// <summary>
      ///// CTRL + RIGHT should move to end of row
      ///// </summary>
      ///// <param name="e"></param>
      //protected virtual void OnPreviewRIGHTKeyDown(KeyEventArgs e)
      //{
      //   FocusNavigationDirection navigationDirection = FocusNavigationDirection.Right;
      //   bool bMove = !DataGridProxy.IsInEdit;

      //   if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      //   {
      //      navigationDirection = FocusNavigationDirection.Last;
      //      bMove = true;
      //   }

      //   if (bMove)
      //   {
      //      using (DataGridProxy.PreserveEditState())
      //      {
      //         //   // Get the column with the correct display index.
      //         //   CurrentColumn = Columns.First(c => c.DisplayIndex == nextColumnDisplayIndex);
      //         //CurrentRow.FocusManager.MoveFocus(navigationDirection);
      //      };

      //      e.Handled = true;
      //   }
      //}

      ///// <summary>
      ///// CTRL + LEFT should move to start of row
      ///// </summary>
      ///// <param name="e"></param>
      //protected virtual void OnPreviewLEFTKeyDown(KeyEventArgs e)
      //{
      //   FocusNavigationDirection navigationDirection = FocusNavigationDirection.Left;
      //   bool bMove = !DataGridProxy.IsInEdit;

      //   if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      //   {
      //      navigationDirection = FocusNavigationDirection.First;
      //      bMove = true;
      //   }

      //   if (bMove)
      //   {
      //      using (DataGridProxy.PreserveEditState())
      //      {
      //         //   // Get the column with the correct display index.
      //         //   CurrentColumn = Columns.First(c => c.DisplayIndex == nextColumnDisplayIndex);
      //         ///CurrentRow.FocusManager.MoveFocus(navigationDirection);
      //      };

      //      e.Handled = true;
      //   }
      //}

      /// <summary>
      /// Overridden to control the table's edit state and movement.
      /// </summary>
      /// <param name="e">Key event arguments.</param>
      protected void OnKeyDown(KeyEventArgs e)
      {
         switch (e.Key)
         {
            case Key.Home:
            case Key.End:
               bool bBlockKey = (Keyboard.Modifiers == ModifierKeys.Shift) || (Keyboard.Modifiers == ModifierKeys.None);
               // If Shift-Home or Shift-End was not handled by the editing control
               // prevent the table from handling it, because it does it the wrong way.
               if (DataGridProxy.IsInEdit && bBlockKey)
                  e.Handled = true;
               break;
         }
      }

   }
}
