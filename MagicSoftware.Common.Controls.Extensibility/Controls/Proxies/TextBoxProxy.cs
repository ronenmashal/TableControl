using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace MagicSoftware.Common.Controls.Proxies
{
   public class TextBoxProxy : ElementProxy
   {
      public TextBoxProxy(TextBox element) :
         base(element)
      {

      }

      protected override void DetachFromElement()
      {
         
      }

      public override DependencyProperty GetValueProperty()
      {
         return TextBox.TextProperty;
      }

      protected override bool TryExecuteCommand(Extensibility.Controls.ProxyCommand command)
      {
         throw new NotImplementedException();
      }
   }
}
