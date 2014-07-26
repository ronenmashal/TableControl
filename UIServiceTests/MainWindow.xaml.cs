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
using System.Collections.ObjectModel;

namespace UIServiceTests
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public List<string> Items { get; set; }
      public ObservableCollection<string> SelectedItems { get; set; }

      public MainWindow()
      {
         SelectedItems = new ObservableCollection<string>();
         Items = new List<string>(new string[] { "Hello", "Goodbye", "Good morning", "Good night", "Have a nice day!" });
         InitializeComponent();
         DataContext = this;
      }
   }
}
