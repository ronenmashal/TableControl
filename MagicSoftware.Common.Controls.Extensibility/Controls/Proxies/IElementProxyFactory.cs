using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel.Composition;

namespace MagicSoftware.Common.Controls.Proxies
{
   public interface IElementProxyFactory
   {
      ElementProxy CreateProxy(UIElement element);
      ElementProxy GetProxy(UIElement element);
   }
}
