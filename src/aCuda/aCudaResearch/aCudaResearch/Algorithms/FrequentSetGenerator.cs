using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Algorithms
{
    public static class FrequentSetGenerator
    {
        /// <summary>
        /// Generate the frequent sets from the input data
        /// </summary>
        /// <param name="dataIn">Input data</param>
        /// <param name="minSup">Minimal support for the frequent set</param>
        /// <returns>Elements of the frequent sets.</returns>
        public static Dictionary<int, int[]> Generate(Dictionary<int, int[]> dataIn, double minSup)
        {
            var frequentSets = new Dictionary<int, int>();

            foreach (var key in dataIn.Keys)
            {
                foreach (var value in dataIn[key])
                {
                    if (!frequentSets.ContainsKey(value))
                    {
                        frequentSets[value] = 1;
                    }
                    else
                    {
                        frequentSets[value]++;
                    }
                }
            }

            int[] tab = (from a in frequentSets
                         where ((double)a.Value / dataIn.Keys.Count) >= minSup
                         orderby a.Value descending
                         select a.Key).ToArray();

            var result = new Dictionary<int, int[]>();
            var attr = new List<int>();

            foreach (var key in dataIn.Keys)
            {
                var values = dataIn[key];

                attr.AddRange(tab.Where(value => values.Any(p => p == value)));

                if (attr.Count > 0)
                {
                    result.Add(key, attr.ToArray());
                    attr.Clear();
                }
            }

            return result;
        }
    }
}
