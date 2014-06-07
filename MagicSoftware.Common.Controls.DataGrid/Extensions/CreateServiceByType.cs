using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class CreateServiceByType : IUIServiceFactory
   {
      public Type ServiceType { get; set; }

      public IUIService CreateUIService()
      {
         var instance = Activator.CreateInstance(ServiceType, false);
         return (IUIService)instance;
      }
   }
}
