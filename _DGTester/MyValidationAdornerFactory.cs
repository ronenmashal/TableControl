using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Extensibility.Controls.Extenders;

namespace _DGTester
{
   public class MyValidationAdornerFactory : IValidationAdornerFactory
   {
      #region IValidationAdornerFactory Members

      public System.Windows.Documents.Adorner CreateAdorner(Validation.ExtendedValidationResult validationResult, System.Windows.UIElement element)
      {
         if (validationResult.ShouldBlock)
            return new MyErrorAdorner(element) { Text = validationResult.Message };

         return new MyWarningAdorner(element) { Text = validationResult.Message };
      }

      #endregion
   }
}
