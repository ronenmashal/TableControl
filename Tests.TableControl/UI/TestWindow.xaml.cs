using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
using Tests.Common;

namespace Tests.TableControl.UI
{
   /// <summary>
   /// Interaction logic for TestWindow.xaml
   /// </summary>
   public partial class TestWindow : Window
   {
      public TestWindow()
      {
         InitializeComponent();
      }

      public DataGrid MainDataGrid { get { return dataGrid; } }

      public static IDisposable Show(IList dataList, out DataGrid dataGrid)
      {
         var w = new TestWindow();
         w.DataContext = new ListCollectionView(dataList);

         var result = TestUtils.AutoCloseWindow(w);
         dataGrid = w.MainDataGrid;
         return result;
      }
   }
}
