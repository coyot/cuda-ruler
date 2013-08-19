using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Helpers
{
    public static class EnumerableHelper
    {
        /// <summary>
        /// Returns the list of all subsets for defined set of elements.
        /// </summary>
        /// <param name="originalList">List of elements/</param>
        /// <returns>
        /// List of lists with the cardinality from 1 to n, where n is the cardinality of the input set.
        /// </returns>
        public static List<List<T>> GetSubsets<T>(IEnumerable<T> originalList)
        {
            var elements = originalList.ToArray();
            var subsets = new List<List<T>>();
            for (var i = 1; i < Math.Pow(2, originalList.Count()); i++)
            {
                var binary = Convert.ToString(i, 2);
                var subset = new List<T>();
                for (var j = 0; j < binary.Length; j++)
                {
                    if (binary[j] == '1')
                    {
                        subset.Add(elements[binary.Length - j - 1]);
                    }
                }
                subsets.Add(subset);
            }

            return subsets;
        }
    }
}
