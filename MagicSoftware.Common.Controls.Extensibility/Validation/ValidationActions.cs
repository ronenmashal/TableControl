using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _DGTester.Validation
{
   [Flags]
   public enum ValidationActions
   {
      None = 0,
      ShowIndication = 1,
      Block = 2
   }
}
