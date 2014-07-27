﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

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
         foreach (var handler in registeredCommandHandlers)
         {
            PreviewCanExecute -= handler;
         }

         element = null;
      }

      public void Dispose()
      {
         DetachFromElement(element);
      }

      public void ExecuteCommand(RoutedCommand command, object commandParameter)
      {
         command.Execute(commandParameter, element);
      }
   }
}