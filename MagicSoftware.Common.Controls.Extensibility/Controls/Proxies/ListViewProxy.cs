using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Proxies
{
   public class ListBoxProxy : SelectorProxy
   {
      public ListBoxProxy(ListBox element):
         base(element)
      {
      }

      private ListBox ListBox { get { return (ListBox)ListBox; } }



      protected override void ScrollIntoView(object item)
      {
         ListBox.ScrollIntoView(item);
      }
   }
}
