using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DataGridFocusManagementExtender : ElementExtenderBase<DataGrid>
   {
      ICurrentCellService currentCellService;

      protected override void Setup()
      {
         currentCellService = UIServiceProvider.GetService<ICurrentCellService>(TargetElement);
         currentCellService.CurrentCellChanged += new EventHandler(currentCellService_CurrentCellChanged);
      }

      protected override void Cleanup()
      {
         currentCellService.CurrentCellChanged -= new EventHandler(currentCellService_CurrentCellChanged);
         currentCellService = null;
      }

      void currentCellService_CurrentCellChanged(object sender, EventArgs e)
      {
         var currentCellElement = currentCellService.CurrentCellElement;
         if (currentCellElement == null)
            return;

         currentCellElement.Focus();
      }
   }
}
