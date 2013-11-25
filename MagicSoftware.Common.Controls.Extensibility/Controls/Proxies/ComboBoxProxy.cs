using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.ProxiesX
{
   public class ComboBoxProxy : ElementProxy
   {
      public ComboBoxProxy(ComboBox element):
         base(element)
      {

      }
      protected override void DetachFromElement()
      {
         throw new NotImplementedException();
      }

      public override DependencyProperty GetValueProperty()
      {
         return ComboBox.TextProperty;
      }

      protected override bool TryExecuteCommand(Extensibility.Controls.ProxyCommand command)
      {
         throw new NotImplementedException();
      }
   }
}
