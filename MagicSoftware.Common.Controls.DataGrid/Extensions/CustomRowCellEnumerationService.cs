using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.CellTypes;
using log4net;
using MagicSoftware.Common.Utils;
using LogLevel = log4net.Core.Level;
using System.Windows.Media;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICellEnumerationService))]
   public class CustomRowCellEnumerationService : ICellEnumerationService, IUIService
   {
      /// <summary>
      /// Stores an table of 'current index' values.
      /// </summary>
      private static readonly DependencyProperty CurrentCellIndexTableProperty =
          DependencyProperty.RegisterAttached("CurrentCellIndexTable", typeof(Dictionary<object, int>), typeof(CustomRowCellEnumerationService), new UIPropertyMetadata(null));

      // Move this into dependency property.
      private List<VirtualTableCell> cells = new List<VirtualTableCell>();

      private int id;
      ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private ItemsControl owner;
      private DataGridRow row;

      public CustomRowCellEnumerationService(object rowTypeIdentifier)
      {
         id = IdGenerator.GetNewId(this);
         this.RowTypeIdentifier = rowTypeIdentifier;
      }

      public int CellCount
      {
         get 
         {
            if (cells.Count == 0)
               cells = row.GetDescendants<VirtualTableCell>();
            return cells.Count; 
         }
      }

      public int CurrentCellIndex
      {
         get
         {
            var indexTable = GetCurrentCellIndexTable(owner);
            int index;
            if (!indexTable.TryGetValue(RowTypeIdentifier, out index))
            {
               index = cells.Count > 0 ? 0 : -1;
               indexTable.Add(RowTypeIdentifier, index);
            }
            return index;
         }
      }

      public virtual bool IsAttached { get { return row != null; } }

      public object RowTypeIdentifier { get; private set; }

      public void AttachToElement(System.Windows.FrameworkElement element)
      {
         if (row != null)
         {
            if (object.ReferenceEquals(element, row))
               return;

            throw new InvalidOperationException(this.ToString() + " is already attached to row " + row.Item.ToString());
         }

         log.InfoFormat("Attaching {0} to {1}", this, element);
         row = element as DataGridRow;

         Debug.Assert(row != null);

         owner = UIUtils.GetAncestor<ItemsControl>(row);
         EnsureCurrentCellIndexTableExistance(owner);

         cells = row.GetDescendants<VirtualTableCell>();
      }

      public void DetachFromElement(System.Windows.FrameworkElement element)
      {
      }

      public void Dispose()
      {
         log.InfoFormat("Detaching {0} from {1}", this, row);

         if (cells != null && cells.Count > 0)
            cells.Clear();
         cells = null;
         owner = null;
         row = null;
      }

      public System.Windows.FrameworkElement GetCellAt(int index)
      {
         return cells[index];
      }

      public UniversalCellInfo GetCurrentCellInfo()
      {
         if (row == null)
            return new UniversalCellInfo(null, -1);
         return new UniversalCellInfo(row.Item, CurrentCellIndex);
      }

      public override string ToString()
      {
         return this.GetType().Name + " #" + id;
      }

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

}