using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.ViewModel;
using MagicSoftware.Common.Forms;
using MagicSoftware.Studio.Project;
using MagicSoftware.Studio.Repositories.Programs;
using MagicSoftware.Studio.Forms;
using MagicSoftware.Studio.Bridge;
using System.Runtime.InteropServices;
using MagicSoftware.Exp.Common.ViewModel;

namespace MagicStudio
{
   public class Project : MagicSoftware.Exp.Common.ViewModel.ObservableObject
   {
      public static ObservableProperty ProjectPathProperty = new ObservableProperty(x => ((Project)x).ProjectPath, typeof(Project), "");
      public static ObservableProperty IsLoadedProperty = new ObservableProperty(x => ((Project)x).IsLoaded, typeof(Project), false, ProjectPathProperty);
      public static ObservableProperty CurrentPresenterProperty = new ObservableProperty(x=> ((Project)x).CurrentPresenter, typeof(Project), null);

      private MagicSoftware.Studio.Project.ProjectManager projectManager;

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

      public PresenterBase CurrentPresenter
      {
         get { return (PresenterBase)CurrentPresenterProperty.GetValue(this); }
         set { CurrentPresenterProperty.SetValue(this, value); }
      }
      

      public static ObservableProperty SubTaskProperty = new ObservableProperty(x => ((Project)x).SubTask, typeof(Project), 2);
        public int SubTask { 
          get { 
            return (int)SubTaskProperty.GetValue(this); 
          } 
          set { 
            SubTaskProperty.SetValue(this, value);
          }
        }

      public Project(MagicSoftware.Studio.Project.ProjectManager projectManager)
      {
         // TODO: Complete member initialization
         this.projectManager = projectManager;
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
