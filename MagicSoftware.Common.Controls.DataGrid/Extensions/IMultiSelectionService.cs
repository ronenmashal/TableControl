using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface IMultiSelectionService
   {
      event SelectionChangedEventHandler SelectionChanged;

      FrameworkElement Element { get; }

      IList SelectedItems { get; }

      void ClearSelection();

      int SelectedIndex { get; set; }
      object SelectedItem { get; set; }

      void ToggleSelection(object hitItem);
   }

   [ImplementedService(typeof(IMultiSelectionService))]
   public abstract class MultiSelectionService<T> : IMultiSelectionService, IUIService
   where T : FrameworkElement
   {
      public abstract event SelectionChangedEventHandler SelectionChanged;

      public T Element { get; private set; }

      FrameworkElement IMultiSelectionService.Element { get { return Element; } }

      public abstract IList SelectedItems { get; }


      public abstract void ClearSelection();
      public abstract int SelectedIndex { get; set; }
      public abstract object SelectedItem { get; set; }
      public abstract bool ItemIsSelected(object item);
      public abstract void AddItemToSelection(object item);
      public abstract void RemoveItemFromSelection(object item);

      public virtual void AttachToElement(FrameworkElement element)
      {
         Element = (T) element;
      }

      public virtual void DetachFromElement(FrameworkElement element)
      {
         Element = null;
      }

      public bool IsAttached
      {
         get { return Element != null; }
      }

      public void Dispose()
      {
         DetachFromElement(Element);
      }

      public void ToggleSelection(object hitItem)
      {
         if (ItemIsSelected(hitItem))
            RemoveItemFromSelection(hitItem);
         else
            AddItemToSelection(hitItem);
      }
   }
}