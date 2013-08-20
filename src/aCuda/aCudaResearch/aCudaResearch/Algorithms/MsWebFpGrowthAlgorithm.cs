using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CudaRuler.Algorithms.FpGrowth;
using CudaRuler.Data;
using CudaRuler.Data.MsWeb;

namespace CudaRuler.Algorithms
{
    /// <summary>
    /// FPGrowth Algorithm where the data is from the MsWeb dataset.
    /// </summary>
    public class MsWebFpGrowthAlgorithm : MsWebAbstractAlgorithm
    {
        public override void Run(ExecutionSettings executionSettings, bool printRules)
        {
            builder = new MsDataBuilder();
            var data = builder.BuildInstance(executionSettings);

            var compressedDatabase = FrequentSetGenerator.Generate(data.Transactions, executionSettings.MinSup);

            var tree = new FpTree<int>();
            tree.BuildFpTreeFromData(compressedDatabase);

            var treeExplorer = new TreeExplorer<int>((int)(data.Transactions.Keys.Count * executionSettings.MinSup));
            var rules = treeExplorer.GenerateRuleSet(tree, executionSettings.MinConf);

            if (!printRules) return;

            var result = PrintRules(rules, executionSettings.DataSourcePath, executionSettings.MinSup, executionSettings.MinConf, data.Transactions.Keys.Count, data.Elements);
            Console.WriteLine(result);
        }
    }
}
