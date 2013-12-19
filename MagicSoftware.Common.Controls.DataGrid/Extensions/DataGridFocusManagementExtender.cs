using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridFocusManagementExtender : ElementExtenderBase<DataGrid>
   {
      protected EnhancedDGProxy DataGridProxy { get { return (EnhancedDGProxy)TargetElementProxy; } }
      protected ICurrentItemProvider CurrentItemTracker { get; private set; }

      protected override void Setup()
      {
         CurrentItemTracker = new DGProxyAsCurrentItemProvider(DataGridProxy);
         CurrentItemTracker.PropertyChanged += CurrentItemTracker_PropertyChanged;
      }

      protected override void Cleanup()
      {
         CurrentItemTracker.PropertyChanged -= CurrentItemTracker_PropertyChanged;
         CurrentItemTracker = null;
      }

      void CurrentItemTracker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
      }
   }
}
