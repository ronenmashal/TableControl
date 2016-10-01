using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public abstract class InputGesturesFactory
   {
      private ModifierKeys[] modifierCombinations;

      public InputGesturesFactory(ModifierKeys[] modifierCombinations)
      {
         if (modifierCombinations == null || modifierCombinations.Length == 0)
            this.modifierCombinations = new ModifierKeys[] { ModifierKeys.None };
         else
            this.modifierCombinations = modifierCombinations;
      }

      public static ModifierKeys[] AllCombinationsOf(params ModifierKeys[] modifiers)
      {
         ModifierKeys[] combinations = new ModifierKeys[modifiers.Length * modifiers.Length];
         combinations[0] = ModifierKeys.None;
         int c = 1;
         for (int i = 0; i < modifiers.Length; i++)
         {
            combinations[c++] = modifiers[i];
            for (int j = i + 1; j < modifiers.Length; j++)
            {
               combinations[c++] = modifiers[i] | modifiers[j];
            }
         }
         return combinations;
      }

      public InputGesture[] GetGestures()
      {
         InputGesture[] gestures = new InputGesture[modifierCombinations.Length];
         int i = 0;
         foreach (var modifier in modifierCombinations)
            gestures[i++] = CreateGesture(modifier);
         return gestures;
      }

      protected abstract InputGesture CreateGesture(ModifierKeys modifiers);
   }

   public class InputService : IUIService
   {
      private static ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      #region Class Input Filter

      static Dictionary<Type, IInputFilter> typeFilters = new Dictionary<Type, IInputFilter>();

      public static void RegisterTypeInputFilter(Type type, IInputFilter inputFilter)
      {
         typeFilters.Add(type, inputFilter);
      }

      static IInputFilter GetTypeInputFilter(Type type)
      {
         IInputFilter filter = null;
         typeFilters.TryGetValue(type, out filter);
         return filter;
      }

      #endregion

      #region Input Filter attached property

      public static readonly DependencyProperty InputFilterProperty =
          DependencyProperty.RegisterAttached("InputFilter", typeof(IInputFilter), typeof(InputService), new UIPropertyMetadata(null));

      public static IInputFilter GetInputFilter(DependencyObject obj)
      {
         log.DebugFormat("Getting input filter on {0}", obj);
         return (IInputFilter)obj.GetValue(InputFilterProperty);
      }

      public static IInputFilter GetInputFilterOnPath(DependencyObject innermostElement, DependencyObject outermostElement)
      {
         log.DebugFormat("Searching for input filter on elements between {0} and {1}", innermostElement, outermostElement);

         JointInputFilter jointFilters = new JointInputFilter();

         var currentElement = innermostElement;
         IInputFilter filter;
         while (currentElement != null && currentElement != outermostElement)
         {
            filter = GetInputFilter(currentElement);
            if (filter != null)
               jointFilters.Add(currentElement, filter);

            filter = GetTypeInputFilter(currentElement.GetType());
            if (filter != null)
               jointFilters.Add(currentElement, filter);

            // Try to get the items control owning currentElement. If currentElement is an item container -
            // the result will be the owning items control. Otherwise, it will be null.
            var itemsControl = ItemsControl.ItemsControlFromItemContainer(currentElement);
            if (itemsControl != null)
               currentElement = itemsControl;
            else
               currentElement = VisualTreeHelper.GetParent(currentElement);
         }

         if (currentElement != null)
         {
            filter = GetInputFilter(currentElement);
            if (filter != null)
               jointFilters.Add(currentElement, filter);
            filter = GetTypeInputFilter(currentElement.GetType());
            if (filter != null)
               jointFilters.Add(currentElement, filter);
         }

         return jointFilters;
      }

      public static void SetInputFilter(DependencyObject obj, IInputFilter value)
      {
         obj.SetValue(InputFilterProperty, value);
      }

      #endregion Input Filter attached property

      private FrameworkElement element;
      private GesturesRegistrar registeredGestures = new GesturesRegistrar();

      public virtual bool IsAttached { get { return element != null; } }

      public void AttachToElement(FrameworkElement element)
      {
         this.element = element;
         element.PreviewKeyDown += element_PreviewKeyDown;
         element.PreviewMouseDown += element_PreviewMouseDown;
      }

      public void DetachFromElement(FrameworkElement element)
      {
         element.PreviewKeyDown -= element_PreviewKeyDown;
         element.PreviewMouseDown -= element_PreviewMouseDown;
         element = null;
      }

      public void Dispose()
      {
         if (element != null)
            DetachFromElement(element);
      }

      public void RegisterGestureAction(InputGesture gesture, Action<InputEventArgs> action)
      {
         registeredGestures.RegisterGestureAction(gesture, action);
      }

      public void RegisterKeyActionGestures(Action<KeyEventArgs> action, KeyGesturesFactory gesturesFactory)
      {
         foreach (KeyGesture gesture in gesturesFactory.GetGestures())
            RegisterKeyGestureAction(gesture, action);
      }

      public void RegisterKeyGestureAction(KeyGesture gesture, Action<KeyEventArgs> action)
      {
         RegisterGestureAction(gesture, (e) => { action((KeyEventArgs)e); });
      }

      public void RegisterMouseActionGestures(Action<MouseEventArgs> action, MouseGesturesFactory gesturesFactory)
      {
         foreach (MouseGesture gesture in gesturesFactory.GetGestures())
            RegisterMouseGestureAction(gesture, action);
      }

      public void RegisterMouseGestureAction(MouseGesture gesture, Action<MouseEventArgs> action)
      {
         RegisterGestureAction(gesture, (e) => { action((MouseEventArgs)e); });
      }

      public void UnregisterGestureAction(InputGesture gesture)
      {
         //TODO: Pass action to unregister.
         registeredGestures.UnregisterGesture(gesture);
      }

      private void element_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         var inputFilter = GetInputFilterOnPath(e.OriginalSource as FrameworkElement, sender as FrameworkElement);
         if (inputFilter != null)
         {
            if (inputFilter.ElementWillProcessInput(e))
               return;
         }

         var action = registeredGestures.GetActionForGesture(sender, e);
         if (action != null)
            action(e);
      }

      private void element_PreviewMouseDown(object sender, MouseEventArgs e)
      {
         var inputFilter = GetInputFilterOnPath(e.OriginalSource as FrameworkElement, sender as FrameworkElement);
         if (inputFilter != null)
         {
            if (inputFilter.ElementWillProcessInput(e))
               return;
         }

         var action = registeredGestures.GetActionForGesture(sender, e);
         if (action != null)
            action(e);
      }

      private class JointInputFilter : IInputFilter
      {
         private List<KeyValuePair<DependencyObject, IInputFilter>> jointFilters = new List<KeyValuePair<DependencyObject, IInputFilter>>();

         public void Add(DependencyObject obj, IInputFilter filter)
         {
            jointFilters.Add(new KeyValuePair<DependencyObject, IInputFilter>(obj, filter));
         }

         public bool ElementWillProcessInput(InputEventArgs args)
         {
            foreach (var filter in jointFilters)
            {
               var newArgs = CloneEventArgs(args, filter.Key);
               if (filter.Value.ElementWillProcessInput(newArgs))
                  return true;
            }
            return false;
         }

         private InputEventArgs CloneEventArgs(InputEventArgs args, DependencyObject newSource)
         {
            InputEventArgs clone = args;
            if (args is KeyEventArgs)
            {
               clone = CloneKeyEventArgs((KeyEventArgs)args, newSource);
            }
            return clone;
         }

         private KeyEventArgs CloneKeyEventArgs(KeyEventArgs args, DependencyObject newSource)
         {
            var clone = new KeyEventArgs((KeyboardDevice)args.Device, args.InputSource, args.Timestamp, args.Key)
            {
               RoutedEvent = args.RoutedEvent,
               Source = newSource
            };
            return clone;
         }

         private MouseEventArgs CloneMouseEventArgs(MouseEventArgs args, DependencyObject newSource)
         {
            var clone = new MouseEventArgs((MouseDevice)args.Device, args.Timestamp)
            {
               RoutedEvent = args.RoutedEvent,
               Source = newSource
            };
            return clone;
         }
      }

      class GesturesRegistrar
      {
         private Dictionary<InputGesture, List<PrioritizedAction>> registeredGestures = new Dictionary<InputGesture, List<PrioritizedAction>>();

         class PrioritizedAction
         {
            public PrioritizedAction (int priority, Action<InputEventArgs> action)
            {
               this.Priority = priority;
               this.Action = action;
            }

            public int Priority {get; private set;}
            public Action<InputEventArgs> Action {get; private set;}
         }

         public void RegisterGestureAction(InputGesture gesture, Action<InputEventArgs> action)
         {
            //TODO: Handle duplicities.
            //if (registeredGestures.ContainsKey(gesture))
               //throw new InvalidOperationException("Gesture " + gesture + " is already mapped to action " + registeredGestures[gesture]);

            if (!registeredGestures.ContainsKey(gesture))
            {
               registeredGestures.Add(gesture, new List<PrioritizedAction>());
            }
            var gestureActions = registeredGestures[gesture];
            gestureActions.Add(new PrioritizedAction(int.MaxValue, action));
            gestureActions.Sort((pa1, pa2) => pa1.Priority - pa2.Priority);
         }

         internal Action<InputEventArgs> GetActionForGesture(object sender, InputEventArgs e)
         {
            foreach (var mapping in registeredGestures)
            {
               if (mapping.Key.Matches(sender, e))
               {
                  var action = mapping.Value[0];
                  return action.Action;
               }
            }
            return null;
         }

         internal void UnregisterGesture(InputGesture gesture)
         {
            if (registeredGestures.ContainsKey(gesture))
               registeredGestures.Remove(gesture);
         }
      }
   }

   public class KeyGesturesFactory : InputGesturesFactory
   {
      private Key key;

      public KeyGesturesFactory(Key key, params ModifierKeys[] modifierCombinations)
         : base(modifierCombinations)
      {
         this.key = key;
      }

      protected override InputGesture CreateGesture(ModifierKeys modifiers)
      {
         return new KeyGesture(key, modifiers);
      }
   }

   public class MouseGesturesFactory : InputGesturesFactory
   {
      private MouseAction mouseAction;

      public MouseGesturesFactory(MouseAction mouseAction, params ModifierKeys[] modifierCombinations)
         : base(modifierCombinations)
      {
         this.mouseAction = mouseAction;
      }

      protected override InputGesture CreateGesture(ModifierKeys modifiers)
      {
         return new MouseGesture(mouseAction, modifiers);
      }
   }
}