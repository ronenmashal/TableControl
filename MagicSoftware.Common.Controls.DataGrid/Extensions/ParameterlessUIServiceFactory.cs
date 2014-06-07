using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class ParameterlessUIServiceFactory : IUIServiceFactory
   {
      public Type ServiceType { get; set; }

      public IUIService CreateUIService()
      {
         return (IUIService)Activator.CreateInstance(ServiceType);
      }
   }
}
