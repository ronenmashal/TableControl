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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _DGTester.Data;
using MagicSoftware.Common.Controls.ProxiesX;

namespace _DGTester
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {

      static MainWindow()
      {
         ElementProxyFactory.Instance.RegisterProxyType(typeof(DataGridProxy), typeof(DataGrid));
         ElementProxyFactory.Instance.RegisterProxyType(typeof(TextBoxProxy), typeof(TextBox));
         ElementProxyFactory.Instance.RegisterProxyType(typeof(CheckBoxProxy), typeof(CheckBox));
         ElementProxyFactory.Instance.RegisterProxyType(typeof(ComboBoxProxy), typeof(ComboBox));
      }

      public MainWindow()
      {
         InitializeComponent();
         DataContext = new View1();
      }

      private void DataGrid_Loaded(object sender, RoutedEventArgs e)
      {
         ((FrameworkElement)sender).Focus();
      }



      public Visibility ColVisibility
      {
         get { return (Visibility)GetValue(ColVisibilityProperty); }
         set { SetValue(ColVisibilityProperty, value); }
      }

      // Using a DependencyProperty as the backing store for ColVisibility.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ColVisibilityProperty =
          DependencyProperty.Register("ColVisibility", typeof(Visibility), typeof(MainWindow), new UIPropertyMetadata(Visibility.Visible));

      private void ChangeColumnVisibility_Click(object sender, RoutedEventArgs e)
      {
         ColVisibility = (Visibility)(((int)ColVisibility + 1) % 3);
      }

      private void GetSelection_Click(object sender, RoutedEventArgs e)
      {
         var proxy = ElementProxyFactory.Instance.GetProxy(DG) as MultiSelectorProxy;
         StringBuilder selection = new StringBuilder();
         foreach (var item in proxy.SelectedItems)
            selection.Append(item.ToString() + Environment.NewLine);
         MessageBox.Show(selection.ToString());
      }



   }
}
