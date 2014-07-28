using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using log4net;
using System.Windows.Media;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   [ImplementedService(typeof(ICellEnumerationService))]
   internal abstract class CellEnumerationServiceBase : ICellEnumerationService, IUIService
   {
      private int id;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public CellEnumerationServiceBase(object rowTypeIdentifier)
      {
         id = IdGenerator.GetNewId(this);
         this.ServiceGroupIdentifier = rowTypeIdentifier;
      }

      public abstract int CellCount { get; }

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
      }

      public virtual void DetachFromElement(FrameworkElement element)
      {
         log.InfoFormat("Detaching {0} from {1}", this, Row);
         Owner = null;
         Row = null;
      }

      public void Dispose()
      {
         DetachFromElement(Row);
      }

      public abstract FrameworkElement GetCellAt(int index);

      public abstract UniversalCellInfo GetCellContaining(DependencyObject dependencyObject);

      public abstract UniversalCellInfo GetCellInfo(int displayIndex);

      public virtual bool MoveToCell(int cellIndex)
      {
         if (Row == null)
            return false;

         return true;
      }

      public override string ToString()
      {
         return this.GetType().Name + " #" + id;
      }

      public abstract int GetCellIndex(FrameworkElement cellElement);

      protected abstract FrameworkElement GetCellContaining(Visual element);

      protected abstract IList<FrameworkElement> GetCells();
   }
}