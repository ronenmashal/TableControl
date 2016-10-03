using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using log4net;
using MagicSoftware.Common.Controls.Extensibility;
using MagicSoftware.Common.Utils;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   /// <summary>
   /// Provides selection services such as connecting with an selected items view.
   /// </summary>
   public class SelectionExtender : ElementExtenderBase<ItemsControl>
   {
      #region Public Fields

      public static readonly DependencyProperty SelectionViewProperty =
          DependencyProperty.RegisterAttached("SelectionView", typeof(ObservableCollection<object>), typeof(SelectionExtender), new UIPropertyMetadata(new ObservableCollection<object>(), OnSelectionViewChanged));

      #endregion Public Fields

      #region Internal Fields

      internal static string LoggerName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".Selection";

      #endregion Internal Fields

      #region Private Fields

      private static readonly DependencyProperty SelectionExtenderProperty =
          DependencyProperty.RegisterAttached("SelectionExtender", typeof(SelectionExtender), typeof(SelectionExtender), new UIPropertyMetadata(null));

      private readonly AutoResetFlag suppressChangeHandling = new AutoResetFlag();
      private ILog log = log4net.LogManager.GetLogger(LoggerName);

      //private SelectionModeManager selectionModeManager;

      #endregion Private Fields

      #region Public Properties

      public bool IsAttached
      {
         get { return TargetElementProxy != null; }
      }

      #endregion Public Properties

      #region Private Properties

      private IMultiSelectionService TargetElementProxy { get; set; }

      #endregion Private Properties

      #region Public Methods

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

      ICurrentItemService currentItemTracker;
      readonly AutoResetFlag ignoreCurrentItemChangedEvent = new AutoResetFlag();

      public void AttachToElement(FrameworkElement element)
      {
         //if (TargetElement != null)
         //{
         //   if (ReferenceEquals(TargetElement.ElementSelectionService, element))
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

         currentItemTracker = UIServiceProvider.GetService<ICurrentItemService>(element, false);
         currentItemTracker.CurrentChanged += new EventHandler(currentItemTracker_CurrentChanged);
         //if (currentItemTracker != null)
         //{
         //   selectionModeManager = new SelectionModeManager(TargetElement);
         //}

         var inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         inputService.RegisterMouseActionGestures(ToggleSelection, new MouseGesturesFactory(MouseAction.LeftClick, ModifierKeys.Control));
         inputService.RegisterMouseActionGestures(SelectRange, new MouseGesturesFactory(MouseAction.LeftClick, ModifierKeys.Shift));
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
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(TargetElementProxy.Element);
      }

      #endregion Public Methods

      #region Protected Methods

      protected override void Cleanup()
      {
         DetachFromElement(TargetElement);
      }

      protected override void Setup()
      {
         AttachToElement(TargetElement);
      }

      #endregion Protected Methods

      #region Private Methods

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

      private object GetClickedItem(MouseEventArgs eventArgs)
      {
         var container = TargetElement.ItemContainerGenerator.ContainerFromIndex(0);
         if (container != null)
         {
            var containerType = container.GetType();
            var hitTestResult = VisualTreeHelper.HitTest(TargetElement, eventArgs.GetPosition(TargetElement));
            if (hitTestResult != null && hitTestResult.VisualHit != null)
            {
               var hitItemContainer = UIUtils.GetAncestor<FrameworkElement>(hitTestResult.VisualHit, (element) => element.GetType().Equals(containerType));
               var hitItem = TargetElement.ItemContainerGenerator.ItemFromContainer(hitItemContainer);
               return hitItem;
            }
         }
         return null;
      }

      readonly SelectionRange selectionRange = new SelectionRange();

      private void currentItemTracker_CurrentChanged(object sender, EventArgs e)
      {

         if (ignoreCurrentItemChangedEvent.IsSet)
            return;

         TargetElementProxy.SelectedItems.Clear();
         TargetElementProxy.SelectedItem = currentItemTracker.CurrentItem;
         selectionRange.SetAnchor(currentItemTracker.CurrentPosition);
      }

      void ToggleSelection(MouseEventArgs eventArgs)
      {
         var hitItem = GetClickedItem(eventArgs);
         if (hitItem != null)
         {
            TargetElementProxy.ToggleSelection(hitItem);
            using (ignoreCurrentItemChangedEvent.Set())
               currentItemTracker.MoveCurrentTo(hitItem);
            selectionRange.SetAnchor(currentItemTracker.CurrentPosition);
            eventArgs.Handled = true;
         }
      }

      void SelectRange(MouseEventArgs eventArgs)
      {
         var hitItem = GetClickedItem(eventArgs);
         if (hitItem != null)
         {
            var container = TargetElement.ItemContainerGenerator.ContainerFromItem(hitItem);
            var itemIndex = TargetElement.ItemContainerGenerator.IndexFromContainer(container);
            var selectionChange = selectionRange.GetRangeChange(itemIndex);
            foreach (var i in selectionChange.RemovedItems)
            {
               var itemContainer = TargetElement.ItemContainerGenerator.ContainerFromIndex(i);
               var itemToRemove = TargetElement.ItemContainerGenerator.ItemFromContainer(itemContainer);
               TargetElementProxy.SelectedItems.Remove(itemToRemove);
            }

            foreach (var i in selectionChange.AddedItems)
            {
               var itemContainer = TargetElement.ItemContainerGenerator.ContainerFromIndex(i);
               var itemToAdd = TargetElement.ItemContainerGenerator.ItemFromContainer(itemContainer);
               TargetElementProxy.SelectedItems.Add(itemToAdd);
            }

            selectionRange.SetEndIndex(itemIndex);

            eventArgs.Handled = true;
         }
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

      #endregion Private Methods
   }

   /// <summary>
   /// Represents a consequtive selection of items.
   /// </summary>
   class SelectionRange
   {
      private int rangeEndIndex;
      private int anchorItemIndex;

      public SelectionRange()
      {
         this.anchorItemIndex = -1;
         this.rangeEndIndex = -1;
      }

      public void SetAnchor(int newAnchorIndex)
      {
         anchorItemIndex = newAnchorIndex;
         rangeEndIndex = newAnchorIndex;
      }

      public SelectionRangeChange GetRangeChange(int newRangeEndIndex)
      {
         if (newRangeEndIndex < anchorItemIndex)
         {
            // New range goes upwards.

            if (rangeEndIndex < anchorItemIndex)
            {
               // Old range also went upwards.
               if (newRangeEndIndex < rangeEndIndex)
                  return SelectionRangeChange.AddedOnly(newRangeEndIndex, rangeEndIndex - 1);
               else if (newRangeEndIndex > rangeEndIndex)
                  return SelectionRangeChange.RemovedOnly(rangeEndIndex, newRangeEndIndex - 1);
            }
            else if (anchorItemIndex > newRangeEndIndex)
            {
               // Old range went downwards.
               return new SelectionRangeChange(anchorItemIndex + 1, rangeEndIndex, newRangeEndIndex, anchorItemIndex - 1);
            }
         }
         else if (newRangeEndIndex > anchorItemIndex)
         {
            // New range goes downwards.

            if (rangeEndIndex > anchorItemIndex)
            {
               // Old range also went downwards.
               if (newRangeEndIndex > rangeEndIndex)
                  return SelectionRangeChange.AddedOnly(newRangeEndIndex, rangeEndIndex + 1);
               else if (newRangeEndIndex < rangeEndIndex)
                  return SelectionRangeChange.RemovedOnly(rangeEndIndex, newRangeEndIndex + 1);
            }
            else if (anchorItemIndex < newRangeEndIndex)
            {
               // Old range went downwards.
               return new SelectionRangeChange(anchorItemIndex - 1, rangeEndIndex, newRangeEndIndex, anchorItemIndex + 1);
            }
         }
         else if (newRangeEndIndex != rangeEndIndex)
         {
            return SelectionRangeChange.RemovedOnly(anchorItemIndex, newRangeEndIndex);
         }

         return SelectionRangeChange.Empty;
      }

      internal void SetEndIndex(int itemIndex)
      {
         rangeEndIndex = itemIndex;
      }
   }

   class SelectionRangeChange
   {
      int firstRemovedIndex;
      int lastRemovedIndex;
      int firstAddedIndex;
      int lastAddedIndex;

      public SelectionRangeChange(int firstRemovedIndex, int lastRemovedIndex, int firstAddedIndex, int lastAddedIndex)
      {
         this.firstAddedIndex = Math.Min(firstAddedIndex, lastAddedIndex);
         this.lastAddedIndex = Math.Max(firstAddedIndex, lastAddedIndex);
         this.firstRemovedIndex = Math.Min(firstRemovedIndex, lastRemovedIndex);
         this.lastRemovedIndex = Math.Max(firstRemovedIndex, lastRemovedIndex);
      }

      public IEnumerable<int> RemovedItems
      {
         get
         {
            if (firstRemovedIndex > -1 && lastRemovedIndex > -1)
            {
               for (int i = firstRemovedIndex; i <= lastRemovedIndex; i++)
                  yield return i;
            }
         }
      }

      public IEnumerable<int> AddedItems
      {
         get
         {
            if (firstAddedIndex > -1 && lastAddedIndex > -1)
            {
               for (int i = firstAddedIndex; i <= lastAddedIndex; i++)
                  yield return i;
            }
         }
      }

      public static SelectionRangeChange RemovedOnly(int from, int to)
      {
         return new SelectionRangeChange(from, to, -1, -1);
      }

      public static SelectionRangeChange AddedOnly(int from, int to)
      {
         return new SelectionRangeChange(-1, -1, from, to);
      }

      public static readonly SelectionRangeChange Empty = new SelectionRangeChange(-1, -1, -1, -1);
   }
}