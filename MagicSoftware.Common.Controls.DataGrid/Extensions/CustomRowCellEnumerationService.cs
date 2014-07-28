using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.CellTypes;
using MagicSoftware.Common.Utils;
using System.Windows.Media;

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

         return new CustomRowCellEnumerationService(RowTypeIdentifier);
      }
   }

   internal class CustomRowCellEnumerationService : CellEnumerationServiceBase
   {
      private IList<FrameworkElement> cells;

      public CustomRowCellEnumerationService(object rowTypeIdentifier)
         : base(rowTypeIdentifier)
      {
      }

      public override int CellCount
      {
         get
         {
            if (cells.Count == 0)
               UpdateCellsCollection();
            return cells.Count;
         }
      }

      public override void AttachToElement(FrameworkElement element)
      {
         base.AttachToElement(element);
         UpdateCellsCollection();
      }

      public override void DetachFromElement(FrameworkElement element)
      {
         if (cells != null && cells.Count > 0)
            cells.Clear();
         cells = null;

         base.DetachFromElement(element);
      }

      public override FrameworkElement GetCellAt(int index)
      {
         if (cells.Count == 0)
            UpdateCellsCollection();
         return cells[index];
      }

      public override UniversalCellInfo GetCellContaining(DependencyObject dependencyObject)
      {
         var cell = GetCellContaining((Visual)dependencyObject);
         if (cells.Count == 0)
            UpdateCellsCollection();
         return new UniversalCellInfo(this.Row.Item, cells.IndexOf(cell));
      }

      public override UniversalCellInfo GetCellInfo(int displayIndex)
      {
         return new UniversalCellInfo(this.Row.Item, displayIndex);
      }

      public override int GetCellIndex(FrameworkElement cellElement)
      {
         return cells.IndexOf(cellElement);
      }

      protected override FrameworkElement GetCellContaining(Visual element)
      {
         return UIUtils.GetAncestor<VirtualTableCell>((Visual)element);
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<VirtualTableCell>());
      }

      protected void UpdateCellsCollection()
      {
         cells = GetCells();
      }
   }
}