using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MagicSoftware.Common.Utils;
using log4net;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Provides selection services such as connecting with an selected items view.
   /// </summary>
   public class SelectionExtender : IUIService
   {
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
         if (TargetElement != null)
         {
            if (ReferenceEquals(TargetElement.Element, element))
            {
               log.DebugFormat("Extender {0} is already attached to element {1}", this, element);
               return;
            }
            throw new InvalidOperationException("Cannot re-attach an attached extender");
         }
            

         if (element is MultiSelector)
            TargetElement = new TrivialMultiSelectorAdapter((MultiSelector)element);
         else if (element is ListBox)
            TargetElement = new ListBoxMultiSelectorAdapter((ListBox)element);
         SetSelectionExtender(element, this);
         var selectionView = GetSelectionView(element);
         this.AttachSelectionModel(null, selectionView);
         TargetElement.SelectionChanged += TargetElement_SelectionChanged;
      }

      public void DetachFromElement(FrameworkElement element)
      {
         if (TargetElement == null)
         {
            log.WarnFormat("Detaching extender {0} from {1} for the second time", this, element);
            return;
         }
         SetSelectionExtender(element, null);
         TargetElement.SelectionChanged -= TargetElement_SelectionChanged;
         UnregisterSelectionModelEvents(GetSelectionView(TargetElement.Element));
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
            extender.AttachSelectionModel((ObservableCollection<object>)args.OldValue, (ObservableCollection<object>)args.NewValue);
         }
      }

      private void AttachSelectionModel(ObservableCollection<object> oldModel, ObservableCollection<object> newModel)
      {
         UnregisterSelectionModelEvents(oldModel);

         if (newModel != null)
            newModel.CollectionChanged += ItemsView_CollectionChanged;
      }

      private void UnregisterSelectionModelEvents(ObservableCollection<object> selectionModel)
      {
         if (selectionModel != null)
            selectionModel.CollectionChanged -= ItemsView_CollectionChanged;
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
      int selectionChangedHandlersCount = 0;

      public ListBoxMultiSelectorAdapter(ListBox element)
         : base(element)
      {
      }

      public override event SelectionChangedEventHandler SelectionChanged
      {
         add { Element.SelectionChanged += value; selectionChangedHandlersCount++; }
         remove { Element.SelectionChanged -= value; selectionChangedHandlersCount--; }
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
      int selectionChangedHandlersCount = 0;

      public TrivialMultiSelectorAdapter(MultiSelector element)
         : base(element)
      {
      }

      public override event SelectionChangedEventHandler SelectionChanged
      {
         add { Element.SelectionChanged += value; selectionChangedHandlersCount++; }
         remove { Element.SelectionChanged -= value; selectionChangedHandlersCount--; }
      }

      public override IList SelectedItems
      {
         get { return Element.SelectedItems; }
      }
   }
}