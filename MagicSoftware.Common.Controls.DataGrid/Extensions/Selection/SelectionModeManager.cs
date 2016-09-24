using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Table.Utils;
using System.Windows.Threading;
using MagicSoftware.Common.Controls.Table.Models;
using System.Windows.Controls.Primitives;
using log4net;
using MagicSoftware.Common.Utils;
using LogLevel = log4net.Core.Level;

namespace MagicSoftware.Common.Controls.Table.Extensions.Selection
{
   internal class SelectionModeManager : IDisposable
   {
      public static readonly IdleSelectionMode IdleSelectionMode = new IdleSelectionMode();
      public static readonly MultiSelectionMode MultiSelectionMode = new MultiSelectionMode();
      public static readonly SingleSelectionMode SingleSelectionMode = new SingleSelectionMode();
      protected ILog log = log4net.LogManager.GetLogger(SelectionExtender.LoggerName);

      private SelectionMode currentSelectionMode;

      private SelectionViewManager selectionViewManager = null;

      public SelectionModeManager(MultiSelector element, ICurrentItemService currentItemTracker)
      {
         Element = element;
         CurrentItemTracker = currentItemTracker;
         CurrentItemTracker.CurrentChanged += CurrentItemTracker_CurrentChanged;

         Element.AddHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown), true);
         Element.AddHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp), true);
         Element.AddHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown), true);

         SingleSelectionMode.Initialize(element, currentItemTracker);
         MultiSelectionMode.Initialize(element, currentItemTracker);

         SetCurrentSelectionMode(SingleSelectionMode);
         currentSelectionMode.Enter();
      }

      protected ICurrentItemService CurrentItemTracker { get; private set; }

      protected MultiSelector Element { get; private set; }

      public static bool IsMultiSelectionKey(Key key)
      {
         switch (key)
         {
            case Key.LeftCtrl:
            case Key.RightCtrl:
            case Key.LeftShift:
            case Key.RightShift:
               return true;
         }
         return false;
      }

      public virtual void Dispose()
      {
         Element.RemoveHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown));
         Element.RemoveHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp));
         Element.RemoveHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown));
         CurrentItemTracker.CurrentChanged -= CurrentItemTracker_CurrentChanged;
         CurrentItemTracker = null;
      }

      public void SetSelectionView(SelectionView selectionView)
      {
         if (selectionViewManager != null)
            selectionViewManager.Dispose();

         if (selectionView == null)
            selectionViewManager = null;

         //SetCurrentSelectionMode(IdleSelectionMode);

         selectionViewManager = new SelectionViewManager(Element, selectionView);
         Element.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
         {
            selectionViewManager.SelectItemsOnElement();
         }));
      }

      protected void TargetElement_PreviewKeyUp(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      protected void TargetElement_PreviewMouseDown(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }

      private void CurrentItemTracker_CurrentChanged(object sender, EventArgs args)
      {
         currentSelectionMode.OnCurrentItemChanged();
         Element.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
         {
            if (selectionViewManager != null)
               selectionViewManager.UpdateViewFromElement();
         }));
      }

      private void SetCurrentSelectionMode(SelectionMode nextSelectionMode)
      {
         if (nextSelectionMode != currentSelectionMode)
         {
            if (currentSelectionMode != null)
               currentSelectionMode.Leave();
            currentSelectionMode = nextSelectionMode;
            currentSelectionMode.Enter();
         }
      }

      private void TargetElement_PreviewKeyDown(object sender, RoutedEventArgs e)
      {
         SetCurrentSelectionMode(currentSelectionMode.HandleInputEvent(sender, e as InputEventArgs));
      }
   }
}
