using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;
using System.Collections.ObjectModel;

namespace Tests.TableControl.Data
{
   class Company : TestClassBase
   {
      public string Name
      {
         get { return (string)properties["Name"]; }
         set { properties["Name"] = value; }
      }

      public Employee Manager
      {
         get { return (Employee)properties["Manager"]; }
         set { properties["Manager"] = value; }
      }

      public string Ticker
      {
         get { return (string)properties["Ticker"]; }
         set { properties["Ticker"] = value; }
      }

      ObservableCollection<Department> departments = new ObservableCollection<Department>();
      public ReadOnlyObservableCollection<Department> Departments { get; private set; }

      public Company()
      {
         Departments = new ReadOnlyObservableCollection<Department>(departments);
      }

      public void Add(Department department)
      {
         departments.Add(department);
      }
   }
}
