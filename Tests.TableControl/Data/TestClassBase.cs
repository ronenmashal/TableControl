using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;

namespace Tests.TableControl.Data
{
   class TestClassBase : ObservableObject
   {
      protected readonly PropertyStorage properties;

      public TestClassBase()
      {
          properties = new PropertyStorage((p) => OnPropertyChanged(p));
      }
   }
}
