using System;
using System.Collections.Generic;

namespace MagicStudio.SampleData
{
   internal class SampleProgramsRepository
   {
      public List<object> Programs { get; private set; }

      public SampleProgramsRepository()
      {
         Programs = new List<object>();
      }
   }
}