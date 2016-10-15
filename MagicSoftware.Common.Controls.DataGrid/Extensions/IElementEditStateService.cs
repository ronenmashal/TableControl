using System;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface IElementEditStateService
   {
      bool CanBeginEdit { get; }

      bool IsEditingField { get; }

      bool IsEditingItem { get; }

      event EventHandler CanBeginEditChanged;

      event EventHandler EditStateChanged;

      bool BeginFieldEdit();

      bool BeginItemEdit();

      bool CancelFieldEdit();

      bool CancelItemEdit();

      bool CommitFieldEdit();

      bool CommitItemEdit();

      bool DisableEditing();

      bool EnableEditing();
   }
}