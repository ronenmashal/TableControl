using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TableControl
{
   class PrivateAccessHelper<T, A>
      where T : class
      where A : BaseShadow
   {
      public T Target { get; private set; }
      private PrivateObject PrivateObject { get; set; }
      public A Accessor { get; private set; }

      public PrivateAccessHelper(T target)
      {
         Target = target;
         PrivateObject = new PrivateObject(target);
         Accessor = (A)Activator.CreateInstance(typeof(A), this.PrivateObject);
      }

      public object Invoke(string methodName, params object[] args)
      {
         return PrivateObject.Invoke(methodName, args);
      }
   }
}