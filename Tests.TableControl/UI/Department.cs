using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;
using System.Collections.ObjectModel;

namespace Tests.TableControl.UI
{
   class Department : ObservableObject
   {
      PropertyStorage properties;

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
         properties = new PropertyStorage((propName) => OnPropertyChanged(propName));
         members = new ObservableCollection<Employee>();
         Members = new ReadOnlyObservableCollection<Employee>(members);
      }

      public void AddMember(Employee employee)
      {
         members.Add(employee);
      }
   }

   class PropertyStorage
   {
      Dictionary<string, object> propertyValues = new Dictionary<string, object>();
      Action<string> propertyChangedCallback;

      public PropertyStorage(Action<string> propertyChangedCallback)
      {
         this.propertyChangedCallback = propertyChangedCallback;
      }

      public object this[string propertyName]
      {
         get { return propertyValues[propertyName]; }
         set
         {
            propertyValues[propertyName] = value;
            if (propertyChangedCallback != null)
               propertyChangedCallback(propertyName);
         }
      }

      public T GetValue<T>(string propertyName)
      {
         return (T)propertyValues[propertyName];
      }
   }
}
