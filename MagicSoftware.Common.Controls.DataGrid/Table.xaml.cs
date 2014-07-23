using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using log4net;
using MagicSoftware.Common.Controls.Table.Extensions;
using MagicSoftware.Common.Controls.Table.Models;
using MagicSoftware.Common.Controls.Table.Utils;

namespace MagicSoftware.Common.Controls.Table
{
   /// <summary>
   /// Interaction logic for Table.xaml
   /// </summary>
   public partial class Table : UserControl
   {
      public static readonly DependencyProperty ItemsSourceProperty =
          DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Table), new UIPropertyMetadata(null));

      public static readonly DependencyProperty RowStyleSelectorProperty =
          DependencyProperty.Register("RowStyleSelector", typeof(StyleSelector), typeof(Table), new UIPropertyMetadata(null, OnRowStyleSelectorChanged));

      public static readonly DependencyProperty SelectionViewProperty =
          DependencyProperty.Register("SelectionView", typeof(SelectionView), typeof(Table), new UIPropertyMetadata(null, OnSelectionViewChanged));

      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public Table()
      {
         InitializeComponent();
      }

      public ItemsControlEditMode EditMode
      {
         get { return (ItemsControlEditMode)DataGridEditingExtender.GetEditMode(rootItemsControl); }
         set { DataGridEditingExtender.SetEditMode(rootItemsControl, value); }
      }

      public IEnumerable ItemsSource
      {
         get { return (IEnumerable)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }

      public StyleSelector RowStyleSelector
      {
         get { return (StyleSelector)GetValue(RowStyleSelectorProperty); }
         set { SetValue(RowStyleSelectorProperty, value); }
      }

      public SelectionView SelectionView
      {
         get { return (SelectionView)GetValue(SelectionViewProperty); }
         set { SetValue(SelectionViewProperty, value); }
      }

      private static void OnRowStyleSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
      {
         var table = sender as Table;
         if (table != null)
         {
            ((InternalRowStyleSelector)(table.rootItemsControl.RowStyleSelector)).WrappedSelector = changeArgs.NewValue as StyleSelector;
         }
      }

      private static void OnSelectionViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
      {
         var table = sender as Table;
         if (table != null)
         {
            DataGridSelectionService.SetSelectionView(table.rootItemsControl, (SelectionView)args.NewValue);
         }
      }
   }

   internal class InternalRowStyleSelector : StyleSelector
   {
      public StyleSelector WrappedSelector { get; set; }

      public override Style SelectStyle(object item, DependencyObject container)
      {
         Style selectedStyle = null;
         if (WrappedSelector != null)
            selectedStyle = WrappedSelector.SelectStyle(item, container);

         if (selectedStyle == null)
            selectedStyle = ((FrameworkElement)container).TryFindResource("TableRowStyle") as Style;

         if (selectedStyle != null)
            return selectedStyle;
         return base.SelectStyle(item, container);
      }
   }
}