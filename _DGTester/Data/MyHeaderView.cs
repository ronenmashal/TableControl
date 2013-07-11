using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _DGTester.Data
{
   class MyHeaderView : MyDataView
   {
      public string StringValue { get; set; }
      public int IntValue { get; set; }

      public override string ToString()
      {
         return String.Format("MyHeaderView: {0},{1}", StringValue, IntValue);
      }
   }
}
