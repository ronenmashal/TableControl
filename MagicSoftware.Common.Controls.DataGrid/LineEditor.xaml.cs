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
   /// Interaction logic for LineEditor.xaml
   /// </summary>
   public partial class LineEditor : UserControl
   {
      public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(LineEditor));
      public static readonly DependencyProperty ItemTemplateSelectorProperty = ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(LineEditor), new FrameworkPropertyMetadata(new SelectTemplateByItemType()));

      public IEnumerable ItemsSource
      {
         get { return (IEnumerable)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }

      public DataTemplateSelector ItemTemplateSelector
      {
         get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
         set { SetValue(ItemTemplateSelectorProperty, value); }
      }

      public LineEditor()
      {
         InitializeComponent();
      }

      class SelectTemplateByItemType : DataTemplateSelector
      {
         public override DataTemplate SelectTemplate(object item, DependencyObject container)
         {
            FrameworkElement itemContainer = container as FrameworkElement;
            var resource = itemContainer.TryFindResource(item.GetType());
            if (resource != null)
               return resource as DataTemplate;

            return base.SelectTemplate(item, container);
         }
      }
   }
}
