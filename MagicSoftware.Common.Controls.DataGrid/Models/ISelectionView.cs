using System;
using System.ComponentModel;
using MagicSoftware.Common.ViewModel;

namespace MagicSoftware.Common.Controls.Table.Models
{
   public interface ISelectionView : INotifyPropertyChanged
   {
      bool IsMultiSelection { get; }

      object[] Selection { get; set; }
   }

   public class MultiSelectionView : ObservableObject, ISelectionView
   {
      private object[] selection;

      public bool IsMultiSelection
      {
         get { return true; }
      }

      public object[] Selection
      {
         get { return selection; }
         set
         {
            selection = value;
            OnPropertyChanged("Selection");
         }
      }
   }

   public class SingleItemSelectionView : ObservableObject, ISelectionView
   {
      private object selectedItem;

      object[] ISelectionView.Selection
      {
         get { return new object[] { selectedItem }; }
         set
         {
            if (value == null || value.Length == 0)
               Selection = null;
            Selection = value[0];
         }
      }

      public bool IsMultiSelection
      {
         get { return false; }
      }

      public object Selection
      {
         get { return selectedItem; }
         set
         {
            selectedItem = value;
            OnPropertyChanged("Selection");
         }
      }
   }
}