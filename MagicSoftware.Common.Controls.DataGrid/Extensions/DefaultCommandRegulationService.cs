using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICommandRegulationService))]
   internal class DefaultCommandRegulationService : ICommandRegulationService, IUIService
   {
      private FrameworkElement element;

      HashSet<CanExecuteRoutedEventHandler> registeredCommandHandlers = new HashSet<CanExecuteRoutedEventHandler>();

      public event CanExecuteRoutedEventHandler PreviewCanExecute
      {
         add 
         { 
            CommandManager.AddPreviewCanExecuteHandler(element, value);
            registeredCommandHandlers.Add(value);
         }
         remove 
         {
            registeredCommandHandlers.Remove(value);
            CommandManager.RemovePreviewCanExecuteHandler(element, value); 
         }
      }

      public bool IsAttached
      {
         get { return element != null; }
      }

      public void AttachToElement(FrameworkElement element)
      {
         this.element = element;
      }

      public void DetachFromElement(FrameworkElement element)
      {
         var handlersReplica = new HashSet<CanExecuteRoutedEventHandler>(registeredCommandHandlers);
         foreach (var handler in handlersReplica)
         {
            PreviewCanExecute -= handler;
         }
         handlersReplica.Clear();

         element = null;
      }

      public void Dispose()
      {
         DetachFromElement(element);
      }

      public void ExecuteCommand(RoutedCommand command, object commandParameter)
      {
         element.Dispatcher.Invoke(DispatcherPriority.Input, new Action(() => 
         {
            int retries = 3;
            while(true)
            {
               try
               {
                  command.Execute(commandParameter, element);
                  return;
               }
               catch (Exception)
               {
                  retries--;
                  if (retries == 0)
                     throw;
                  System.Threading.Thread.Sleep(1);
               }
            }
         }));
      }
   }
}