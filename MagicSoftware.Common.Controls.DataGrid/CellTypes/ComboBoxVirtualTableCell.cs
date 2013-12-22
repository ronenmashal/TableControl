using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using MagicSoftware.Common.Utils;
using System.Windows.Data;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.CellTypes
{
   public class ComboBoxVirtualTableCell : VirtualTableCell
   {
      static ComboBoxVirtualTableCell()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxVirtualTableCell), new FrameworkPropertyMetadata(typeof(ComboBoxVirtualTableCell)));
      }

      public bool IsEditable
      {
         get { return (bool)GetValue(IsEditableProperty); }
         set { SetValue(IsEditableProperty, value); }
      }

      // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty IsEditableProperty =
          DependencyProperty.Register("IsEditable", typeof(bool), typeof(ComboBoxVirtualTableCell), new UIPropertyMetadata(false));



      public IEnumerable ItemsSource
      {
         get { return (IEnumerable)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }

      // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ItemsSourceProperty =
          DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ComboBoxVirtualTableCell), new UIPropertyMetadata(null));


      protected override void SetBindings(FrameworkElement primaryBindingTarget)
      {
         if (ItemsSource != null)
         {
            BindingBase itemsSourceBinding = BindingOperations.GetBindingBase(this, ItemsSourceProperty);
            if (itemsSourceBinding != null)
               BindingOperations.SetBinding(primaryBindingTarget, ItemsControl.ItemsSourceProperty, itemsSourceBinding);
            else
               primaryBindingTarget.SetValue(ItemsControl.ItemsSourceProperty, ItemsSource);
         }
      }
   }
}
