using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using log4net.Core;
using MagicSoftware.Common.Utils;

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
         ModifierKeys[] combinations = new ModifierKeys[(int)Math.Pow(2, modifiers.Length)];
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
      public static readonly DependencyProperty InputFilterProperty =
          DependencyProperty.RegisterAttached("InputFilter", typeof(IInputFilter), typeof(InputService), new UIPropertyMetadata(null));

      private static ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      private static Dictionary<Type, IInputFilter> typeFilters = new Dictionary<Type, IInputFilter>();

      private FrameworkElement element;

      private GesturesRegistrar registeredGestures = new GesturesRegistrar();

      public virtual bool IsAttached { get { return element != null; } }

      public bool PropogateUnhandledKeyGestures { get; set; }

      public bool PropogateUnhandledMouseGestures { get; set; }

      public static IInputFilter GetInputFilter(DependencyObject obj)
      {
         log.LogMessage(Level.Finer, "Getting input filter on {0}", obj);
         return (IInputFilter)obj.GetValue(InputFilterProperty);
      }

      public static IInputFilter GetInputFilterOnPath(DependencyObject innermostElement, DependencyObject outermostElement)
      {
         log.LogMessage(Level.Finer, "Searching for input filter on elements between {0} and {1}", innermostElement, outermostElement);

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

      public static void RegisterTypeInputFilter(Type type, IInputFilter inputFilter)
      {
         typeFilters.Add(type, inputFilter);
      }

      public static void SetInputFilter(DependencyObject obj, IInputFilter value)
      {
         obj.SetValue(InputFilterProperty, value);
      }

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
         using (registeredGestures.IgnoreFrameInRegistrationTrace())
            registeredGestures.RegisterGestureAction(gesture, action);
      }

      public void RegisterKeyActionGestures(Action<KeyEventArgs> action, KeyGesturesFactory gesturesFactory)
      {
         using (registeredGestures.IgnoreFrameInRegistrationTrace())
            foreach (KeyGesture gesture in gesturesFactory.GetGestures())
               RegisterKeyGestureAction(gesture, action);
      }

      public void RegisterKeyGestureAction(KeyGesture gesture, Action<KeyEventArgs> action)
      {
         using (registeredGestures.IgnoreFrameInRegistrationTrace())
            RegisterGestureAction(gesture, (e) => { action((KeyEventArgs)e); });
      }

      public void RegisterMouseActionGestures(Action<MouseEventArgs> action, MouseGesturesFactory gesturesFactory)
      {
         using (registeredGestures.IgnoreFrameInRegistrationTrace())
            foreach (MouseGesture gesture in gesturesFactory.GetGestures())
               RegisterMouseGestureAction(gesture, action);
      }

      public void RegisterMouseGestureAction(MouseGesture gesture, Action<MouseEventArgs> action)
      {
         using (registeredGestures.IgnoreFrameInRegistrationTrace())
            RegisterGestureAction(gesture, (e) => { action((MouseEventArgs)e); });
      }

      public void UnregisterGestureAction(InputGesture gesture)
      {
         //TODO: Pass action to unregister.
         registeredGestures.UnregisterGesture(gesture);
      }

      private static IInputFilter GetTypeInputFilter(Type type)
      {
         IInputFilter filter = null;
         typeFilters.TryGetValue(type, out filter);
         return filter;
      }

      private void ProcessInputEvent(object sender, InputEventArgs e, bool propogateUnhandledGesture)
      {
         var inputFilter = GetInputFilterOnPath(e.OriginalSource as FrameworkElement, sender as FrameworkElement);
         if (inputFilter != null)
         {
            if (inputFilter.ElementWillProcessInput(e))
               return;
         }

         var actions = registeredGestures.GetActionsForGesture(sender, e);
         if (actions != null)
         {
            foreach (var action in actions)
            {
               action(e);
               if (e.Handled)
                  return;
            }
         }
         else
            e.Handled = !propogateUnhandledGesture;
      }

      private void element_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         ProcessInputEvent(sender, e, PropogateUnhandledKeyGestures);
      }

      private void element_PreviewMouseDown(object sender, MouseEventArgs e)
      {
         ProcessInputEvent(sender, e, PropogateUnhandledMouseGestures);
      }

      private class GesturesRegistrar
      {
         private readonly DisposableCounter ignoreStackFramesCounter = new DisposableCounter(1);
         private Dictionary<InputGesture, List<PrioritizedAction>> registeredGestures = new Dictionary<InputGesture, List<PrioritizedAction>>(new GestureComparer());

         private class GestureComparer : IEqualityComparer<InputGesture>
         {

            public bool Equals(InputGesture x, InputGesture y)
            {
               if (ReferenceEquals(x, y))
                  return true;

               if (x == null || y == null)
                  return false;

               if (x.GetType() != y.GetType())
                  return false;

               if (x is KeyGesture)
                  return Equals((KeyGesture) x, (KeyGesture) y);
               
               if (x is MouseGesture)
                  return Equals((MouseGesture)x, (MouseGesture)y);
               
               throw new ArgumentException("Don't know how to compare " + x.GetType().Name);
            }

            public int GetHashCode(InputGesture obj)
            {
               var hash = obj.GetType().GetHashCode();
               if (obj is KeyGesture)
                  hash |= GetHashCode((KeyGesture) obj);
               else if (obj is MouseGesture)
                  hash |= GetHashCode((MouseGesture) obj);
               else
                  throw new ArgumentException("Don't know how to hash " + obj.GetType().Name);

               return hash;
            }

            int GetHashCode(KeyGesture gesture)
            {
               return gesture.Key.GetHashCode() * (int)Math.Pow(gesture.Modifiers.GetHashCode(), 2);
            }

            int GetHashCode(MouseGesture gesture)
            {
               return gesture.MouseAction.GetHashCode()*(int) Math.Pow(gesture.Modifiers.GetHashCode(), 2);
            }

            bool Equals(KeyGesture x, KeyGesture y)
            {
               return x.Modifiers == y.Modifiers && x.Key == y.Key;
            }

            bool Equals(MouseGesture x, MouseGesture y)
            {
               return x.Modifiers == y.Modifiers && x.MouseAction == y.MouseAction;
            }
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

            var trace = new StackTrace(ignoreStackFramesCounter.Value);
            var callerFrame = trace.GetFrame(0);
            var gestureActions = registeredGestures[gesture];
            gestureActions.Add(new PrioritizedAction(int.MaxValue, action, callerFrame));
            gestureActions.Sort((pa1, pa2) => pa1.Priority - pa2.Priority);
         }

         internal IEnumerable<Action<InputEventArgs>> GetActionsForGesture(object sender, InputEventArgs e)
         {
            foreach (var mapping in registeredGestures)
            {
               if (mapping.Key.Matches(sender, e))
               {
                  var actions = mapping.Value;
                  //log.DebugFormat("Found action registered by {0}", actions.RegistrationStackTrace);
                  return actions.Select(pa => pa.Action).ToList();
               }
            }
            return null;
         }

         internal IDisposable IgnoreFrameInRegistrationTrace()
         {
            return ignoreStackFramesCounter.Increase();
         }

         internal void UnregisterGesture(InputGesture gesture)
         {
            if (registeredGestures.ContainsKey(gesture))
               registeredGestures.Remove(gesture);
         }

         private class DisposableCounter
         {
            private int counterValue;

            public int Value { get { return counterValue; } }

            public DisposableCounter(int initialValue = 0)
            {
               counterValue = initialValue;
            }

            public IDisposable Increase()
            {
               counterValue++;
               return new DisposalActionCaller(() => counterValue--);
            }
         }

         private class PrioritizedAction
         {
            public Action<InputEventArgs> Action { get; private set; }

            public int Priority { get; private set; }

            public string RegistrationStackTrace { get
            {
               return string.Format("{0}.{1}", callerFrame.GetMethod().DeclaringType.Name, callerFrame.GetMethod().Name);
            } }

            private StackFrame callerFrame;

            public PrioritizedAction(int priority, Action<InputEventArgs> action, StackFrame callerFrame)
            {
               Priority = priority;
               Action = action;
               this.callerFrame = callerFrame;
            }
         }
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
   }

   public class InputServiceFactory : IUIServiceFactory
   {
      public bool BlockUnhandledMouseGestures { get; set; }

      public IUIService CreateUIService()
      {
         return new InputService()
         {
            PropogateUnhandledMouseGestures = !BlockUnhandledMouseGestures,
            PropogateUnhandledKeyGestures = true
         };
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