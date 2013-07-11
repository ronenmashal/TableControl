using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _DGTester.Validation
{
   /// <summary>
   /// Enumeration detailing the result of a validation.
   /// </summary>
   public enum ResultType
   {
      /// <summary>
      /// Inform the object is valid.
      /// </summary>
      Valid = 0,

      /// <summary>
      /// Inform the object is invalid, with warnings.
      /// </summary>
      Warning = 1,

      /// <summary>
      /// Inform the object is severely invalid, with errors.
      /// </summary>
      Error = 2
   }
}
