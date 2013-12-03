using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Diagnostics;
using MagicSoftware.Common.ViewModel;

namespace _DGTester.Data
{
   class View1 : ObservableObject
   {
      public ObservableCollection<MyDataView> items;
      public ListCollectionView Items { get; private set; }

      public View1()
      {
         items = new ObservableCollection<MyDataView>();
         for (int i = 0; i < 100; i++)
         {
            if (i % 20 == 0)
               items.Add(new MyHeaderView() { StringValue = "Header " + i, IntValue = i });
            items.Add(new MyDataView() { StringVal = "String " + i, BoolVal = i % 2 == 0, FloatVal = (float)i / 5, IntVal = i });
         }
         items.Insert(1, new MyDataView() { StringVal = "XXX", BoolVal = true, FloatVal = 0, IntVal = 0 });

         Items = new ListCollectionView(items);

         Items.CurrentChanged += new EventHandler(Items_CurrentChanged);
      }

      void Items_CurrentChanged(object sender, EventArgs e)
      {
         OnPropertyChanged("CurrentItem");
      }

      public object CurrentItem { get { return Items.CurrentItem; } }
   }
}
