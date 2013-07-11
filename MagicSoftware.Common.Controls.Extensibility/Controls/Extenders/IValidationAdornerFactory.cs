using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using _DGTester.Validation;
using System.Windows.Documents;

namespace MagicSoftware.Common.Controls.Extensibility.Controls.Extenders
{
   public interface IValidationAdornerFactory
   {
      Adorner CreateAdorner(ExtendedValidationResult validationResult, UIElement element);
   }
}
