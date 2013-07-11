using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace MagicSoftware.Common.Controls.Extensibility.Controls
{
   [DebuggerDisplay("ProxyCommand {Name}")]
   public class ProxyCommand : RoutedUICommand
   {
      public static readonly ProxyCommand MoveDown = new ProxyCommand("Move Down", "PC_MoveDown");
      public static readonly ProxyCommand MoveUp = new ProxyCommand("Move Up", "PC_MoveUp");
      public static readonly ProxyCommand GoToField = new ProxyCommand("Go To Field", "PC_GoToField");
      public static readonly ProxyCommand ScrollUp = new ProxyCommand("ScrollUp", "PC_ScrollUp");
      public static readonly ProxyCommand ScrollDown = new ProxyCommand("ScrollDown", "PC_ScrollDown");

      public static readonly HashSet<ProxyCommand> NavigationCommands = new HashSet<ProxyCommand>()
      {
         MoveDown, MoveUp, GoToField
      };

      public static readonly HashSet<ProxyCommand> ScrollCommands = new HashSet<ProxyCommand>()
      {
         ScrollUp, ScrollDown
      };

      public ProxyCommand(string text, string name)
         : base(text, name, typeof(ProxyCommand))
      {

      }

      public ProxyCommand(string name)
         : base(name, name, typeof(ProxyCommand))
      {

      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override bool Equals(object obj)
      {
         var other = obj as ProxyCommand;
         if (other == null)
            return false;

         return Name.Equals(other.Name);
      }
   }
}
