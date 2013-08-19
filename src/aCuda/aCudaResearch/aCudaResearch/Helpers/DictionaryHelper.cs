using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Helpers
{
    public static class DictionaryHelper
    {
        /// <summary>
        /// Prints the dictionary content in the organized manner (could be
        /// later substituted by the writing result into the file)
        /// </summary>
        /// <param name="dictionary">File with the results</param>
        public static void Print(this Dictionary<AlgorithmType, long> dictionary)
        {
            foreach (var keyVale in dictionary)
            {
                Console.WriteLine("{0}\t{1}ms", keyVale.Key.ToString(), keyVale.Value);
            }
        }
    }
}
