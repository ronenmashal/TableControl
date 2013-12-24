using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TableControl
{
   class PrivateAccessHelper<T, A>
      where T : class
      where A : BaseShadow
   {
      public T Target { get; private set; }
      public PrivateObject PrivateObject { get; private set; }
      public A Accessor { get; private set; }

      public PrivateAccessHelper(T target)
      {
         PrivateObject = new PrivateObject(target);
         Accessor = (A)Activator.CreateInstance(typeof(A), this.PrivateObject);
      }
   }
}