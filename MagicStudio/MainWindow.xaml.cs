using System;
using System.Windows;
using log4net;
using MagicSoftware.Studio;
using MagicSoftware.Studio.Bridge;

namespace MagicStudio
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public MainWindow()
      {
         InitializeComponent();
         InitializeStudioDll();
      }

      protected void InitializeStudioDll()
      {
         log.Info("Initializing the studio dll.");

         try
         {
            StudioDllWrapper.Initialize(new StudioDllImports());
            StudioInitializer.Initialize();
            StudioInitializer.InitializeConsumedAssemblies();
         }
         catch (System.Exception ex)
         {
            log.Error("Failed initializing the studio dll", ex);
            // Re-throw the exception to display the failure message.
            throw;
         }

         log.Debug("Finished initializing studio dll.");
      }

      protected override void OnClosed(EventArgs e)
      {
         base.OnClosed(e);
         StudioDllWrapper.Instance.StudioDllClean();
      }
   }
}