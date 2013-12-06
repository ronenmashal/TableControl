using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MagicSoftware.Common.Utils;
using System.Diagnostics;
using MagicSoftware.Common.Controls.ProxiesX;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   public class SelectorKeyboardHandlerBase : KeyboardHandlerBase
   {
      SelectorProxy SelectorProxy { get { return (SelectorProxy)Proxy; } }

      protected override void HandlePreviewKeyDown(object sender, KeyEventArgs e)
      {
         Trace.WriteLine(String.Format("Preview key down on {0}: {1}, {2}", sender, e.Key, e.OriginalSource));

         if (e.Key.IsPrintable() && log.IsDebugEnabled)
         {
            log.DebugFormat("Preview key down on: {0}", e.OriginalSource);
         }

         DataGridRow currentRow = UIUtils.GetAncestor<DataGridRow>(Keyboard.FocusedElement as Visual);

         if (currentRow != null)
         {
            switch (e.Key)
            {
               case Key.Tab:
                  // Ensure the current item can actually be seen.
                  SelectorProxy.EnsureCurrentItemIsVisible();
                  OnPreviewTABKeyDown(e);
                  break;

               case Key.Up:
                  OnPreviewUPKeyDown(e);
                  break;

               case Key.Down:
                  OnPreviewDOWNKeyDown(e);
                  break;

               case Key.Home:
                  OnPreviewHOMEKeyDown(e);
                  break;

               case Key.End:
                  OnPreviewENDKeyDown(e);
                  break;

               case Key.PageDown:
                  OnPreviewPAGEDOWNKeyDown(e);
                  break;

               case Key.PageUp:
                  OnPreviewPAGEUPKeyDown(e);
                  break;

               default:
                  //canBeginEditing = false;
                  break;
            }
            //if (!e.Handled)
            //   base.OnPreviewKeyDown(e);
         }
      }

      /// <summary>
      /// Handle the Down key --> Move to the next item.
      /// </summary>
      /// <param name="e"></param>
      protected virtual void OnPreviewDOWNKeyDown(KeyEventArgs e)
      {
            //SelectorProxy.MoveCurrentToNext();
      }

      /// <summary>
      /// Handle the Up key --> Move to the previous item.
      /// </summary>
      /// <param name="e"></param>
      protected virtual void OnPreviewUPKeyDown(KeyEventArgs e)
      {
            //SelectorProxy.MoveCurrentToPrevious();
      }

      /// <summary>
      /// TAB at end of line or SHIFT-TAB at beginning of line should remain on same line
      /// </summary>
      /// <param name="e"></param>
      protected virtual void OnPreviewTABKeyDown(KeyEventArgs e)
      {
         Boolean shiftIsPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
         Boolean controlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
         if (controlPressed)
         {
            log.Debug("Control is pressed --> Not handling TAB key.");
            return;
         }

         //SelectorProxy.MoveToCell(shiftIsPressed ? FocusNavigationDirection.Left : FocusNavigationDirection.Right);

         //e.Handled = true;
      }

      /// <summary>
      /// ENTER should change edit state
      /// </summary>
      /// <param name="e"></param>
      //protected virtual void OnPreviewENTERKeyDown(KeyEventArgs e)
      //{
      //   if (SelectorProxy.IsReadOnly)
      //      return;

      //   // switch the edit state
      //   if (SelectorProxy.IsInEdit)
      //   {
      //      SelectorProxy.CommitEdit(DataGridEditingUnit.Cell, true);
      //   }
      //   else
      //   {
      //      SelectorProxy.BeginEdit();
      //   }
      //   e.Handled = true;
      //}

      /// <summary>
      /// CTRL + Home - move to 1st line
      /// </summary>
      protected virtual void OnPreviewHOMEKeyDown(KeyEventArgs e)
      {
         // exit the edit state, allows the move to previous/next line
         if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
         {
            SelectorProxy.Items.MoveCurrentToFirst();
            ///DataGridProxy.MoveToFirstLine();
            e.Handled = true;
         }
      }

      /// <summary>
      /// CTRL + End - move to last line.
      /// </summary>
      protected virtual void OnPreviewENDKeyDown(KeyEventArgs e)
      {
         // exit the edit state, allows the move to previous/next line
         if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
         {
            SelectorProxy.Items.MoveCurrentToLast();
            e.Handled = true;
         }
      }

      /// <summary>
      /// Overrides the Grid's Page Down behavior, in order to ensure that the focus will be set
      /// correctly on rows whose style is other than the default grid style.
      /// </summary>
      /// <param name="e"></param>
      protected virtual void OnPreviewPAGEDOWNKeyDown(KeyEventArgs e)
      {
         // Ensure the data-grid has a scroll bar.
         if (SelectorProxy.RowsPerPage > -1)
         {
            int currentRowIndex = SelectorProxy.Items.CurrentPosition;
            int currentRowViewOffset = currentRowIndex - SelectorProxy.TopMostRowIndex;

            //SelectorProxy.ScrollPage(FocusNavigationDirection.Down);
            if (SelectorProxy.ExecuteCommand(SelectorProxy.ScrollPageDown))
               SelectorProxy.InvokeOnRender(new Action(() =>
               {
                  // Calculate the row to go to. 
                  //int nextRow = DataGridProxy.TopMostRowIndex + currentRowViewOffset;
                  int nextRow = currentRowIndex + SelectorProxy.RowsPerPage;
                  // Ensure the row is within the data items' range.
                  if (nextRow >= SelectorProxy.Items.Count)
                  {
                     nextRow = SelectorProxy.Items.Count - 1;
                  }
                  // Move the grid to the calculated row index.
                  SelectorProxy.Items.MoveCurrentToPosition(nextRow);
               }));


            e.Handled = true;
         }
      }

      /// <summary>
      /// Overrides the Grid's Page Up behavior, in order to ensure that the focus will be set
      /// correctly on rows whose style is other than the default grid style.
      /// </summary>
      /// <param name="e"></param>
      protected virtual void OnPreviewPAGEUPKeyDown(KeyEventArgs e)
      {
         // Ensure the data-grid has a scroll bar.
         if (SelectorProxy.RowsPerPage > -1)
         {
            int currentRowIndex = SelectorProxy.Items.CurrentPosition;
            int currentRowViewOffset = currentRowIndex - SelectorProxy.TopMostRowIndex;

            //SelectorProxy.ScrollPage(FocusNavigationDirection.Up);
            if (SelectorProxy.ExecuteCommand(SelectorProxy.ScrollPageUp))
               SelectorProxy.InvokeOnRender(new Action(() =>
               {
                  // Calculate the row to go to. 
                  //int nextRow = DataGridProxy.TopMostRowIndex + currentRowViewOffset;
                  int nextRow = currentRowIndex - SelectorProxy.RowsPerPage;

                  // Ensure the row is within the data items' range.
                  if (nextRow < 0)
                  {
                     nextRow = 0;
                  }

                  // Move the grid to the calculated row index.
                  SelectorProxy.Items.MoveCurrentToPosition(nextRow);
               }));

            e.Handled = true;
         }
      }


   }
}
