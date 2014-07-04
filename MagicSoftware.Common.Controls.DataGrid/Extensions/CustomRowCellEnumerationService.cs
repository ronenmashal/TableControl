using System;
using System.Collections.Generic;
using System.Windows;
using MagicSoftware.Common.Controls.Table.CellTypes;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class CustomRowCellEnumerationServiceFactory : IUIServiceFactory
   {
      public object RowTypeIdentifier { get; set; }

      public IUIService CreateUIService()
      {
         if (RowTypeIdentifier == null)
            throw new Exception("RowTypeIdentifier must be set on " + this.GetType().Name);

         return new CustomRowCellEnumerationService(RowTypeIdentifier);
      }
   }

   internal class CustomRowCellEnumerationService : CellEnumerationServiceBase
   {
      public CustomRowCellEnumerationService(object rowTypeIdentifier)
         : base(rowTypeIdentifier)
      {
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<VirtualTableCell>());
      }
   }
}