﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MagicSoftware.Common.Utils;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.CellTypes
{
   public abstract class VirtualTableCell : ContentControl
   {
      public static readonly DependencyProperty BindingTargetPropertyProperty =
          DependencyProperty.RegisterAttached("BindingTargetProperty", typeof(DependencyProperty), typeof(VirtualTableCell), new UIPropertyMetadata(null));

      public static readonly DependencyProperty EditingElementProperty =
          DependencyProperty.Register("EditingElement", typeof(DataTemplate), typeof(VirtualTableCell), new UIPropertyMetadata(null));

      public static readonly DependencyProperty ElementProperty =
          DependencyProperty.Register("Element", typeof(DataTemplate), typeof(VirtualTableCell), new UIPropertyMetadata(null));

      public static readonly DependencyProperty IsEditingProperty;

      private static DependencyPropertyKey IsEditingPropertyKey;
      private ContentPresenter contentPresenter;

      static VirtualTableCell()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualTableCell), new FrameworkPropertyMetadata(typeof(VirtualTableCell)));
         IsEditingPropertyKey = DependencyProperty.RegisterReadOnly("IsEditing", typeof(bool), typeof(VirtualTableCell), new UIPropertyMetadata(false, OnIsEditingChanged));
         IsEditingProperty = IsEditingPropertyKey.DependencyProperty;
      }

      public VirtualTableCell()
      {
      }

      public BindingBase Binding { get; set; }

      public DataTemplate EditingElement
      {
         get { return (DataTemplate)GetValue(EditingElementProperty); }
         set { SetValue(EditingElementProperty, value); }
      }

      public DataTemplate Element
      {
         get { return (DataTemplate)GetValue(ElementProperty); }
         set { SetValue(ElementProperty, value); }
      }

      public bool IsEditing
      {
         get { return (bool)GetValue(IsEditingProperty); }
         private set { SetValue(IsEditingPropertyKey, value); }
      }

      public static DependencyProperty GetBindingTargetProperty(DependencyObject obj)
      {
         return (DependencyProperty)obj.GetValue(BindingTargetPropertyProperty);
      }

      public static void SetBindingTargetProperty(DependencyObject obj, DependencyProperty value)
      {
         obj.SetValue(BindingTargetPropertyProperty, value);
      }

      public override void OnApplyTemplate()
      {
         base.OnApplyTemplate();
         contentPresenter = UIUtils.GetVisualChild<ContentPresenter>(this);
         contentPresenter.Loaded += new RoutedEventHandler(contentPresenter_Loaded);
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

      public void UpdateFocus()
      {
         if (IsEditing)
         {
            Dispatcher.Invoke(DispatcherPriority.Input, new Action(() =>
            {
               var contentElement = UIUtils.GetFirstParkableControl(contentPresenter);
               if (contentElement != null)
                  contentElement.Focus();
            }));
         }
      }

      protected virtual void SetBindings(FrameworkElement primaryBindingTarget)
      {
      }

      private static void OnIsEditingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         //VirtualTableCell cell = sender as VirtualTableCell;
         //if (cell != null)
         //{
         //   cell.UpdateFocus();
         //}
      }

      private void contentPresenter_Loaded(object sender, RoutedEventArgs e)
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