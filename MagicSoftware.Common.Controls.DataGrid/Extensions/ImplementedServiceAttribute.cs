using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
   class ImplementedServiceAttribute : Attribute
   {
      public ImplementedServiceAttribute(Type implementedServiceType)
      {
         if (implementedServiceType == null)
            throw new ArgumentNullException("implementedServiceType");

         //if (!implementedServiceType.GetInterfaces().Contains(typeof(IUIService)))
         //   throw new ArgumentException("Implemented service type must inherit from IUIService", "implementedServiceType");

         ImplementedServiceType = implementedServiceType;
      }

      public Type ImplementedServiceType { get; private set; }
   }
}
