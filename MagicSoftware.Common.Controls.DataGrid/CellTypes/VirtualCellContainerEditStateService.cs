using System;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.Extensions;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.CellTypes
{
   [ImplementedService(typeof(IElementEditStateService))]
   public class VirtualCellContainerEditStateService : IElementEditStateService, IUIService
   {
      private readonly AutoResetFlag suppressEditStateEvent = new AutoResetFlag();
      private VirtualTableCell editingCell = null;

      public event EventHandler EditStateChanged;

      public bool IsAttached
      {
         get { return CellContainer != null; }
      }

      public bool IsEditingField
      {
         get { return editingCell != null; }
      }

      public bool IsEditingItem
      {
         get;
         private set;
      }

      private FrameworkElement CellContainer { get; set; }

      private FrameworkElement CellContainerOwner { get; set; }

      private ICellEnumerationService CellEnumerationService
      {
         get
         {
            return UIServiceProvider.GetService<ICellEnumerationService>(CellContainer);
         }
      }

      public void AttachToElement(System.Windows.FrameworkElement element)
      {
         CellContainer = element;
         CellContainerOwner = UIUtils.GetAncestor<ItemsControl>(element);
      }

      public bool BeginFieldEdit()
      {
         var currentCellService = UIServiceProvider.GetService<ICurrentCellService>(CellContainerOwner);
         editingCell = currentCellService.CurrentCellContainer as VirtualTableCell;
         if (editingCell != null)
         {
            if (editingCell.BeginEdit())
            {
               OnEditStateChanged();
               return true;
            }
         }
         return false;
      }

      public bool BeginItemEdit()
      {
         if (!IsEditingItem)
         {
            using (suppressEditStateEvent.Set())
               BeginFieldEdit();

            if (IsEditingField)
            {
               IsEditingItem = true;
               OnEditStateChanged();
               return true;
            }
         }
         return false;
      }

      public bool CancelFieldEdit()
      {
         if (IsEditingField)
         {
            if (editingCell.CancelEdit())
            {
               editingCell = null;
               OnEditStateChanged();
               return true;
            }
         }
         return false;
      }

      public bool CancelItemEdit()
      {
         if (IsEditingField)
         {
            using (suppressEditStateEvent.Set())
               CancelFieldEdit();
            if (IsEditingField)
               return false;
         }
         IsEditingItem = false;
         OnEditStateChanged();
         return true;
      }

      public bool CommitFieldEdit()
      {
         if (IsEditingField)
         {
            if (editingCell.CommitEdit())
            {
               editingCell = null;
               OnEditStateChanged();
               return true;
            }
         }
         return false;
      }

      public bool CommitItemEdit()
      {
         if (IsEditingField)
         {
            using (suppressEditStateEvent.Set())
               CommitFieldEdit();
            if (IsEditingField)
               return false;
         }
         IsEditingItem = false;
         OnEditStateChanged();
         return true;
      }

      public void DetachFromElement(System.Windows.FrameworkElement element)
      {
         CellContainer = null;
         CellContainerOwner = null;
      }

      public void Dispose()
      {
         if (IsAttached)
            DetachFromElement(CellContainer);
      }

      private void OnEditStateChanged()
      {
         if (EditStateChanged != null && !suppressEditStateEvent.IsSet)
            EditStateChanged(this, new EventArgs());
      }
   }
}