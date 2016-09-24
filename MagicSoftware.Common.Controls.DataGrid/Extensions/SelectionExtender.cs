using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;
using MagicSoftware.Common.Utils;
using System.Collections.Generic;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Provides selection services such as connecting with an selected items view.
   /// </summary>
   public class SelectionExtender : ElementExtenderBase<ItemsControl>
   {
      public static readonly DependencyProperty SelectionViewProperty =
          DependencyProperty.RegisterAttached("SelectionView", typeof(ObservableCollection<object>), typeof(SelectionExtender), new UIPropertyMetadata(new ObservableCollection<object>(), OnSelectionViewChanged));

      internal static string LoggerName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".Selection";

      private static readonly DependencyProperty SelectionExtenderProperty =
          DependencyProperty.RegisterAttached("SelectionExtender", typeof(SelectionExtender), typeof(SelectionExtender), new UIPropertyMetadata(null));

      private readonly AutoResetFlag suppressChangeHandling = new AutoResetFlag();
      private ILog log = log4net.LogManager.GetLogger(LoggerName);

      public bool IsAttached
      {
         get { return TargetElementProxy != null; }
      }

      private IMultiSelectionService TargetElementProxy { get; set; }

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
         //if (TargetElement != null)
         //{
         //   if (ReferenceEquals(TargetElement.Element, element))
         //   {
         //      log.DebugFormat("Extender {0} is already attached to element {1}", this, element);
         //      return;
         //   }
         //   throw new InvalidOperationException("Cannot re-attach an attached extender");
         //}

         TargetElementProxy = UIServiceProvider.GetService<IMultiSelectionService>(element);
         SetSelectionExtender(element, this);
         var selectionView = GetSelectionView(element);
         this.AttachSelectionModel(null, selectionView);
         TargetElementProxy.SelectionChanged += TargetElement_SelectionChanged;

         var itemTracker = UIServiceProvider.GetService<ICurrentItemService>(TargetElementProxy.Element, false);
         if (itemTracker != null)
         {
            itemTracker.CurrentChanged += new EventHandler(itemTracker_CurrentChanged);
         }
      }

      void itemTracker_CurrentChanged(object sender, EventArgs e)
      {
         var itemTracker = UIServiceProvider.GetService<ICurrentItemService>(TargetElementProxy.Element);
         TargetElementProxy.SetSelectedItem(itemTracker.CurrentItem);
      }

      public void DetachFromElement(FrameworkElement element)
      {
         if (TargetElementProxy == null)
         {
            log.WarnFormat("Detaching extender {0} from {1} for the second time", this, element);
            return;
         }
         SetSelectionExtender(element, null);
         TargetElementProxy.SelectionChanged -= TargetElement_SelectionChanged;
         UnregisterSelectionModelEvents(GetSelectionView(TargetElementProxy.Element));
         TargetElementProxy = null;

         var itemTracker = UIServiceProvider.GetService<ICurrentItemService>(TargetElement, false);
         if (itemTracker != null)
         {
            itemTracker.CurrentChanged -= itemTracker_CurrentChanged;
         }
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(TargetElementProxy.Element);
      }

      protected override void Cleanup()
      {
         DetachFromElement(TargetElement);
      }

      protected override void Setup()
      {
         AttachToElement(TargetElement);
      }

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
                     TargetElementProxy.SelectedItems.Add(item);
                  }
               }

               if (e.OldItems != null)
               {
                  foreach (var item in e.OldItems)
                  {
                     TargetElementProxy.SelectedItems.Remove(item);
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

      private void UnregisterSelectionModelEvents(ObservableCollection<object> selectionModel)
      {
         if (selectionModel != null)
            selectionModel.CollectionChanged -= ItemsView_CollectionChanged;
      }

      class SelectionModeManager
      {
         class SelectionRange
         {
            public int fromItem;
            public int toItem;
         }

         List<SelectionRange> Ranges = new List<SelectionRange>();

         IMultiSelectionService elementSelectionAdaptor;

         public SelectionModeManager(IMultiSelectionService elementSelectionAdaptor)
         {
            this.elementSelectionAdaptor = elementSelectionAdaptor;
         }

         public void ClearSelection()
         {
            //elementSelectionAdaptor.ClearSelection();
         }
      }
   }
}