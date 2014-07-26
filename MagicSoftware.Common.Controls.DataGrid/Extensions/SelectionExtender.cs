using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Provides selection services such as connecting with an selected items view.
   /// </summary>
   public class SelectionExtender : IUIService
   {
      public static readonly DependencyProperty SelectionViewProperty =
          DependencyProperty.RegisterAttached("SelectionView", typeof(ObservableCollection<object>), typeof(SelectionExtender), new UIPropertyMetadata(new ObservableCollection<object>(), OnSelectionViewChanged));

      public bool IsAttached
      {
         get { return TargetElement != null; }
      }

      public static SelectionExtender GetSelectionExtender(DependencyObject obj)
      {
         return (SelectionExtender)obj.GetValue(SelectionExtenderProperty);
      }

      public static ObservableCollection<object> GetSelectionView(DependencyObject obj)
      {
         return (ObservableCollection<object>)obj.GetValue(SelectionViewProperty);
      }

      public static void SetSelectionExtender(DependencyObject obj, SelectionExtender value)
      {
         if (obj != null)
            obj.SetValue(SelectionExtenderProperty, value);
      }

      public static void SetSelectionView(DependencyObject obj, ObservableCollection<object> value)
      {
         if (obj != null)
            obj.SetValue(SelectionViewProperty, value);
      }

      public void AttachToElement(FrameworkElement element)
      {
         if (element is MultiSelector)
            TargetElement = new TrivialMultiSelectorAdapter((MultiSelector)element);
         else if (element is ListBox)
            TargetElement = new ListBoxMultiSelectorAdapter((ListBox)element);
         SetSelectionExtender(element, this);
         var selectionView = GetSelectionView(element);
         this.AttachSelectionView(null, selectionView);
         TargetElement.SelectionChanged += TargetElement_SelectionChanged;
      }

      public void DetachFromElement(FrameworkElement element)
      {
         SetSelectionExtender(element, null);
         TargetElement.SelectionChanged -= TargetElement_SelectionChanged;
         TargetElement = null;
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(TargetElement.Element);
      }

      private static readonly DependencyProperty SelectionExtenderProperty =
          DependencyProperty.RegisterAttached("SelectionExtender", typeof(SelectionExtender), typeof(SelectionExtender), new UIPropertyMetadata(null));

      private readonly AutoResetFlag suppressChangeHandling = new AutoResetFlag();

      private IMultiSelectorAdapter TargetElement { get; set; }

      private static void OnSelectionViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
      {
         var extender = GetSelectionExtender(obj);
         if (extender != null)
         {
            extender.AttachSelectionView((ObservableCollection<object>)args.OldValue, (ObservableCollection<object>)args.NewValue);
         }
      }

      private void AttachSelectionView(ObservableCollection<object> oldView, ObservableCollection<object> newView)
      {
         if (oldView != null)
            oldView.CollectionChanged -= ItemsView_CollectionChanged;

         if (newView != null)
            newView.CollectionChanged += ItemsView_CollectionChanged;
      }

      private void ItemsView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (!suppressChangeHandling.IsSet)
         {
            using (suppressChangeHandling.Set())
            {
               if (e.NewItems != null)
               {
                  foreach (var item in e.NewItems)
                  {
                     TargetElement.SelectedItems.Add(item);
                  }
               }

               if (e.OldItems != null)
               {
                  foreach (var item in e.OldItems)
                  {
                     TargetElement.SelectedItems.Remove(item);
                  }
               }
            }
         }
      }

      private void TargetElement_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
         if (!suppressChangeHandling.IsSet)
         {
            using (suppressChangeHandling.Set())
            {
               var selectedItemsCollection = GetSelectionView((DependencyObject)sender);

               foreach (var item in e.AddedItems)
               {
                  selectedItemsCollection.Add(item);
               }

               foreach (var item in e.RemovedItems)
               {
                  selectedItemsCollection.Remove(item);
               }
            }
         }
      }
   }

   internal interface IMultiSelectorAdapter
   {
      event SelectionChangedEventHandler SelectionChanged;

      FrameworkElement Element { get; }

      IList SelectedItems { get; }
   }

   internal class ListBoxMultiSelectorAdapter : MultiSelectorAdapter<ListBox>
   {
      public ListBoxMultiSelectorAdapter(ListBox element)
         : base(element)
      {
      }

      public override event SelectionChangedEventHandler SelectionChanged
      {
         add { Element.SelectionChanged += value; }
         remove { Element.SelectionChanged -= value; }
      }

      public override IList SelectedItems
      {
         get { return Element.SelectedItems; }
      }
   }

   internal abstract class MultiSelectorAdapter<T> : IMultiSelectorAdapter
      where T : FrameworkElement
   {
      public MultiSelectorAdapter(FrameworkElement element)
      {
         this.Element = (T)element;
      }

      public abstract event SelectionChangedEventHandler SelectionChanged;

      public T Element { get; private set; }

      FrameworkElement IMultiSelectorAdapter.Element { get { return Element; } }

      public abstract IList SelectedItems { get; }
   }

   internal class TrivialMultiSelectorAdapter : MultiSelectorAdapter<MultiSelector>
   {
      public TrivialMultiSelectorAdapter(MultiSelector element)
         : base(element)
      {
      }

      public override event SelectionChangedEventHandler SelectionChanged
      {
         add { Element.SelectionChanged += value; }
         remove { Element.SelectionChanged -= value; }
      }

      public override IList SelectedItems
      {
         get { return Element.SelectedItems; }
      }
   }
}