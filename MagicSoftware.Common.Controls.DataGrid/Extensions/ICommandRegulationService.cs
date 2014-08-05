using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   interface ICommandRegulationService
   {
      event CanExecuteRoutedEventHandler PreviewCanExecute;
      void ExecuteCommand(RoutedCommand command, object commandParameter);
   }
}
