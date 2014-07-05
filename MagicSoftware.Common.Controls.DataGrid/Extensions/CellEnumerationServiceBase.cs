using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using log4net;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
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
      IItemsControlTraits ownerTraits;

      public CellEnumerationServiceBase(object rowTypeIdentifier, IItemsControlTraits ownerTraits)
      {
         id = IdGenerator.GetNewId(this);
         this.ServiceGroupIdentifier = rowTypeIdentifier;
         this.ownerTraits = ownerTraits;
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
            var indexTable = GetCurrentCellIndexTable(Owner);
            int index;
            if (!indexTable.TryGetValue(ServiceGroupIdentifier, out index))
            {
               index = cells.Count > 0 ? 0 : -1;
               indexTable.Add(ServiceGroupIdentifier, index);
            }
            return index;
         }
         protected set
         {
            var indexTable = GetCurrentCellIndexTable(Owner);
            indexTable[ServiceGroupIdentifier] = value;
         }
      }

      public virtual bool IsAttached { get { return Row != null; } }

      public object ServiceGroupIdentifier { get; private set; }

      protected ItemsControl Owner { get; private set; }

      protected DataGridRow Row { get; private set; }

      public virtual void AttachToElement(System.Windows.FrameworkElement element)
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

         Owner = UIUtils.GetAncestor<ItemsControl>(Row);
         EnsureCurrentCellIndexTableExistance(Owner);

         cells = GetCells();
      }

      public virtual void DetachFromElement(FrameworkElement element)
      {
         log.InfoFormat("Detaching {0} from {1}", this, Row);

         if (cells != null && cells.Count > 0)
            cells.Clear();
         cells = null;
         Owner = null;
         Row = null;
      }

      public void Dispose()
      {
         DetachFromElement(Row);
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

      public virtual bool MoveToCell(int cellIndex)
      {
         if (Row == null)
            return false;

         ownerTraits.SetCurrentCell(Owner, new UniversalCellInfo(Row.Item, cellIndex));
         CurrentCellIndex = cellIndex;
         return true;
      }

      public override string ToString()
      {
         return this.GetType().Name + " #" + id;
      }

      public abstract void UpdateCurrentCellIndex();

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
}