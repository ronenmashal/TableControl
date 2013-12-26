using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Controls;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridFocusManagementExtender : ElementExtenderBase<DataGrid>
   {
      protected EnhancedDGProxy DataGridProxy { get { return (EnhancedDGProxy)TargetElementProxy; } }
      protected ICurrentItemProvider CurrentItemTracker { get; private set; }

      protected override void Setup()
      {
         CurrentItemTracker = DataGridProxy.GetAdapter<ICurrentItemProvider>();
         CurrentItemTracker.CurrentChanged += CurrentItemTracker_PropertyChanged;
      }

      protected override void Cleanup()
      {
         CurrentItemTracker.CurrentChanged -= CurrentItemTracker_PropertyChanged;
      }

      void CurrentItemTracker_PropertyChanged(object sender, RoutedEventArgs e)
      {
      }
   }
}
