using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Proxies
{
   public class CheckBoxProxy : ElementProxy
   {
      public CheckBoxProxy(CheckBox element):
         base(element)
      {

      }
      protected override void DetachFromElement()
      {
         
      }

      public override DependencyProperty GetValueProperty()
      {
         return CheckBox.IsCheckedProperty;
      }

      protected override bool TryExecuteCommand(Extensibility.Controls.ProxyCommand command)
      {
         throw new NotImplementedException();
      }
   }
}
