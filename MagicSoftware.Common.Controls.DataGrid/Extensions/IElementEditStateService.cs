using System;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface IElementEditStateService
   {
      event EventHandler EditStateChanged;

      bool IsEditingField { get; }

      bool IsEditingItem { get; }

      bool BeginFieldEdit();

      bool BeginItemEdit();

      bool CancelFieldEdit();

      bool CancelItemEdit();

      bool CommitFieldEdit();

      bool CommitItemEdit();
   }
}