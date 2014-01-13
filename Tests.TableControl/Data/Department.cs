using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;
using System.Collections.ObjectModel;

namespace Tests.TableControl.Data
{
   class Department : TestClassBase
   {
      ObservableCollection<Employee> members;

      public ReadOnlyObservableCollection<Employee> Members { get; private set; }

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

      public Department()
      {
         members = new ObservableCollection<Employee>();
         Members = new ReadOnlyObservableCollection<Employee>(members);
      }

      public void AddMember(Employee employee)
      {
         members.Add(employee);
      }
   }
}
