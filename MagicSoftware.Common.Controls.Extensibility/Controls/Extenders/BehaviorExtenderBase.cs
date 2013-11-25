using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MagicSoftware.Common.Controls.ProxiesX;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   public abstract class BehaviorExtenderBase : IDisposable
   {
      protected UIElement AttachedElement { get; private set; }
      protected ElementProxy Proxy { get; set; }

      [Import(typeof(IElementProxyFactory), AllowRecomposition=true)]
      protected IElementProxyFactory ProxyFactory { get; set; }

      public void AttachToElement(UIElement targetElement)
      {
         if (AttachedElement != null)
            throw new InvalidOperationException("Cannot attach the same extender to more than one element");

         //var catalog = new AggregateCatalog(new DirectoryCatalog("."), new AssemblyCatalog(Assembly.GetExecutingAssembly()));
         //var container = new CompositionContainer(catalog);
         //var batch = new CompositionBatch();
         //container.Compose(batch);

         //ProxyFactory = container.GetExportedValue<IElementProxyFactory>();
         AttachedElement = targetElement;
         ProxyFactory = ElementProxyFactory.Instance;
         //ProxyFactory = ((MagicSoftware.Common.Controls.Extenders.App)App.Current).ElementProxyFactory;
         //if (ProxyFactory == null)
         //   return;
         Proxy = (DataGridProxy)ProxyFactory.CreateProxy(targetElement);
         Initialize();
      }

      protected abstract void Initialize();
      public abstract void DetachFromElement();

      #region IDisposable Members

      public void Dispose()
      {
         DetachFromElement();
         Proxy.Dispose();
      }

      #endregion
   }
}
