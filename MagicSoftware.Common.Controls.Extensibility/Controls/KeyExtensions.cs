using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicSoftware.Common.Utils
{
   public static class KeyExtensions
   {
      private static readonly Key[] UnprintableKeys = { Key.System, Key.LeftAlt, Key.RightAlt, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift };

      public static string ToString(this Key key, ModifierKeys modifiers)
      {
         StringBuilder keyDesc = new StringBuilder();
         if (modifiers.HasFlag(ModifierKeys.Alt))
            keyDesc.Append("Alt-");
         if (modifiers.HasFlag(ModifierKeys.Control))
            keyDesc.Append("Ctrl-");
         if (modifiers.HasFlag(ModifierKeys.Shift))
            keyDesc.Append("Shift-");

         keyDesc.Append(key.ToString());
         return keyDesc.ToString();
      }

      public static bool IsPrintable(this Key key)
      {
         return !UnprintableKeys.Contains(key);
      }

   }
}
