using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using _DGTester.Validation;

namespace _DGTester.Data
{
   public class XXXValidationRule : ExtendedValidationRule
   {
      public override ExtendedValidationResult Validate(object dataItem, System.Windows.PropertyPath propertyPath, object newValue, System.Globalization.CultureInfo cultureInfo)
      {
         string strValue = newValue as string;
         if (strValue != null && strValue == "XXX")
         {
            return ExtendedValidationResult.Error("The value may not be XXX");
         }
         if (strValue != null && strValue == "YYY")
         {
            return ExtendedValidationResult.Warning("The value should not be YYY, but if you insist...");
         }
         return ExtendedValidationResult.Valid;
      }
   }
}
