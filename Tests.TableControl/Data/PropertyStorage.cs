using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.TableControl.Data
{
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
