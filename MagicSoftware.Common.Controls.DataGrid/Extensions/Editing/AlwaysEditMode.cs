using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal class AlwaysEditMode : DataGridEditModeBase
   {
      private EditModeState currentState;

      private Stack<IDisposable> deferedFocus = new Stack<IDisposable>();

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
         EditStateService.CanBeginEditChanged -= EditStateServiceOnCanBeginEditChanged;

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

         EditStateService.CanBeginEditChanged += EditStateServiceOnCanBeginEditChanged;
      }

      private void EditStateServiceOnCanBeginEditChanged(object sender, EventArgs eventArgs)
      {
         if (EditStateService.CanBeginEdit)
            BeginEditingCurrentItem();
      }

      internal bool BeginEditingCurrentItem()
      {
         try
         {
            if (!EditStateService.IsEditingItem)
            {
               log.DebugFormat("Keyboard focus is on {0}", Keyboard.FocusedElement);
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
         finally
         {
            DisposeDeferedFocus();
         }
      }

      internal override void ProcessKey(KeyEventArgs e)
      {
      }

      protected override bool CanLeaveCurrentCell()
      {
         if (EditStateService.IsEditingField)
         {
            var fms = UIServiceProvider.GetService<IFocusManagementService>(TargetElement);
            deferedFocus.Push(fms.DeferFocusUpdate());
            log.Debug("Leaving current cell: Commit");
            if (EditStateService.CommitFieldEdit())
               return true;
            else
            {
               DisposeDeferedFocus();
               return false;
            }
         }

         return true;
      }

      protected override bool CanLeaveCurrentLine()
      {
         if (EditStateService.IsEditingField)
         {
            log.Debug("Leaving current item: Commit");
            return EditStateService.CommitItemEdit();
         }

         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }

      private void DisposeDeferedFocus()
      {
         if (deferedFocus.Count > 0)
            deferedFocus.Pop().Dispose();
      }

      private void MoveToNextState()
      {
         TargetElement.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
         {
            var fms = UIServiceProvider.GetService<IFocusManagementService>(TargetElement);
            using (fms.DeferFocusUpdate())
            {
               log.Debug("Moving to next edit state");
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
         private ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

         public override EditModeState GetNextState()
         {
            if ((EditMode.EditStateService != null) && (EditMode.EditStateService.IsEditingItem && EditMode.EditStateService.IsEditingField))
            {
               log.DebugFormat("Edit service state: editing item = {0}, editing field = {1}", EditMode.EditStateService.IsEditingItem, EditMode.EditStateService.IsEditingField);
               return new IdleState();
            }

            return new BeginEditFailureRecoveryState();
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

         protected AlwaysEditMode EditMode { get; private set; }

         public void Enter(AlwaysEditMode editMode)
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
         private bool isChangingItem = false;
         private bool isKeyDown = false;

         public override EditModeState GetNextState()
         {
            if (isKeyDown && isChangingItem)
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
            currentCellService.PreviewCurrentCellChanging += currentCellService_PreviewCurrentCellChanging;
         }

         private void currentCellService_CurrentCellChanged(object sender, EventArgs e)
         {
            EditMode.MoveToNextState();
         }

         private void currentCellService_PreviewCurrentCellChanging(object sender, PreviewChangeEventArgs e)
         {
            if (e.NewValue == null || e.OldValue == null)
               isChangingItem = true;

            isChangingItem = ((UniversalCellInfo)e.NewValue).Item != ((UniversalCellInfo)e.OldValue).Item;
         }

         private void TargetElement_PreviewKeyDown(object sender, RoutedEventArgs e)
         {
            isKeyDown = ((KeyEventArgs)e).IsRepeat;
         }

         private void TargetElement_PreviewKeyUp(object sender, RoutedEventArgs e)
         {
            isKeyDown = false;
         }
      }

      private class BeginEditFailureRecoveryState : EditModeState
      {
         public override EditModeState GetNextState()
         {
            return new BeginEditState();
         }

         protected override void Cleanup()
         {
         }

         protected override void Setup()
         {
            if ((EditMode.EditStateService != null) && (!EditMode.EditStateService.IsEditingItem && EditMode.EditStateService.IsEditingField))
            {
               EditMode.EditStateService.CancelFieldEdit();
               EditMode.EditStateService.CancelItemEdit();

               if (EditMode.EditStateService.IsEditingItem || EditMode.EditStateService.IsEditingField)
               {
                  throw new Exception("Edit state failure");
               }
               EditMode.MoveToNextState();
            }
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