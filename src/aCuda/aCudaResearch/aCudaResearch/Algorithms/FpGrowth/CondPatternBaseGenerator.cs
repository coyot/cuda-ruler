using System.Collections.Generic;
using System.Linq;

namespace CudaRuler.Algorithms.FpGrowth
{
    /// <summary>
    /// Conditional pattern base generator.
    /// </summary>
    /// <typeparam name="T">Type of the element.</typeparam>
    public class CondPatternBaseGenerator<T>
    {
        /// <summary>
        /// Generates the conditional pattern base, based on the input data and the minimal support.
        /// </summary>
        /// <param name="database">Data with elements and its support</param>
        /// <param name="minSup">Minimal support for elements if they should be included in the pattern base.</param>
        /// <returns>
        /// Conditional pattern base as the dictionary with keys as list of nodes, and values their supports.
        /// </returns>
        public Dictionary<T[], int> Generate(Dictionary<List<TreeNode<T>>, int> database, int minSup)
        {
            var frequentSets = new Dictionary<T, int>();
            var result = new Dictionary<T[], int>();

            foreach (var key in database.Keys)
            {
                foreach (var node in key)
                {
                    if (frequentSets.ContainsKey(node.Value))
                        frequentSets[node.Value] += database[key];
                    else
                        frequentSets[node.Value] = database[key];
                }
            }

            foreach (var key in database.Keys)
            {
                var elements = (from node in key where frequentSets[node.Value] >= minSup select node.Value).ToList();

                if (elements.Count > 0)
                {
                    result[elements.ToArray()] = database[key];
                }
            }
            return result;
        }
    }
}
