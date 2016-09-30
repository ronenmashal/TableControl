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

      public SelectionModeManager(FrameworkElement element)
      {
         ElementSelectionService = UIServiceProvider.GetService<IMultiSelectionService>(element);
         CurrentItemTracker = UIServiceProvider.GetService<ICurrentItemService>(element);

         CurrentItemTracker.CurrentChanged += CurrentItemTracker_CurrentChanged;

         //ElementSelectionService.AddHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown), true);
         //ElementSelectionService.AddHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp), true);
         //ElementSelectionService.AddHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown), true);

         SingleSelectionMode.Initialize(ElementSelectionService, CurrentItemTracker);
         MultiSelectionMode.Initialize(ElementSelectionService, CurrentItemTracker);

         SetCurrentSelectionMode(SingleSelectionMode);
         currentSelectionMode.Enter();
      }

      protected ICurrentItemService CurrentItemTracker { get; private set; }

      protected IMultiSelectionService ElementSelectionService { get; private set; }

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
         //ElementSelectionService.RemoveHandler(FrameworkElement.PreviewKeyDownEvent, new RoutedEventHandler(TargetElement_PreviewKeyDown));
         //ElementSelectionService.RemoveHandler(FrameworkElement.PreviewKeyUpEvent, new RoutedEventHandler(TargetElement_PreviewKeyUp));
         //ElementSelectionService.RemoveHandler(FrameworkElement.PreviewMouseDownEvent, new RoutedEventHandler(TargetElement_PreviewMouseDown));
         CurrentItemTracker.CurrentChanged -= CurrentItemTracker_CurrentChanged;
         CurrentItemTracker = null;
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
         bool bShift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
         bool bControl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;

         if (bShift || bControl)
            SetCurrentSelectionMode(MultiSelectionMode);
         else
            SetCurrentSelectionMode(SingleSelectionMode);

         currentSelectionMode.OnCurrentItemChanged();
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
