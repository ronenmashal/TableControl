using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   static class IdGenerator
   {
      static Dictionary<Type, int> ids = new Dictionary<Type, int>();

      public static int GetNewId(object idOwner)
      {
         int nextId;
         if (!ids.TryGetValue(idOwner.GetType(), out nextId))
         {
            nextId = 1;
            ids[idOwner.GetType()] = 2;
         }
         else
            ids[idOwner.GetType()]++;

         return nextId;
      }
   }
}
