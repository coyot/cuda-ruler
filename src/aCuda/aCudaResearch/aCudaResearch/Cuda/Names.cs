using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Cuda
{
    public class Names
    {
        #region CudaModules

        public static string CudaAprioriCountModule = "apriori_count1.cubin";

        #endregion
        
        #region CudaFunctions

        public static string CountFrequency = "count_frequency";
        public static string CountSetsFrequencies = "count_sets_frequencies";
        
        #endregion
    }
}
