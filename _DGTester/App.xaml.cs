using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace _DGTester
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      protected override void OnStartup(StartupEventArgs e)
      {
         var catalog = new AggregateCatalog(new DirectoryCatalog("."), new AssemblyCatalog(Assembly.GetExecutingAssembly()));
         var container = new CompositionContainer(catalog);
         var batch = new CompositionBatch();
         container.Compose(batch);

         log4net.Config.XmlConfigurator.Configure();
      }
   }
}
