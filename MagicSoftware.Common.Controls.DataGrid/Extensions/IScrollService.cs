using System;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   internal interface IVerticalScrollService
   {
      int ItemsPerPage { get; }

      bool ScrollDown(uint distance);

      bool ScrollTo(object item);

      bool ScrollToBottom();

      bool ScrollToTop();

      bool ScrollUp(uint distance);
   }
}