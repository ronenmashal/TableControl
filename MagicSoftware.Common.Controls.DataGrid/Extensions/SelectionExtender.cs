using log4net;
using MagicSoftware.Common.Controls.Extensibility;
using MagicSoftware.Common.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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

      private readonly AutoResetFlag ignoreCurrentItemChangedEvent = new AutoResetFlag();
      private readonly SelectionRange selectionRange = new SelectionRange();
      private readonly AutoResetFlag suppressChangeHandling = new AutoResetFlag();
      private ICurrentItemService currentItemTracker;
      private Lazy<Type> ItemContainerType;
      private ILog log = log4net.LogManager.GetLogger(LoggerName);

      //private SelectionModeManager selectionModeManager;

      public bool IsAttached
      {
         get { return TargetElementProxy != null; }
      }

      private IMultiSelectionService TargetElementProxy { get; set; }

      public SelectionExtender()
      {
         ItemContainerType = new Lazy<Type>(GetItemContainerType);
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
         currentItemTracker.PreviewCurrentChanging += CurrentItemTracker_PreviewCurrentChanging;
         //if (currentItemTracker != null)
         //{
         //   selectionModeManager = new SelectionModeManager(TargetElement);
         //}

         var inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         inputService.RegisterMouseActionGestures(ToggleSelection, new MouseGesturesFactory(MouseAction.LeftClick, ModifierKeys.Control));
         inputService.RegisterKeyActionGestures(ToggleSelection, new KeyGesturesFactory(Key.Space, ModifierKeys.Control));
         //inputService.RegisterMouseActionGestures(SelectRange, new MouseGesturesFactory(MouseAction.LeftClick, ModifierKeys.Shift));
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

      private void currentItemTracker_CurrentChanged(object sender, EventArgs e)
      {
         if (ignoreCurrentItemChangedEvent.IsSet)
            return;

         bool shiftIsPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
         bool controlIsPressed = (Keyboard.Modifiers & ModifierKeys.Control) != 0;

         if (!shiftIsPressed)
         {
            if (!controlIsPressed)
            {
               TargetElementProxy.SelectedItems.Clear();
               TargetElementProxy.SelectedItem = currentItemTracker.CurrentItem;
               EnableEditing();
            }
            selectionRange.AnchorItemIndex = currentItemTracker.CurrentPosition;
            return;
         }

         DisableEditing();

         SelectRange(currentItemTracker.CurrentItem);
      }

      private void CurrentItemTracker_PreviewCurrentChanging(object sender, PreviewChangeEventArgs previewChangeEventArgs)
      {
         if (ignoreCurrentItemChangedEvent.IsSet)
            return;

         bool shiftIsPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
         bool controlIsPressed = (Keyboard.Modifiers & ModifierKeys.Control) != 0;

         if (shiftIsPressed || controlIsPressed)
         {
            if (!DisableEditing())
            {
               previewChangeEventArgs.Canceled = true;
            }
         }
      }

      private bool DisableEditing()
      {
         var editSvc = UIServiceProvider.GetService<IElementEditStateService>(TargetElement, false);
         if (editSvc != null)
         {
            return editSvc.DisableEditing();
         }
         return true;
      }

      private void EnableEditing()
      {
         var editSvc = UIServiceProvider.GetService<IElementEditStateService>(TargetElement, false);
         if (editSvc != null)
         {
            editSvc.EnableEditing();
         }
      }

      private object GetClickedItem(MouseEventArgs eventArgs)
      {
         var containerType = ItemContainerType.Value;
         if (containerType != null)
         {
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

      private Type GetItemContainerType()
      {
         if (TargetElement != null)
         {
            var itemContainerGenerator = ((IItemContainerGenerator)TargetElement.ItemContainerGenerator);
            var pos = new GeneratorPosition(-1, 0);
            using (itemContainerGenerator.StartAt(pos, GeneratorDirection.Forward))
            {
               bool isNew;
               var container = (FrameworkElement)itemContainerGenerator.GenerateNext(out isNew);
               if (container != null)
               {
                  return container.GetType();
               }
            }
         }
         return null;
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

      private void SelectRange(object upToItem)
      {
         DisableEditing();
         var container = TargetElement.ItemContainerGenerator.ContainerFromItem(upToItem);
         var itemIndex = TargetElement.ItemContainerGenerator.IndexFromContainer(container);
         var selectionChange = selectionRange.GetRangeChange(itemIndex);
         foreach (var i in selectionChange.RemovedItems)
         {
            var itemToRemove = TargetElement.Items.GetItemAt(i);
            TargetElementProxy.SelectedItems.Remove(itemToRemove);
         }

         foreach (var i in selectionChange.AddedItems)
         {
            var itemToAdd = TargetElement.Items.GetItemAt(i);
            TargetElementProxy.SelectedItems.Add(itemToAdd);
         }

         selectionRange.EndItemIndex = itemIndex;
      }

      private void SelectRange(MouseEventArgs eventArgs)
      {
         var hitItem = GetClickedItem(eventArgs);
         if (hitItem != null)
         {
            DisableEditing();
            var container = TargetElement.ItemContainerGenerator.ContainerFromItem(hitItem);
            var itemIndex = TargetElement.ItemContainerGenerator.IndexFromContainer(container);
            var selectionChange = selectionRange.GetRangeChange(itemIndex);
            foreach (var i in selectionChange.RemovedItems)
            {
               var itemToRemove = TargetElement.Items.GetItemAt(i);
               TargetElementProxy.SelectedItems.Remove(itemToRemove);
            }

            foreach (var i in selectionChange.AddedItems)
            {
               var itemToAdd = TargetElement.Items.GetItemAt(i);
               TargetElementProxy.SelectedItems.Add(itemToAdd);
            }

            selectionRange.EndItemIndex = itemIndex;

            eventArgs.Handled = true;
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

      private void ToggleSelection(KeyEventArgs eventArgs)
      {
         DisableEditing();
         TargetElementProxy.ToggleSelection(currentItemTracker.CurrentItem);
      }

      private void ToggleSelection(MouseEventArgs eventArgs)
      {
         var hitItem = GetClickedItem(eventArgs);
         if (hitItem != null)
         {
            TargetElementProxy.ToggleSelection(hitItem);
         }
      }

      private void UnregisterSelectionModelEvents(ObservableCollection<object> selectionModel)
      {
         if (selectionModel != null)
            selectionModel.CollectionChanged -= ItemsView_CollectionChanged;
      }
   }

   /// <summary>
   /// Represents a consequtive selection of items.
   /// </summary>
   internal class SelectionRange
   {
      private int anchorItemIndex;
      private int rangeEndIndex;

      public int AnchorItemIndex
      {
         get { return anchorItemIndex; }
         set
         {
            anchorItemIndex = value;
            rangeEndIndex = value;
         }
      }

      internal int EndItemIndex
      {
         get { return rangeEndIndex; }
         set { rangeEndIndex = value; }
      }

      public SelectionRange()
      {
         this.anchorItemIndex = -1;
         this.rangeEndIndex = -1;
      }

      public SelectionRangeChange GetRangeChange(int newRangeEndIndex)
      {
         var direction = Math.Sign(rangeEndIndex - anchorItemIndex);
         var newDirection = Math.Sign(newRangeEndIndex - anchorItemIndex);

         if (direction == 0)
         {
            if (newDirection == 0)
               return SelectionRangeChange.Empty;

            return SelectionRangeChange.AddedOnly(anchorItemIndex + newDirection, newRangeEndIndex);
         }

         if (newDirection == 0)
         {
            return SelectionRangeChange.RemovedOnly(anchorItemIndex + direction, rangeEndIndex);
         }

         if (newDirection != direction)
         {
            return new SelectionRangeChange(anchorItemIndex + direction, rangeEndIndex, anchorItemIndex + newDirection, newRangeEndIndex);
         }

         // Compute the indexes diff and set appropriate sign, so that:
         // endIndexesDiff > 0 ==> newRange contains more items than current range.
         // endIndexesDiff < 0 ==> newRange contains less items than current range.
         var endIndexesDiff = (newRangeEndIndex - rangeEndIndex) * direction;

         if (endIndexesDiff < 0)
         {
            // Exclude the last item of the new range from the removed items range.
            newRangeEndIndex += direction;
            return SelectionRangeChange.RemovedOnly(rangeEndIndex, newRangeEndIndex);
         }

         if (endIndexesDiff > 0)
         {
            // Exclude the last item of the old range from the added items range.
            var addedItemIndex = rangeEndIndex + direction;
            return SelectionRangeChange.AddedOnly(addedItemIndex, newRangeEndIndex);
         }

         // Nothing was changed.
         return SelectionRangeChange.Empty;
      }
   }

   internal class SelectionRangeChange
   {
      public static readonly SelectionRangeChange Empty = new SelectionRangeChange(-1, -1, -1, -1);
      private int firstAddedIndex;
      private int firstRemovedIndex;
      private int lastAddedIndex;
      private int lastRemovedIndex;

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

      public SelectionRangeChange(int firstRemovedIndex, int lastRemovedIndex, int firstAddedIndex, int lastAddedIndex)
      {
         this.firstAddedIndex = Math.Min(firstAddedIndex, lastAddedIndex);
         this.lastAddedIndex = Math.Max(firstAddedIndex, lastAddedIndex);
         this.firstRemovedIndex = Math.Min(firstRemovedIndex, lastRemovedIndex);
         this.lastRemovedIndex = Math.Max(firstRemovedIndex, lastRemovedIndex);
      }

      public static SelectionRangeChange AddedOnly(int from, int to)
      {
         return new SelectionRangeChange(-1, -1, from, to);
      }

      public static SelectionRangeChange RemovedOnly(int from, int to)
      {
         return new SelectionRangeChange(from, to, -1, -1);
      }
   }
}