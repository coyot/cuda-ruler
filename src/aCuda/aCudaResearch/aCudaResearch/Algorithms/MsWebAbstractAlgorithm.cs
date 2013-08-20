using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aCudaResearch.Data;
using aCudaResearch.Data.MsWeb;

namespace aCudaResearch.Algorithms
{
    /// <summary>
    /// Representation of the algorithm which uses MSWeb Data as the input.
    /// </summary>
    public abstract class MsWebAbstractAlgorithm : IAlgorithm
    {
        internal MsDataBuilder builder;

        public abstract void Run(ExecutionSettings executionSettings, bool printResults);

        internal static string PrintRules(IList<DecisionRule<int>> rules, string inputFile, double minSupport, double minConfidence, int databaseSize, Dictionary<int, MsElement> elements)
        {
            var sb = new StringBuilder();
            sb.Append(inputFile + "\n");
            sb.Append(minSupport + "\n");
            sb.Append(minConfidence + "\n");
            sb.Append(rules.Count + "\n\n\n");
            sb.Append("** RULES\n\n");

            for (var i = 0; i < rules.Count; i++)
            {
                sb.Append((i + 1) + ": " + rules[i].ToString(databaseSize, elements) + "\n\n");
            }

            return sb.ToString();
        }
    }
}
