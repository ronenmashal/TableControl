using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   class DataGridColumnArrangeExtender : DataGridExtenderBase
   {
      protected override void Initialize()
      {
         AttachedDG.Loaded += new System.Windows.RoutedEventHandler(AttachedDG_Loaded);
      }

      void AttachedDG_Loaded(object sender, System.Windows.RoutedEventArgs e)
      {
         ReorderColumns();
      }

      public override void DetachFromElement()
      {
         AttachedDG.Loaded -= new System.Windows.RoutedEventHandler(AttachedDG_Loaded);
      }

      void ReorderColumns()
      {
         int i = 0;
         foreach (var column in AttachedDG.Columns)
         {
            i++;
            column.DisplayIndex = AttachedDG.Columns.Count - i;
         }
      }
   }
}
