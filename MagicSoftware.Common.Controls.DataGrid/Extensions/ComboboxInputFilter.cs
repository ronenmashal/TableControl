using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using log4net;
using MagicSoftware.Common.Utils;
using LogLevel = log4net.Core.Level;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class ComboboxInputFilter : IInputFilter
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);      

      public bool ElementWillProcessInput(InputEventArgs args)
      {
         ComboBox combobox = args.OriginalSource as ComboBox;

         log.DebugFormat("Filtering input on {0}", args.OriginalSource);

         KeyEventArgs keyEventArgs = args as KeyEventArgs;
         if (keyEventArgs != null)
         {
            if (keyEventArgs.IsDown)
            {
               switch (keyEventArgs.Key)
               {
                  case Key.Home:
                  case Key.End:
                     return true;

                  case Key.Up:
                     return combobox.IsDropDownOpen;

                  case Key.Down:
                     return combobox.IsDropDownOpen | (Keyboard.Modifiers == ModifierKeys.Alt);
               }
            }

            // Element will not handle the key.
            return false;
         }

         // Not a key event -- Is it a mouse event?

         MouseEventArgs mouseEventArgs = args as MouseEventArgs;
         if (mouseEventArgs != null)
         {
            return true;
         }

         return false;
      }
   }
}
