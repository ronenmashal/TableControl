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

      void SetSelectedItem(object item);
   }

   [ImplementedService(typeof(IMultiSelectionService))]
   public abstract class MultiSelectionService<T> : IMultiSelectionService, IUIService
   where T : FrameworkElement
   {
      public abstract event SelectionChangedEventHandler SelectionChanged;

      public T Element { get; private set; }

      FrameworkElement IMultiSelectionService.Element { get { return Element; } }

      public abstract IList SelectedItems { get; }


      public abstract void SetSelectedItem(object item);

      public void AttachToElement(FrameworkElement element)
      {
         Element = (T) element;
      }

      public void DetachFromElement(FrameworkElement element)
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
   }
}