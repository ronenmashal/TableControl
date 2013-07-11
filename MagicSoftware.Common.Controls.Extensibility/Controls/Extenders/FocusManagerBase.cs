using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MagicSoftware.Common.Controls.Extenders
{
   public abstract class FocusManagerBase : BehaviorExtenderBase
   {
      public FocusManagerBase()
      {
         CanUpdateFocus = true;
      }

      protected abstract void PerformFocusUpdate();

      public void UpdateFocus()
      {
         if (CanUpdateFocus)
            PerformFocusUpdate();
      }

      private bool CanUpdateFocus { get; set; }

      public IDisposable InhibitFocusUpdates()
      {
         return new FocusUpdatesSuppressor(this);
      }

      private class FocusUpdatesSuppressor : IDisposable
      {
         FocusManagerBase suppressedFocusManager;

         public FocusUpdatesSuppressor(FocusManagerBase suppressedFocusManager)
         {
            this.suppressedFocusManager = suppressedFocusManager;
            suppressedFocusManager.CanUpdateFocus = false;
         }

         #region IDisposable Members

         public void Dispose()
         {
            suppressedFocusManager.CanUpdateFocus = true;
         }

         #endregion
      }

   }
}
