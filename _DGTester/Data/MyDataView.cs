using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MagicSoftware.Common.ViewModel;

namespace _DGTester.Data
{
   class MyDataView : ObservableObject, IEditableObject
   {
      private string stringVal = "";

      public string StringVal 
      {
         get { return stringVal; }
         set
         {
            if (value == "XXX")
               throw new ArgumentException("Value cannot be XXX");
            stringVal = value;
            OnPropertyChanged("StringVal");
         }
      }
      public bool BoolVal { get; set; }
      public int IntVal { get; set; }
      public float FloatVal { get; set; }
      public Animal Animal { get; set; }

      #region IEditableObject Members

      public void BeginEdit()
      {
         
      }

      public void CancelEdit()
      {
         
      }

      public void EndEdit()
      {
         
      }

      #endregion

      public override string ToString()
      {
         return String.Format("MyDataView: {0},{1},{2}", StringVal, IntVal, Animal);
      }
   }
}
