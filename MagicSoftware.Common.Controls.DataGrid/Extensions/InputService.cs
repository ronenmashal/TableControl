using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class InputService : IUIService
   {
      private FrameworkElement element;
      private Dictionary<InputGesture, Action<InputEventArgs>> registeredGestures = new Dictionary<InputGesture, Action<InputEventArgs>>();

      public void AttachToElement(FrameworkElement element)
      {
         this.element = element;
         element.PreviewKeyDown += element_PreviewKeyDown;
      }

      public void DetachFromElement(FrameworkElement element)
      {
         element.PreviewKeyDown -= element_PreviewKeyDown;
         element = null;
      }

      public void Dispose()
      {
         if (element != null)
            DetachFromElement(element);
      }

      public void RegisterGestureAction(InputGesture gesture, Action<InputEventArgs> action)
      {
         if (registeredGestures.ContainsKey(gesture))
            throw new InvalidOperationException("Gesture " + gesture + " is already mapped to action " + registeredGestures[gesture]);

         registeredGestures.Add(gesture, action);
      }

      public void RegisterKeyGestureAction(KeyGesture gesture, Action<KeyEventArgs> action)
      {
         RegisterGestureAction(gesture, (e) => { action((KeyEventArgs)e); });
      }

      private void element_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         foreach (var mapping in registeredGestures)
         {
            if (mapping.Key.Matches(sender, e))
               mapping.Value(e);
         }
      }
   }
}