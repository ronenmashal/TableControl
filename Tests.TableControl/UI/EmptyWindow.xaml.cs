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
using Tests.Common;

namespace Tests.TableControl.UI
{
   /// <summary>
   /// Interaction logic for EmptyWindow.xaml
   /// </summary>
   public partial class EmptyWindow : Window
   {
      public EmptyWindow()
      {
         InitializeComponent();
      }

      public IDisposable ShowForTest()
      {
         return TestUtils.AutoCloseWindow(this);
      }
   }
}
