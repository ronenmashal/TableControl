using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Proxies;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface IElementEditStateService
   {
      bool BeginEdit();
      bool CommitEdit();
      bool CancelEdit();

      bool IsEditing { get; }
   }
}
