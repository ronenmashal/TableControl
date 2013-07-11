using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using MagicSoftware.Common.Controls.Extensibility.Controls;

namespace MagicSoftware.Common.Controls.Proxies
{
   public abstract class ElementProxy : IDisposable
   {
      bool isDisposed = false;

      protected UIElement Element { get; private set; }

      HashSet<ProxyCommand> disabledCommands = new HashSet<ProxyCommand>();

      public ElementProxy(UIElement element)
      {
         Element = element;
      }

      public void Dispose()
      {
         if (!isDisposed)
         {
            DetachFromElement();
         }
         isDisposed = true;
      }

      protected abstract void DetachFromElement();

      public abstract DependencyProperty GetValueProperty();


      public DispatcherOperation InvokeOnRender(Delegate method, params object[] args)
      {
         return Element.Dispatcher.BeginInvoke(method, DispatcherPriority.Render, args);
      }

      public void DisableCommands(IEnumerable<ProxyCommand> commands)
      {
         foreach (var command in commands)
         {
            disabledCommands.Add(command);
         }
      }

      public void DisableCommands(params ProxyCommand[] commands)
      {
         DisableCommands((IEnumerable<ProxyCommand>)commands);
      }

      public void EnableCommands(params ProxyCommand[] commands)
      {
         disabledCommands.RemoveWhere((command) => commands.Contains(command));
      }

      protected bool IsCommandEnabled(ProxyCommand command)
      {
         return (!disabledCommands.Contains(command));
      }

      protected abstract bool TryExecuteCommand(ProxyCommand command);

      public bool ExecuteCommand(ProxyCommand command)
      {
         if (!IsCommandEnabled(command))
            return false;
            
         return TryExecuteCommand(command);
      }
   }
}
