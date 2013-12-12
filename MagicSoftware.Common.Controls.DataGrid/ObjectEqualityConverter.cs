using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;

namespace MagicSoftware.Common.Controls.DataGrid
{
   class ObjectEqualityConverter : IMultiValueConverter
   {
      #region IMultiValueConverter Members

      public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         Debug.Assert(values.Length == 2);
         if (parameter != null && parameter.Equals("byref"))
         {
            return object.Equals(values[0], values[1]);
         }
         else
         {
            return object.ReferenceEquals(values[0], values[1]);
         }
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
      {
         throw new NotImplementedException();
      }

      #endregion
   }
}
