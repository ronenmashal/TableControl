using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Proxies;

namespace MagicSoftware.Common.Controls.Extenders
{
   public abstract class DataGridExtenderBase : BehaviorExtenderBase
   {
      protected DataGridProxy DGProxy { get { return (DataGridProxy)Proxy; } }
      protected System.Windows.Controls.DataGrid AttachedDG { get { return (System.Windows.Controls.DataGrid)AttachedElement; } }
   }
}
