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

   public abstract class MultiSelectionService<T> : IMultiSelectionService
   where T : FrameworkElement
   {
      public MultiSelectionService(FrameworkElement element)
      {
         this.Element = (T)element;
      }

      public abstract event SelectionChangedEventHandler SelectionChanged;

      public T Element { get; private set; }

      FrameworkElement IMultiSelectionService.Element { get { return Element; } }

      public abstract IList SelectedItems { get; }


      public abstract void SetSelectedItem(object item);
   }
}