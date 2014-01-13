using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;

namespace Tests.TableControl.UI
{
   class Employee : ObservableObject
   {
      string name;
      public string Name
      {
         get { return name;}
         set
         {
            name = value;
            OnPropertyChanged("Name");
         }
      }

      int age;
      public int Age
      {
         get { return age; }
         set
         {
            age = value;
            OnPropertyChanged("Age");
         }
      }

      string address;
      public string Address
      {
         get { return address; }
         set
         {
            address = value;
            OnPropertyChanged("Address");
         }
      }

      public bool isActive;
      public bool IsActive
      {
         get { return isActive; }
         set
         {
            isActive = value;
            OnPropertyChanged("IsActive");
         }
      }
   }
}
