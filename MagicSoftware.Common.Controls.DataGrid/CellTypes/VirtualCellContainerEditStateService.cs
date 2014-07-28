using System;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.Extensions;

namespace MagicSoftware.Common.Controls.Table.CellTypes
{
   [ImplementedService(typeof(IElementEditStateService))]
   public class VirtualCellContainerEditStateService : IElementEditStateService, IUIService
   {
      private VirtualTableCell editingCell = null;

      public bool IsAttached
      {
         get { return CellContainer != null; }
      }

      public bool IsEditing
      {
         get { return editingCell != null; }
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

      public bool BeginEdit()
      {
         if (!IsEditing)
         {
            var currentCellService = UIServiceProvider.GetService<ICurrentCellService>(CellContainerOwner);
            editingCell = currentCellService.CurrentCellContainer as VirtualTableCell;
            if (editingCell != null)
            {
               if (editingCell.BeginEdit())
                  return true;
            }
         }
         return false;
      }

      public bool CancelEdit()
      {
         if (IsEditing)
         {
            if (editingCell.CancelEdit())
            {
               editingCell = null;
               return true;
            }
         }
         return false;
      }

      public bool CommitEdit()
      {
         if (IsEditing)
         {
            if (editingCell.CommitEdit())
            {
               editingCell = null;
               return true;
            }
         }
         return false;
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
   }
}