using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility.Controls.Extenders;
using System.Windows.Documents;

namespace _DGTester
{
   public class MyValidationAdornerFactory : IValidationAdornerFactory
   {
      #region IValidationAdornerFactory Members

      public System.Windows.Documents.Adorner CreateAdorner(Validation.ExtendedValidationResult validationResult, System.Windows.UIElement element)
      {
         Adorner adorner;

         if (validationResult.ShouldBlock)
            adorner= new MyErrorAdorner(element) { Text = validationResult.Message };
         else
            adorner = new MyWarningAdorner(element) { Text = validationResult.Message };
         adorner.IsHitTestVisible = false;
         return adorner;
      }

      #endregion
   }
}
