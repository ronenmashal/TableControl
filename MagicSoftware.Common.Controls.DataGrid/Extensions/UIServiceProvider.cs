using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
          DependencyProperty.RegisterAttached("Services", typeof(IEnumerable<IUIService>), typeof(UIServiceProvider), new UIPropertyMetadata(new List<IUIService>(), OnServiceListChanged));

      static void OnServiceListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         var element = sender as UIElement;
         if (element != null)
         {
            // Create a service provider for the element.
            SetServiceProvider(element, new UIServiceProvider(element, (IEnumerable<IUIService>)changeArgs.NewValue));
         }
      }

      public static UIServiceProvider GetServiceProvider(DependencyObject obj)
      {
         return (UIServiceProvider)obj.GetValue(ServiceProviderProperty);
      }

      public static void SetServiceProvider(DependencyObject obj, UIServiceProvider value)
      {
         obj.SetValue(ServiceProviderProperty, value);
      }

      public static readonly DependencyProperty ServiceProviderProperty =
          DependencyProperty.RegisterAttached("ServiceProvider", typeof(UIServiceProvider), typeof(UIServiceProvider), new UIPropertyMetadata(null));


      #endregion

      //--------------------------------------------------------------------------------------------------------//
      // End of static part.
      //--------------------------------------------------------------------------------------------------------//

      Dictionary<Type, IUIService> serviceImplementations = new Dictionary<Type, IUIService>();


      private UIServiceProvider(UIElement element, IEnumerable<IUIService> serviceList)
      {
         SetServiceList(element, serviceList);
      }

      void SetServiceList(UIElement element, IEnumerable<IUIService> serviceList)
      {
         foreach (var service in serviceList)
         {
            var serviceType = service.GetType();
            var customAttrs = serviceType.GetCustomAttributes(typeof(ImplementedServiceAttribute), false);
            if (customAttrs != null && customAttrs.Count() > 0)
            {
               serviceType = ((ImplementedServiceAttribute)customAttrs[0]).ImplementedServiceType;
            }
            service.SetElement(element);
            serviceImplementations.Add(serviceType, service);
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
