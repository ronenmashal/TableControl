using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions.Selection
{
   internal class IdleSelectionMode : SelectionMode
   {
      public override void Enter()
      {
      }

      public override void Leave()
      {
      }

      public override void OnCurrentItemChanged()
      {
      }

      protected override SelectionMode HandleKeyboardEvent(object sender, KeyEventArgs keyEventArgs)
      {
         return this;
      }

      protected override SelectionMode HandleMouseEvent(object sender, MouseButtonEventArgs mouseButtonEventArgs)
      {
         return this;
      }
   }
}
