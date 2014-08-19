using System;
using System.Windows;
using com.magicsoftware.util;
using log4net;
using MagicSoftware.Studio;
using MagicSoftware.Studio.Bridge;
using MagicSoftware.Studio.Program;
using MagicSoftware.Studio.Program.Logic;
using MagicSoftware.Studio.Program.TaskItem;
using MagicSoftware.Studio.Project;

namespace MagicStudio
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private Project currentProject;
      private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public MainWindow()
      {
         InitializeComponent();
         InitializeStudioDll();

         currentProject = new Project(ProjectManager.Instance);
         currentProject.ProjectPath = @"d:\dev-magic\_Trunk\Sources\Projects\ggg\ggg.edp";
         currentProject.Load();
      }

      protected void InitializeStudioDll()
      {
         log.Info("Initializing the studio dll.");

         try
         {
            StudioDllWrapper.Initialize(new StudioDllImports());
            StudioInitializer.Initialize();
            StudioInitializer.InitializeConsumedAssemblies();

            ProjectManager.CreateTaskFactory = CreateTaskFactory;
            MainWindowHelper.HideMainWindow = HideMainWindow;

            ProjectManager.GoToBreakInvoke = GoToBreakInvoke;
            ProjectManager.CreateBreakInvoke = CreateBreakInvoke;
            ProjectManager.CreateVariableChangeInvoke = CreateVariableChangeInvoke;
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
         currentProject.Dispose();
         StudioDllWrapper.Instance.StudioDllClean();
         base.OnClosed(e);
      }

      #region Required for studio

      public object CreateBreakInvoke(Logic logic, BrkLevel brkLevel, BrkType brkType, string text, bool isZoom, ReferencedKey reference)
      {
         TaskLine targetTaskLine = logic.CreateBreak(brkLevel, brkType, text, isZoom);

         return targetTaskLine;
      }

      /// <summary>
      /// create task factory
      /// </summary>
      /// <param name="program"></param>
      /// <returns></returns>
      public TaskFactory CreateTaskFactory(MagicSoftware.Studio.Program.Program program)
      {
         return new TaskFactory(program);
      }

      public object CreateVariableChangeInvoke(Logic logic, BrkLevel brkLevel, BrkType brkType, string text, bool isZoom, ReferencedKey reference)
      {
         return logic.CreateVariableChangeBreak(reference);
      }

      public void GoToBreakInvoke(MagicSoftware.Studio.Program.Task task, int handel, int taskLineIsn, int breakIsn)
      {
         //NavigationPath navigationPath = new NavigationPath();
         //// navigate to task pane
         //navigationPath.Add(new NavigateToTaskEditor(this, task.Program.RootTask.TaskHeader.Isn));
         //// if needed, navigate to subtask
         //if (!task.IsProgram)
         //   navigationPath.Add(new NavigateToSubTask(new CreateTaskPathByIsn(task.Isn)));
         //// navigate to logic tab
         //navigationPath.Add(new NavigateInTabbedPresenter(MagicSoftware.Studio.Properties.Resources.LogicView_s));
         //// navigate to task line
         //navigationPath.Add(new NavigateToLine(new FindItemByISN(taskLineIsn)));

         //navigationPath.Execute(Dispatcher.CurrentDispatcher);
      }

      /// <summary>
      /// Hide the window
      /// </summary>
      public void HideMainWindow()
      {
         MainWindowHelper.Hide();
      }

      #endregion Required for studio
   }
}