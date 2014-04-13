using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using log4net;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   ///
   /// </summary>
   /// <example>
   /// var service = UIServiceProvider.GetServiceProvider(element).GetService(serviceType);
   /// </example>
   public class UIServiceProvider : IWeakEventListener
   {
      #region Static members

      /// <summary>
      /// Defines the list of services assigned to an element.
      /// </summary>
      public static readonly DependencyProperty ServiceListProperty =
          DependencyProperty.RegisterAttached("ServiceList", typeof(IEnumerable<IUIService>), typeof(UIServiceProvider), new UIPropertyMetadata(new List<IUIService>(), OnServiceListChanged));

      private static readonly DependencyProperty ServiceProviderProperty =
          DependencyProperty.RegisterAttached("ServiceProvider", typeof(UIServiceProvider), typeof(UIServiceProvider), new UIPropertyMetadata(null));

      public static IUIService GetService(FrameworkElement element, Type serviceType)
      {
         IUIService service = null;
         var serviceProvider = GetServiceProvider(element);
         if (serviceProvider == null)
         {
            var serviceList = GetServiceList(element);
            if (serviceList != null)
            {
               serviceProvider = new UIServiceProvider();
               serviceProvider.AttachToElement(element, serviceList);
               SetServiceProvider(element, serviceProvider);
            }
         }
         if (serviceProvider != null)
         {
            service = serviceProvider.GetService(serviceType);
         }
         return service;
      }

      public static T GetService<T>(FrameworkElement element)
      {
         return (T)GetService(element, typeof(T));
      }

      public static void SetServiceList(DependencyObject obj, IEnumerable<IUIService> value)
      {
         obj.SetValue(ServiceListProperty, value);
      }

      private static IEnumerable<IUIService> GetServiceList(DependencyObject obj)
      {
         return (IEnumerable<IUIService>)obj.GetValue(ServiceListProperty);
      }

      private static UIServiceProvider GetServiceProvider(DependencyObject obj)
      {
         return (UIServiceProvider)obj.GetValue(ServiceProviderProperty);
      }

      private static void OnServiceListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         var element = sender as FrameworkElement;
         var oldList = changeArgs.OldValue as UIServiceCollection;
         if (element != null)
         {
            if (oldList != null)
            {
               var oldProvider = GetServiceProvider(element);
               if (oldProvider != null)
                  oldProvider.Dispose();
            }

            // Create a service provider for the element.
            UIServiceProvider newProvider = null;
            var newList = changeArgs.NewValue as UIServiceCollection;
            if (newList != null)
            {
               newProvider = new UIServiceProvider();
               newProvider.AttachToElement(element, newList);
            }
            SetServiceProvider(element, newProvider);
         }
      }

      private static void SetServiceProvider(DependencyObject obj, UIServiceProvider value)
      {
         obj.SetValue(ServiceProviderProperty, value);
      }

      #endregion Static members

      //--------------------------------------------------------------------------------------------------------//
      // End of static part.
      //--------------------------------------------------------------------------------------------------------//

      private FrameworkElement element;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private Window owningWindow;
      private Dictionary<Type, IUIService> serviceImplementations = new Dictionary<Type, IUIService>();

      public void Dispose()
      {
         if (owningWindow != null)
            owningWindow.Closed -= OwningWindow_Closed;
         LoadedEventManager.RemoveListener(element, this);
         UnloadedEventManager.RemoveListener(element, this);
         DetachFromElement(element, serviceImplementations.Values);
         foreach (var service in serviceImplementations.Values)
         {
            service.Dispose();
         }
      }

      public IUIService GetService(Type serviceType)
      {
         if (serviceImplementations.ContainsKey(serviceType))
            return serviceImplementations[serviceType];

         return null;
      }

      public T GetService<T>()
         where T : class
      {
         return GetService(typeof(T)) as T;
      }

      private void AttachToElement(FrameworkElement element, IEnumerable<IUIService> serviceList)
      {
         log.Debug("Setting service list for " + element);
         this.element = element;
         foreach (var service in serviceList)
         {
            var serviceType = service.GetType();
            var customAttrs = serviceType.GetCustomAttributes(typeof(ImplementedServiceAttribute), true);
            if (customAttrs != null && customAttrs.Count() > 0)
            {
               serviceType = ((ImplementedServiceAttribute)customAttrs[0]).ImplementedServiceType;
            }
            log.DebugFormat("Adding service {0} => {1}", serviceType.Name, service.GetType().Name);
            serviceImplementations.Add(serviceType, service);
         }
         LoadedEventManager.AddListener(element, this);
         UnloadedEventManager.AddListener(element, this);
         if (element.IsLoaded)
            Element_Loaded(element, new RoutedEventArgs());
      }

      private void DetachFromElement(FrameworkElement element, IEnumerable<IUIService> oldServiceList)
      {
         foreach (var service in oldServiceList)
            service.DetachFromElement(element);
      }

      private void Element_Loaded(object sender, RoutedEventArgs args)
      {
         foreach (var service in serviceImplementations.Values)
            service.AttachToElement((FrameworkElement)sender);

         owningWindow = UIUtils.GetAncestor<Window>(element);
         if (owningWindow != null)
            owningWindow.Closed += OwningWindow_Closed;
      }

      private void Element_Unloaded(object sender, RoutedEventArgs args)
      {
         DetachFromElement((FrameworkElement)sender, serviceImplementations.Values);
      }

      private void OwningWindow_Closed(object sender, EventArgs e)
      {
         Dispose();
      }

      #region IWeakEventListener Members

      public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
      {
         if (managerType == typeof(LoadedEventManager))
            Element_Loaded(sender, (RoutedEventArgs)e);
         else if (managerType == typeof(UnloadedEventManager))
            Element_Unloaded(sender, (RoutedEventArgs)e);
         else
            return false;

         return true;
      }

      #endregion IWeakEventListener Members
   }

   internal class LoadedEventManager : RoutedEventManagerBase
   {
      public override RoutedEvent Event
      {
         get { return FrameworkElement.LoadedEvent; }
      }

      public static void AddListener(UIElement source, IWeakEventListener listener)
      {
         GetManager<LoadedEventManager>().ProtectedAddListener(source, listener);
      }

      public static void RemoveListener(UIElement source, IWeakEventListener listener)
      {
         GetManager<LoadedEventManager>().ProtectedRemoveListener(source, listener);
      }
   }

   internal abstract class RoutedEventManagerBase : WeakEventManager
   {
      public abstract RoutedEvent Event { get; }

      protected static T GetManager<T>()
         where T : RoutedEventManagerBase, new()
      {
         var manager_type = typeof(T);
         var manager = WeakEventManager.GetCurrentManager(manager_type) as T;

         if (manager == null)
         {
            manager = new T();
            WeakEventManager.SetCurrentManager(manager_type, manager);
         }

         return manager;
      }

      protected override void StartListening(object source)
      {
         var sourceElement = (UIElement)source;
         sourceElement.AddHandler(Event, new RoutedEventHandler(OnRoutedEvent), true);
      }

      protected override void StopListening(object source)
      {
         var sourceElement = (UIElement)source;
         sourceElement.RemoveHandler(Event, new RoutedEventHandler(OnRoutedEvent));
      }

      private void OnRoutedEvent(object sender, RoutedEventArgs args)
      {
         DeliverEvent(sender, args);
      }
   }

   internal class UnloadedEventManager : RoutedEventManagerBase
   {
      public override RoutedEvent Event
      {
         get { return FrameworkElement.UnloadedEvent; }
      }

      public static void AddListener(UIElement source, IWeakEventListener listener)
      {
         GetManager<UnloadedEventManager>().ProtectedAddListener(source, listener);
      }

      public static void RemoveListener(UIElement source, IWeakEventListener listener)
      {
         GetManager<UnloadedEventManager>().ProtectedRemoveListener(source, listener);
      }
   }
}