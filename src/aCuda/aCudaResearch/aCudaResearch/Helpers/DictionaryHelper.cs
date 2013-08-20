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
        public static string Print(this Dictionary<AlgorithmType, long> dictionary)
        {
            var sBuilder = new StringBuilder();

            foreach (var keyVale in dictionary)
            {
                sBuilder.Append(keyVale.Value)
                    .Append(" ");
            }
            sBuilder.AppendLine();
            return sBuilder.ToString();
        }
    }
}
