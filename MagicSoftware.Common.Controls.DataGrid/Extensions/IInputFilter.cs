using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface IInputFilter
   {
      bool ElementWillProcessInput(InputEventArgs args);
   }
}
