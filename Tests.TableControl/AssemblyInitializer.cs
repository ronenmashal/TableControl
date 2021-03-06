﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Common;
using MagicSoftware.Common.Utils;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.Utils;

namespace Tests.TableControl
{
   [TestClass]
   public class AssemblyInitializer
   {
      [AssemblyInitialize()]
      public static void InitializeAssembly(TestContext testContext)
      {
         TestLogUtils.InitializeLog(testContext);
         // Force LoggingExtensions class initialization.
         using (LoggingExtensions.Indent()) { }

         FrameworkElementFormatter.GetInstance().SetTypeFormatter(typeof(DataGridRow), new DataGridRowFormatter());
      }
   }
}
