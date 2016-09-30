using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using MagicSoftware.Common.Utils;
using LogLevel = log4net.Core.Level;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions.Selection
{
   internal abstract class SelectionMode
   {
      protected ILog log = log4net.LogManager.GetLogger(SelectionExtender.LoggerName);

      protected ICurrentItemService CurrentItemTracker { get; private set; }

      protected IMultiSelectionService ElementSelectionService { get; private set; }

      public abstract void Enter();

      public void Initialize(IMultiSelectionService element, ICurrentItemService currentItemTracker)
      {
         ElementSelectionService = element;
         CurrentItemTracker = currentItemTracker;
      }

      public abstract void Leave();

      public abstract void OnCurrentItemChanged();

      internal SelectionMode HandleInputEvent(object sender, InputEventArgs inputEventArgs)
      {
         if (inputEventArgs is KeyEventArgs)
         {
            return HandleKeyboardEvent(sender, inputEventArgs as KeyEventArgs);
         }
         else if (inputEventArgs is MouseButtonEventArgs)
         {
            return HandleMouseEvent(sender, inputEventArgs as MouseButtonEventArgs);
         }
         log.Debug("Don't know how to handle input event " + inputEventArgs);
         return this;
      }

      protected abstract SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs);

      protected abstract SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs);
   }
}
