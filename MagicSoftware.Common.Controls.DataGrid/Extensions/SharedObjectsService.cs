using System;
using System.Collections.Generic;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class SharedObjectsService : IUIService
   {
      Dictionary<string, object> sharedObjects = new Dictionary<string, object>();

      public void SetSharedObject(string identifier, object sharedObject)
      {
         sharedObjects[identifier] = sharedObject;
      }

      public object GetSharedObject(string identifier)
      {
         return sharedObjects[identifier];
      }

      internal bool HasSharedObject(string identifier)
      {
         return sharedObjects.ContainsKey(identifier);
      }

      #region IUIService Members

      public void AttachToElement(FrameworkElement element)
      {
         
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         sharedObjects.Clear();
      }

      #endregion
   }
}

