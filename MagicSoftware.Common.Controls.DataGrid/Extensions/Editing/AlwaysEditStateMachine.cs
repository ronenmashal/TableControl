using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal class AlwaysEditStateMachine : DataGridEditStateMachine
   {
      private EditModeState currentState;

      private IElementEditStateService EditStateService
      {
         get
         {
            var itemsService = UIServiceProvider.GetService<ICurrentCellService>(this.TargetElement);
            if (itemsService == null)
               return null;

            var editStateService = UIServiceProvider.GetService<IElementEditStateService>(itemsService.CurrentItemContainer, false);
            if (editStateService == null)
            {
               // Current item does not have an edit state service - get the ItemsControl's edit state service.
               editStateService = UIServiceProvider.GetService<IElementEditStateService>(this.TargetElement);
            }
            return editStateService;
         }
      }

      public override void Cleanup()
      {
         currentState.Leave();
         currentState = null;
         base.Cleanup();
      }

      public override void Setup()
      {
         base.Setup();

         TargetElement.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() =>
         {
            currentState = new BeginEditState();
            currentState.Enter(this);
         }));
      }

      internal bool BeginEditingCurrentItem()
      {
         if (!EditStateService.IsEditingItem)
         {
            if (!EditStateService.BeginItemEdit())
            {
               log.Warn("Failed beginning edit on current item");
               return false;
            }
         }
         if (!EditStateService.IsEditingField)
         {
            if (!EditStateService.BeginFieldEdit())
            {
               log.Warn("Failed beginning edit on current field");
               return false;
            }
         }
         return true;
      }

      internal override void ProcessKey(KeyEventArgs e)
      {
      }

      protected override bool CanLeaveCurrentCell()
      {
         if (EditStateService.IsEditingField)
         {
            return EditStateService.CommitFieldEdit();
         }

         return true;
      }

      protected override bool CanLeaveCurrentLine()
      {
         if (EditStateService.IsEditingField)
         {
            return EditStateService.CommitItemEdit();
         }

         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }

      private void MoveToNextState()
      {
         TargetElement.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
               log.Debug("Moving to next edit state");
               var fms = UIServiceProvider.GetService<IFocusManagementService>(TargetElement);
               using (fms.DeferFocusUpdate())
               {
                  EditModeState nextState = currentState.GetNextState();
                  currentState.Leave();
                  currentState = nextState;
                  currentState.Enter(this);
               }
            }));
      }

      #region Edit mode states

      internal class BeginEditState : EditModeState
      {
         public override EditModeState GetNextState()
         {
            if ((EditMode.EditStateService != null) && (EditMode.EditStateService.IsEditingField))
               return new IdleState();

            return this;
         }

         protected override void Cleanup()
         {
         }

         protected override void Setup()
         {
            EditMode.BeginEditingCurrentItem();
            EditMode.MoveToNextState();
         }
      }

      internal abstract class EditModeState
      {
         private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

         protected AlwaysEditStateMachine EditMode { get; private set; }

         public void Enter(AlwaysEditStateMachine editMode)
         {
            log.DebugFormat("Entering edit mode state {0}", this.GetType().Name);
            this.EditMode = editMode;
            Setup();
         }

         public abstract EditModeState GetNextState();

         public void Leave()
         {
            log.DebugFormat("Leaving edit mode state {0}", this.GetType().Name);
            Cleanup();
            this.EditMode = null;
         }

         protected abstract void Cleanup();

         protected abstract void Setup();
      }

      internal class IdleState : EditModeState
      {
         private ICurrentCellService currentCellService;
         private bool isKeyDown = false;

         public override EditModeState GetNextState()
         {
            if (isKeyDown)
               return new WaitForKeyUpState();
            else
               return new BeginEditState();
         }

         protected override void Cleanup()
         {
            EditMode.TargetElement.RemoveHandler(FrameworkElement.PreviewKeyDownEvent, (RoutedEventHandler)TargetElement_PreviewKeyDown);
            EditMode.TargetElement.RemoveHandler(FrameworkElement.PreviewKeyUpEvent, (RoutedEventHandler)TargetElement_PreviewKeyUp);

            currentCellService = UIServiceProvider.GetService<ICurrentCellService>(EditMode.TargetElement);
            currentCellService.CurrentCellChanged -= currentCellService_CurrentCellChanged;
         }

         protected override void Setup()
         {
            EditMode.TargetElement.AddHandler(FrameworkElement.PreviewKeyDownEvent, (RoutedEventHandler)TargetElement_PreviewKeyDown, true);
            EditMode.TargetElement.AddHandler(FrameworkElement.PreviewKeyUpEvent, (RoutedEventHandler)TargetElement_PreviewKeyUp, true);

            currentCellService = UIServiceProvider.GetService<ICurrentCellService>(EditMode.TargetElement);
            currentCellService.CurrentCellChanged += currentCellService_CurrentCellChanged;
         }

         private void currentCellService_CurrentCellChanged(object sender, EventArgs e)
         {
            EditMode.MoveToNextState();
         }

         private void TargetElement_PreviewKeyDown(object sender, RoutedEventArgs e)
         {
            isKeyDown = true;
         }

         private void TargetElement_PreviewKeyUp(object sender, RoutedEventArgs e)
         {
            isKeyDown = false;
         }
      }

      private class WaitForKeyUpState : EditModeState
      {
         public override EditModeState GetNextState()
         {
            return new BeginEditState();
         }

         protected override void Cleanup()
         {
            EditMode.TargetElement.RemoveHandler(FrameworkElement.PreviewKeyUpEvent, (RoutedEventHandler)TargetElement_PreviewKeyUp);
         }

         protected override void Setup()
         {
            EditMode.TargetElement.AddHandler(FrameworkElement.PreviewKeyUpEvent, (RoutedEventHandler)TargetElement_PreviewKeyUp, true);
         }

         private void TargetElement_PreviewKeyUp(object sender, RoutedEventArgs e)
         {
            EditMode.MoveToNextState();
         }
      }

      #endregion Edit mode states
   }
}