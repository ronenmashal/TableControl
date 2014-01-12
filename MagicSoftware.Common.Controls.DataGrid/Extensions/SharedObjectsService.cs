using System;
using System.Collections.Generic;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class SharedObjectsService
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
   }
}

