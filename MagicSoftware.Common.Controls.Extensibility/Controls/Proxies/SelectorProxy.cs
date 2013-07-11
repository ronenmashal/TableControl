using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Extensibility.Controls;

namespace MagicSoftware.Common.Controls.Proxies
{
   public abstract class SelectorProxy : ElementProxy
   {
      const string ScrollPageUpCommandName = "SelectorProxy.PageUp";
      public static ProxyCommand ScrollPageUp = new ProxyCommand(ScrollPageUpCommandName);

      const string ScrollPageDownCommandName = "SelectorProxy.PageDown";
      public static ProxyCommand ScrollPageDown = new ProxyCommand(ScrollPageDownCommandName);

      const string MoveLeftCommandName = "SelectorProxy.MoveLeft";
      public static ProxyCommand MoveLeftCommand = new ProxyCommand(MoveLeftCommandName);

      const string MoveRightCommandName = "SelectorProxy.MoveRight";
      public static ProxyCommand MoveRightCommand = new ProxyCommand(MoveRightCommandName);

      public static ProxyCommand[] NavigationCommands = 
      {
         ScrollPageUp, ScrollPageDown
      };

      #region Fields

      ScrollViewer scrollViewer = null;

      #endregion

      #region Constructors

      public SelectorProxy(Selector selectorElement) :
         base(selectorElement)
      {

      }

      #endregion

      #region Properties
      
      private Selector Selector { get { return (Selector)Element; } }
      
      #endregion

      protected override void DetachFromElement()
      {

      }

      protected override bool TryExecuteCommand(Extensibility.Controls.ProxyCommand command)
      {
         switch (command.Name)
         {
            case ScrollPageUpCommandName:
               ScrollPage(FocusNavigationDirection.Up);
               break;
         }

         return true;
      }

      public override DependencyProperty GetValueProperty()
      {
         return Selector.SelectedValueProperty;
      }

      protected ScrollViewer ScrollViewer
      {
         get
         {
            if (scrollViewer == null)
            {
               scrollViewer = UIUtils.GetVisualChild<ScrollViewer>(Element);
            }
            return scrollViewer;
         }
      }

      public int TopMostRowIndex
      {
         get
         {
            if (ScrollViewer == null)
               return 0;
            return (int)ScrollViewer.VerticalOffset;
         }
      }

      public int RowsPerPage
      {
         get
         {
            if (ScrollViewer == null)
            {
               return -1;
            }
            return (int)ScrollViewer.ViewportHeight;
         }
      }

      public ItemCollection Items
      {
         get
         {
            return Selector.Items;
         }
      }

      public double ScrollPage(FocusNavigationDirection direction)
      {
         Debug.Assert(direction == FocusNavigationDirection.Up || direction == FocusNavigationDirection.Down);
         if (ScrollViewer != null)
         {
            if (direction == FocusNavigationDirection.Up)
               ScrollViewer.PageUp();
            else
               ScrollViewer.PageDown();
            return ScrollViewer.VerticalOffset;
         }
         return 0;
      }

      private bool IsCurrentItemOnScreen
      {
         get
         {
            if (Selector.Items.CurrentItem == null)
               return false;

            int currentItemIndex = Selector.Items.CurrentPosition;
            if (ScrollViewer == null)
               return true;

            if (currentItemIndex < (int)ScrollViewer.VerticalOffset)
               return false;
            if (currentItemIndex > (int)(ScrollViewer.VerticalOffset + ScrollViewer.ViewportHeight))
               return false;
            return true;
         }
      }

      public void EnsureCurrentItemIsVisible()
      {
         if (Selector.Items.CurrentItem != null && !IsCurrentItemOnScreen)
            ScrollIntoView(Selector.Items.CurrentItem);
      }

      protected abstract void ScrollIntoView(object item);

   }
}
