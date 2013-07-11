using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;

namespace _DGTester.Validation
{
   public abstract class ExtendedValidationRule : ValidationRule
   {
      public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
      {
         return ValidationResult.ValidResult;
      }

      public abstract ExtendedValidationResult Validate(object dataItem, PropertyPath propertyPath, object newValue, CultureInfo cultureInfo);
   }
}
