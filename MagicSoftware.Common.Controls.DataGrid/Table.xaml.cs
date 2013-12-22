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
using System.Collections;

namespace MagicSoftware.Common.Controls.Table
{
   /// <summary>
   /// Interaction logic for Table.xaml
   /// </summary>
   public partial class Table : UserControl
   {


      public StyleSelector RowStyleSelector
      {
         get { return (StyleSelector)GetValue(RowStyleSelectorProperty); }
         set { SetValue(RowStyleSelectorProperty, value); }
      }

      public static readonly DependencyProperty RowStyleSelectorProperty =
          DependencyProperty.Register("RowStyleSelector", typeof(StyleSelector), typeof(Table), new UIPropertyMetadata(null));



      public IEnumerable ItemsSource
      {
         get { return (IEnumerable)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }

      // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ItemsSourceProperty =
          DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Table), new UIPropertyMetadata(null));



      public Table()
      {
         InitializeComponent();
      }
   }
}
