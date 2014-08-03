using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MagicSoftware.Common.Controls.Table.Extensions.Editing
{
   internal class SingleLineEditStateMachine : DataGridEditStateMachine
   {
      private List<InputGesture> registeredGestures;

      public ICurrentItemService CurrentItemTracker { get; set; }

      public override void Cleanup()
      {
         base.Cleanup();
         InputService inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         foreach (var gesture in registeredGestures)
            inputService.UnregisterGestureAction(gesture);
      }

      public override void Setup()
      {
         base.Setup();
         registeredGestures = new List<InputGesture>();
         RegisterKeyGesture(Key.F2, ToggleEdit);
         RegisterKeyGesture(Key.Enter, ToggleEdit);
         RegisterKeyGesture(Key.Escape, CancelEdit);
      }

      internal override void ProcessKey(KeyEventArgs e)
      {
      }

      protected override bool CanLeaveCurrentLine()
      {
         var editStateService = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);
         if (editStateService.IsEditingField)
         {
            return editStateService.CommitItemEdit();
         }
         return true;
      }

      protected override void PreviewCanExecuteCommand(object commandTarget, CanExecuteRoutedEventArgs args)
      {
         args.CanExecute = true;
         args.Handled = true;
      }

      private void CancelEdit(KeyEventArgs e)
      {
         var editStateService = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);
         if (editStateService.IsEditingField)
            editStateService.CancelItemEdit();
         e.Handled = true;
      }

      private void RegisterKeyGesture(Key key, Action<KeyEventArgs> action)
      {
         InputService inputService = UIServiceProvider.GetService<InputService>(TargetElement);
         var gesture = new KeyGesture(key);
         registeredGestures.Add(gesture);
         inputService.RegisterKeyGestureAction(gesture, action);
      }

      private void ToggleEdit(KeyEventArgs e)
      {
         var editStateService = UIServiceProvider.GetService<IElementEditStateService>(TargetElement);
         if (editStateService.IsEditingField)
            editStateService.CommitItemEdit();
         else
            editStateService.BeginItemEdit();
         e.Handled = true;
      }
   }
}