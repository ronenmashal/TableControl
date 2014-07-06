using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

         IItemsControlTraits traits = ControlTraitsFactory.GetTraitsFor(typeof(DataGrid));

         return new CustomRowCellEnumerationService(RowTypeIdentifier, traits);
      }
   }

   internal class CustomRowCellEnumerationService : CellEnumerationServiceBase
   {
      public CustomRowCellEnumerationService(object rowTypeIdentifier, IItemsControlTraits ownerTraits)
         : base(rowTypeIdentifier, ownerTraits)
      {
      }

      public override void UpdateCurrentCellIndex()
      {
         // Intentionally left blank.
      }

      protected override FrameworkElement GetCellContaining(UIElement element)
      {
         return UIUtils.GetAncestor<VirtualTableCell>(element);
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<VirtualTableCell>());
      }
   }
}