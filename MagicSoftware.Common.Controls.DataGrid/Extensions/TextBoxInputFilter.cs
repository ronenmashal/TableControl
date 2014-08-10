using System;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal class TextBoxInputFilter : IInputFilter
   {
      public bool ElementWillProcessInput(InputEventArgs args)
      {
         KeyEventArgs keyEventArgs = args as KeyEventArgs;
         if (keyEventArgs != null)
         {
            if (keyEventArgs.IsDown)
            {
               switch (keyEventArgs.Key)
               {
                  case Key.Left:
                  case Key.Right:
                  case Key.Home:
                  case Key.End:
                     return true;
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