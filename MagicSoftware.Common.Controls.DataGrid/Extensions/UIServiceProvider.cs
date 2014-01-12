using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
   public class UIServiceProvider
   {
      #region Static members

      public static IEnumerable<IUIService> GetServiceList(DependencyObject obj)
      {
         return (IEnumerable<IUIService>)obj.GetValue(ServiceListProperty);
      }

      public static void SetServiceList(DependencyObject obj, IEnumerable<IUIService> value)
      {
         obj.SetValue(ServiceListProperty, value);
      }

      /// <summary>
      /// Defines the list of services assigned to an element.
      /// </summary>
      public static readonly DependencyProperty ServiceListProperty =
          DependencyProperty.RegisterAttached("ServiceList", typeof(IEnumerable<IUIService>), typeof(UIServiceProvider), new UIPropertyMetadata(new List<IUIService>(), OnServiceListChanged));

      static void OnServiceListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         var element = sender as FrameworkElement;
         if (element != null)
         {
            // Create a service provider for the element.
            SetServiceProvider(element, new UIServiceProvider(element, (IEnumerable<IUIService>)changeArgs.NewValue));
         }
      }

      static UIServiceProvider GetServiceProvider(DependencyObject obj)
      {
         return (UIServiceProvider)obj.GetValue(ServiceProviderProperty);
      }

      static void SetServiceProvider(DependencyObject obj, UIServiceProvider value)
      {
         obj.SetValue(ServiceProviderProperty, value);
      }

      static readonly DependencyProperty ServiceProviderProperty =
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
               SetServiceProvider(element, new UIServiceProvider(element, serviceList));
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

      #endregion

      //--------------------------------------------------------------------------------------------------------//
      // End of static part.
      //--------------------------------------------------------------------------------------------------------//

      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      Dictionary<Type, IUIService> serviceImplementations = new Dictionary<Type, IUIService>();


      private UIServiceProvider(FrameworkElement element, IEnumerable<IUIService> serviceList)
      {
         SetServiceList(element, serviceList);
      }

      void SetServiceList(FrameworkElement element, IEnumerable<IUIService> serviceList)
      {
         log.Debug("Setting service list for " + element);
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
         element.Loaded += Element_Loaded;
         if (element.IsLoaded)
            Element_Loaded(element, new RoutedEventArgs());
      }

      void Element_Loaded(object sender, RoutedEventArgs args)
      {
         foreach (var service in serviceImplementations.Values)
         {
            service.SetElement((FrameworkElement)sender);
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
   }
}
