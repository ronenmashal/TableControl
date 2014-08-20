using System;
using System.Runtime.InteropServices;
using MagicSoftware.Common.Forms;
using MagicSoftware.Exp.Common.ViewModel;
using MagicSoftware.Studio.Bridge;
using MagicSoftware.Studio.DataElement;
using MagicSoftware.Studio.Project;
using MagicSoftware.Studio.Repositories.Programs;

namespace MagicStudio
{
   public class Project : MagicSoftware.Exp.Common.ViewModel.ObservableObject
   {
      public static ObservableProperty IsLoadedProperty;
      public static ObservableProperty ProjectPathProperty;
      public static ObservableProperty SubTaskProperty;

      private MagicSoftware.Studio.Project.ProjectManager projectManager;

      static Project()
      {
         ProjectPathProperty = new ObservableProperty(x => ((Project)x).ProjectPath, typeof(Project), "");
         IsLoadedProperty = new ObservableProperty(x => ((Project)x).IsLoaded, typeof(Project), false, ProjectPathProperty);
         SubTaskProperty = new ObservableProperty(x => ((Project)x).SubTask, typeof(Project), 2);
      }

      public Project(MagicSoftware.Studio.Project.ProjectManager projectManager)
      {
         // TODO: Complete member initialization
         this.projectManager = projectManager;
      }

      public bool IsLoaded
      {
         get
         {
            return (bool)IsLoadedProperty.GetValue(this);
         }
         private set
         {
            IsLoadedProperty.SetValue(this, value);
         }
      }

      public MultipleElementManager Programs
      {
         get
         {
            if (IsLoaded)
               return projectManager.MainComponent.ProgramsRepository;
            else
               return null;
         }
      }

      public string ProjectPath
      {
         get
         {
            return (string)ProjectPathProperty.GetValue(this);
         }
         set
         {
            ProjectPathProperty.SetValue(this, value);
         }
      }

      public int SubTask
      {
         get
         {
            return (int)SubTaskProperty.GetValue(this);
         }
         set
         {
            SubTaskProperty.SetValue(this, value);
         }
      }

      public bool Load()
      {
         StudioDllWrapper.MESSAGE_INFO messageResult = new StudioDllWrapper.MESSAGE_INFO();
         var ReturnResult = ProjectManager.Instance.OpenProjectPhase1(ProjectPath);
         ReturnResult = ProjectManager.Instance.OpenProjectPhase2(ProjectPath, ReturnResult);

         messageResult = (StudioDllWrapper.MESSAGE_INFO)Marshal.PtrToStructure(ReturnResult, typeof(StudioDllWrapper.MESSAGE_INFO));
         if (messageResult.Code != StudioDllWrapper.MESSAGE_CODE.CODE_SUCCESS)
            return false;

         IsLoaded = true;
         return true;
      }

      public TaskHeader SelectProgram()
      {
         ISelectionDialog dlg = SelectionDialogFactory.CreateDialog(2, ZoomType.SelectProgram, null, null);
         TaskHeader selectedTask = null;
         if ((bool)dlg.ShowDialog())
         {
            selectedTask = projectManager.GetPrgByIdx((int)dlg.SelectedItem);
         }
         return selectedTask;
      }
   }
}