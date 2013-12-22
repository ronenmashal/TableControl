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
using MagicSoftware.Common.Utils;
using System.ComponentModel;

namespace MagicSoftware.Common.Controls.Table
{
   public abstract class VirtualTableCell : ContentControl
   {
      static VirtualTableCell()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualTableCell), new FrameworkPropertyMetadata(typeof(VirtualTableCell)));
      }

      public bool IsEditing
      {
         get { return (bool)GetValue(IsEditingProperty); }
         private set { SetValue(IsEditingPropertyKey, value); }
      }

      static DependencyPropertyKey IsEditingPropertyKey = DependencyProperty.RegisterReadOnly("IsEditing", typeof(bool), typeof(VirtualTableCell), new UIPropertyMetadata(true, OnIsEditingChanged));
      public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

      static void OnIsEditingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         //var tableCell = sender as VirtualTableCell;
         //if (tableCell != null)
         //{
         //   if ((bool)changeArgs.NewValue == true)
         //      tableCell.CurrentTemplate = tableCell.EditingElement;
         //   else
         //      tableCell.CurrentTemplate = tableCell.Element;
         //}
      }




      public DataTemplate Element
      {
         get { return (DataTemplate)GetValue(ElementProperty); }
         set { SetValue(ElementProperty, value); }
      }

      // Using a DependencyProperty as the backing store for Element.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ElementProperty =
          DependencyProperty.Register("Element", typeof(DataTemplate), typeof(VirtualTableCell), new UIPropertyMetadata(null));




      public DataTemplate EditingElement
      {
         get { return (DataTemplate)GetValue(EditingElementProperty); }
         set { SetValue(EditingElementProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditingElement.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditingElementProperty =
          DependencyProperty.Register("EditingElement", typeof(DataTemplate), typeof(VirtualTableCell), new UIPropertyMetadata(null));




      public static DependencyProperty GetBindingTargetProperty(DependencyObject obj)
      {
         return (DependencyProperty)obj.GetValue(BindingTargetPropertyProperty);
      }

      public static void SetBindingTargetProperty(DependencyObject obj, DependencyProperty value)
      {
         obj.SetValue(BindingTargetPropertyProperty, value);
      }

      // Using a DependencyProperty as the backing store for BindingTarget.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty BindingTargetPropertyProperty =
          DependencyProperty.RegisterAttached("BindingTargetProperty", typeof(DependencyProperty), typeof(VirtualTableCell), new UIPropertyMetadata(null));



      public BindingBase Binding { get; set; }


      public VirtualTableCell()
      {
         
      }

      ContentPresenter contentPresenter;

      public override void OnApplyTemplate()
      {
         base.OnApplyTemplate();
         contentPresenter = UIUtils.GetVisualChild<ContentPresenter>(this);
         contentPresenter.Loaded += new RoutedEventHandler(contentPresenter_Loaded);
      }

      void contentPresenter_Loaded(object sender, RoutedEventArgs e)
      {
         // Set Data Context on the template's root element.
         var topMostElement = UIUtils.GetVisualChild<FrameworkElement>(contentPresenter);
         if (topMostElement != null)
         {
            topMostElement.SetValue(FrameworkElement.DataContextProperty, this.DataContext);
         }

         // Get the primary binding target, to which the binding will be transferred.
         var bindingTarget = UIUtils.GetVisualChild<FrameworkElement>(contentPresenter, (fe) => { return GetBindingTargetProperty(fe) != null; }, SearchOrder.FirstToLast);
         if (bindingTarget != null)
         {
            var bindingTargetProperty = GetBindingTargetProperty(bindingTarget);
            var appliedBinding = BindingUtils.CloneBinding(Binding);
            if (appliedBinding is Binding)
            {
               ((Binding)appliedBinding).Mode = IsEditing ? BindingMode.TwoWay : BindingMode.OneWay;
            }
            BindingOperations.SetBinding(bindingTarget, bindingTargetProperty, appliedBinding);

            SetBindings(bindingTarget);
         }
      }

      protected virtual void SetBindings(FrameworkElement primaryBindingTarget)
      {
         
      }

      internal bool BeginEdit()
      {
         IsEditing = true;
         return IsEditing;
      }

      internal bool CancelEdit()
      {
         IsEditing = false;
         return !IsEditing;
      }

      internal bool CommitEdit()
      {
         IsEditing = false;
         return !IsEditing;
      }

   }

   public class VirtualTableCellTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate(object item, DependencyObject container)
      {
         var templatedParent = ((FrameworkElement)container).TemplatedParent;
         var tableCell = templatedParent as VirtualTableCell;
         if (tableCell.IsEditing)
            return tableCell.EditingElement;
         else
            return tableCell.Element;
      }
   }
}
