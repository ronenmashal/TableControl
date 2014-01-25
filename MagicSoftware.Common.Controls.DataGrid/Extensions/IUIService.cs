using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface IUIService : IDisposable
   {
      void AttachToElement(FrameworkElement element);
      void DetachFromElement(FrameworkElement element);
   }
}
