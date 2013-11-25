using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.ComponentModel.Composition;

namespace MagicSoftware.Common.Controls.ProxiesX
{
   public class ElementProxyFactory : IElementProxyFactory
   {
      public static ElementProxyFactory Instance { get; private set; }

      static ElementProxyFactory()
      {
         Instance = new ElementProxyFactory();
      }

      /* ----------------------------------------------------------------------- */

      Dictionary<object, ElementProxy> proxies;
      Dictionary<Type, Type> proxyTypes;

      protected ElementProxyFactory()
      {
         proxies = new Dictionary<object, ElementProxy>();
         proxyTypes = new Dictionary<Type, Type>();
      }

      public ElementProxy CreateProxy(UIElement element)
      {
         if (!proxies.ContainsKey(element))
         {
            Type proxyType = FindProxyTypeForElementType(element.GetType());
            proxies.Add(element, (ElementProxy)Activator.CreateInstance(proxyType, element));
         }
         return GetProxy(element);
      }

      private Type FindProxyTypeForElementType(Type elementType)
      {
         Type proxyType;
         if (TryFindProxyTypeForElementType(elementType, out proxyType))
         {
            return proxyType;
         }
         throw new InvalidOperationException("No proxy was registered for element type " + elementType);
      }

      private bool TryFindProxyTypeForElementType(Type elementType, out Type proxyType)
      {
         if (proxyTypes.ContainsKey(elementType))
         {
            proxyType = proxyTypes[elementType];
            return true;
         }

         if (elementType.BaseType != null)
            return TryFindProxyTypeForElementType(elementType.BaseType, out proxyType);

         proxyType = null;
         return false;
      }

      public ElementProxy GetProxy(UIElement element)
      {
         if (!(proxies.ContainsKey(element)))
            CreateProxy(element);
         return proxies[element];
      }

      public void RegisterProxyType(Type proxyType, Type elementType)
      {
         proxyTypes.Add(elementType, proxyType);
      }
   }

}
