using System;
using System.Collections.Generic;
using System.Windows;
using MagicSoftware.Common.Controls.Table.CellTypes;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public class CustomRowCellEnumerationServiceFactory : IUIServiceFactory
   {
      public Type OwnerTraitsType { get; set; }

      public object RowTypeIdentifier { get; set; }

      public IUIService CreateUIService()
      {
         if (RowTypeIdentifier == null)
            throw new Exception("RowTypeIdentifier must be set on " + this.GetType().Name);

         if (OwnerTraitsType == null)
            throw new Exception("OwnerTraitsType must be set on " + this.GetType().Name);

         if (OwnerTraitsType.FindInterfaces((t, o) => { return t == typeof(IItemsControlTraits); }, null).Length == 0)
            throw new Exception("OwnerTraitsType must implement IItemsControlTraits");

         var traits = (IItemsControlTraits)Activator.CreateInstance(OwnerTraitsType);
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

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<VirtualTableCell>());
      }
   }
}