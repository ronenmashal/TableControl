using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;
using System.Windows;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DGProxyAsCurrentItemProvider : ObservableObject, ICurrentItemProvider, IWeakEventListener
   {
      private EnhancedDGProxy dgProxy;
      public DGProxyAsCurrentItemProvider(EnhancedDGProxy dgProxy)
      {
         this.dgProxy = dgProxy;
         PropertyChangedEventManager.AddListener(dgProxy, this, "CurrentItem");
      }

      public object CurrentItem { get { return dgProxy.CurrentItem; } }
      public int CurrentItemIndex { get { return dgProxy.CurrentPosition; } }

      public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
      {
         OnPropertyChanged("CurrentItem");
         return true;
      }
   }

}
