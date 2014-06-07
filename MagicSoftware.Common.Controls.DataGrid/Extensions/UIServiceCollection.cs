using System;
using System.Collections;
using System.Collections.Generic;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class UIServiceCollection : ICollection<IUIService>, IEnumerable<IUIService>, IList, ICollection, IEnumerable
   {
      private List<IUIServiceFactory> serviceFactories = new List<IUIServiceFactory>();
      private List<IUIService> services = new List<IUIService>();

      public int Count
      {
         get { return services.Count + serviceFactories.Count; }
      }

      bool ICollection.IsSynchronized
      {
         get { throw new NotImplementedException(); }
      }

      object ICollection.SyncRoot
      {
         get { throw new NotImplementedException(); }
      }

      bool IList.IsFixedSize
      {
         get { return false; }
      }

      bool IList.IsReadOnly
      {
         get { throw new NotImplementedException(); }
      }

      public bool IsReadOnly
      {
         get { throw new NotImplementedException(); }
      }

      object IList.this[int index]
      {
         get
         {
            throw new NotImplementedException();
         }
         set
         {
            throw new NotImplementedException();
         }
      }

      public void Add(IUIServiceFactory serviceFactory)
      {
         serviceFactories.Add(serviceFactory);
      }

      public void Add(IUIService item)
      {
         services.Add(item);
      }

      public void Clear()
      {
         services.Clear();
         serviceFactories.Clear();
      }

      public bool Contains(IUIService item)
      {
         throw new NotImplementedException();
      }

      public void CopyTo(IUIService[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public IEnumerator<IUIService> GetEnumerator()
      {
         return new Enumerator(this);
      }

      void ICollection.CopyTo(Array array, int index)
      {
         throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         throw new NotImplementedException();
      }

      int IList.Add(object value)
      {
         if (value is IUIService)
         {
            Add((IUIService)value);
            return services.Count;
         }
         if (value is IUIServiceFactory)
         {
            Add((IUIServiceFactory)value);
            return services.Count + serviceFactories.Count;
         }

         throw new ArgumentException("value", "value must be IUIServie or IUIServiceFactory");
      }

      bool IList.Contains(object value)
      {
         return Contains((IUIService)value);
      }

      int IList.IndexOf(object value)
      {
         throw new NotImplementedException();
      }

      void IList.Insert(int index, object value)
      {
         throw new NotImplementedException();
      }

      void IList.Remove(object value)
      {
         throw new NotImplementedException();
      }

      void IList.RemoveAt(int index)
      {
         throw new NotImplementedException();
      }

      public bool Remove(IUIService item)
      {
         throw new NotImplementedException();
      }

      private class Enumerator : IEnumerator<IUIService>
      {
         private IEnumerator currentEnumerator;
         private Func<IUIService> getNextItem;
         private int nextEnumerator;
         private UIServiceCollection serviceList;

         public Enumerator(UIServiceCollection serviceList)
         {
            this.serviceList = serviceList;
            Reset();
         }

         public IUIService Current
         {
            get
            {
               if (currentEnumerator != null)
                  return getNextItem();
               return null;
            }
         }

         object IEnumerator.Current
         {
            get { return Current; }
         }

         public void Dispose()
         {
            currentEnumerator = null;
         }

         public bool MoveNext()
         {
            while (currentEnumerator != null)
            {
               if (currentEnumerator.MoveNext())
                  return true;
               SwitchEnumerator();
            }

            return false;
         }

         public void Reset()
         {
            nextEnumerator = 1;
            SwitchEnumerator();
         }

         private IUIService GetNextUIServiceFromServiceFactoryList()
         {
            return ((IUIServiceFactory)currentEnumerator.Current).CreateUIService();
         }

         private IUIService GetNextUIServiceFromServiceList()
         {
            return (IUIService)currentEnumerator.Current;
         }

         private void SwitchEnumerator()
         {
            switch (nextEnumerator)
            {
               case 1:
                  currentEnumerator = serviceList.services.GetEnumerator();
                  getNextItem = GetNextUIServiceFromServiceList;
                  break;

               case 2:
                  currentEnumerator = serviceList.serviceFactories.GetEnumerator();
                  getNextItem = GetNextUIServiceFromServiceFactoryList;
                  break;

               default:
                  currentEnumerator = null;
                  break;
            }
            nextEnumerator++;
         }
      }
   }
}