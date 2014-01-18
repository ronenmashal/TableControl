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
using MagicSoftware.Common.Controls.Table.Extensions;

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

      public static IDisposable Show(IList dataList, UIServiceCollection dgServiceList, out DataGrid dataGrid)
      {
         var w = new TestWindow();
         if (dgServiceList != null)
            UIServiceProvider.SetServiceList(w.MainDataGrid, dgServiceList);

         w.DataContext = new ListCollectionView(dataList);

         var result = TestUtils.AutoCloseWindow(w);
         dataGrid = w.MainDataGrid;
         return result;
      }

      public static IDisposable Show(IList dataList, out DataGrid dataGrid)
      {
         return Show(dataList, null, out dataGrid);
      }
   }
}
