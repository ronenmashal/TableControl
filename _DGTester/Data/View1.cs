using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace _DGTester.Data
{
   class View1
   {
      public ObservableCollection<MyDataView> Items { get; private set; }

      public View1()
      {
         Items = new ObservableCollection<MyDataView>();
         for (int i = 0; i < 100; i++)
         {
            if (i % 3 == 0)
               Items.Add(new MyHeaderView() { StringValue = "Header " + i, IntValue = i });
            Items.Add(new MyDataView() { StringVal = "String " + i, BoolVal = i % 2 == 0, FloatVal = (float)i / 5, IntVal = i });
         }
         Items.Insert(1, new MyDataView() { StringVal = "XXX", BoolVal = true, FloatVal = 0, IntVal = 0 });
      }
   }
}
