using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;

namespace Tests.TableControl.Data
{
   class Employee : TestClassBase
   {
      public string Name
      {
         get { return (string)properties["Name"]; }
         set { properties["Name"] = value; }
      }

      public int Age
      {
         get { return (int)properties["Age"]; }
         set { properties["Age"] = value; }
      }

      public string Address
      {
         get { return (string)properties["Address"]; }
         set { properties["Address"] = value; }
      }

      public bool IsActive
      {
         get { return (bool)properties["IsActive"]; }
         set { properties["IsActive"] = value; }
      }
   }
}
