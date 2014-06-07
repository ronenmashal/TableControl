using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Table.Extensions;

namespace Tests.TableControl
{
   class ServiceFactoryMock : IUIServiceFactory
   {
      private IUIService targetService;

      public ServiceFactoryMock(IUIService target)
      {
         this.targetService = target;
      }

      public IUIService CreateUIService()
      {
         return targetService;
      }
   }

}
