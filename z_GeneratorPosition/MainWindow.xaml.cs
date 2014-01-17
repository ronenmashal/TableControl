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
using System.Windows.Controls.Primitives;

namespace z_GeneratorPosition
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public List<MyData> Items { get; private set; }

      public MainWindow()
      {
         Items = new List<MyData>();
         for (int i = 0; i < 50; i++)
            Items.Add(new MyData() { StrValue = "value " + i, IntValue = i });

         InitializeComponent();
         DataContext = this;
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         var value1 = dg.ItemContainerGenerator.ContainerFromIndex(30);
         UIElement cntr = null;
         bool isNewlyRealized = false;
         IItemContainerGenerator generator = dg.ItemContainerGenerator;
         GeneratorPosition gp = generator.GeneratorPositionFromIndex(30);
         using (generator.StartAt(gp, GeneratorDirection.Forward, true))
         {
            isNewlyRealized = false;
            cntr = generator.GenerateNext(out isNewlyRealized) as UIElement;
         }

         MessageBox.Show("Row 30: " + value1 + ", " + cntr + ", " + isNewlyRealized);
      }
   }

   public class MyData
   {
      public string StrValue { get; set; }
      public int IntValue { get; set; }
   }
}
