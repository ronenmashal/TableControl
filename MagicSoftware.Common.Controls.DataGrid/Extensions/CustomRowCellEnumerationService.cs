using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Utils;
using log4net;
using MagicSoftware.Common.Controls.Table.CellTypes;

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

   [ImplementedService(typeof(ICellEnumerationService))]
   internal abstract class CellEnumerationServiceBase : ICellEnumerationService, IUIService
   {
      /// <summary>
      /// Stores an table of 'current index' values.
      /// </summary>
      private static readonly DependencyProperty CurrentCellIndexTableProperty =
          DependencyProperty.RegisterAttached("CurrentCellIndexTable", typeof(Dictionary<object, int>), typeof(ICellEnumerationService), new UIPropertyMetadata(null));

      // Move this into dependency property.
      private IList<FrameworkElement> cells;

      private int id;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private DataGrid owner;
      protected DataGridRow Row { get; private set; }

      public CellEnumerationServiceBase(object rowTypeIdentifier)
      {
         id = IdGenerator.GetNewId(this);
         this.ServiceGroupIdentifier = rowTypeIdentifier;
      }

      public int CellCount
      {
         get
         {
            if (cells.Count == 0)
               cells = GetCells();
            return cells.Count;
         }
      }

      public int CurrentCellIndex
      {
         get
         {
            var indexTable = GetCurrentCellIndexTable(owner);
            int index;
            if (!indexTable.TryGetValue(ServiceGroupIdentifier, out index))
            {
               index = cells.Count > 0 ? 0 : -1;
               indexTable.Add(ServiceGroupIdentifier, index);
            }
            return index;
         }
         private set
         {
            var indexTable = GetCurrentCellIndexTable(owner);
            indexTable[ServiceGroupIdentifier] = value;
         }
      }

      public virtual bool IsAttached { get { return Row != null; } }

      public object ServiceGroupIdentifier { get; private set; }

      public void AttachToElement(System.Windows.FrameworkElement element)
      {
         if (Row != null)
         {
            if (object.ReferenceEquals(element, Row))
               return;

            throw new InvalidOperationException(this.ToString() + " is already attached to row " + Row.Item.ToString());
         }

         log.InfoFormat("Attaching {0} to {1}", this, element);
         Row = element as DataGridRow;

         Debug.Assert(Row != null);

         owner = UIUtils.GetAncestor<DataGrid>(Row);
         EnsureCurrentCellIndexTableExistance(owner);

         cells = GetCells();
      }

      public void DetachFromElement(System.Windows.FrameworkElement element)
      {
      }

      public void Dispose()
      {
         log.InfoFormat("Detaching {0} from {1}", this, Row);

         if (cells != null && cells.Count > 0)
            cells.Clear();
         cells = null;
         owner = null;
         Row = null;
      }

      public System.Windows.FrameworkElement GetCellAt(int index)
      {
         return cells[index];
      }

      public UniversalCellInfo GetCurrentCellInfo()
      {
         if (Row == null)
            return new UniversalCellInfo(null, -1);
         return new UniversalCellInfo(Row.Item, CurrentCellIndex);
      }

      public bool MoveToCell(int cellIndex)
      {
         if (Row == null)
            return false;

         owner.CurrentCell = new DataGridCellInfo(Row.Item, owner.ColumnFromDisplayIndex(0));
         CurrentCellIndex = cellIndex;
         return true;
      }

      public override string ToString()
      {
         return this.GetType().Name + " #" + id;
      }

      protected abstract IList<FrameworkElement> GetCells();

      private static void EnsureCurrentCellIndexTableExistance(DependencyObject obj)
      {
         if (GetCurrentCellIndexTable(obj) == null)
            SetCurrentCellIndexTable(obj, new Dictionary<object, int>());
      }

      private static Dictionary<object, int> GetCurrentCellIndexTable(DependencyObject obj)
      {
         return (Dictionary<object, int>)obj.GetValue(CurrentCellIndexTableProperty);
      }

      private static void SetCurrentCellIndexTable(DependencyObject obj, Dictionary<object, int> value)
      {
         obj.SetValue(CurrentCellIndexTableProperty, value);
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

   internal class StandardRowCellEnumerationService : CellEnumerationServiceBase
   {
      public StandardRowCellEnumerationService()
         : base("_default_")
      {
      }

      protected override IList<FrameworkElement> GetCells()
      {
         return new List<FrameworkElement>(Row.GetDescendants<DataGridCell>());
      }
   }
}