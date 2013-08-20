using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CudaRuler.Data;
using CudaRuler.Data.MsWeb;
using CudaRuler.Helpers;

namespace CudaRuler.Algorithms
{
    public class MsWebAprioriAlgorithm : MsWebAbstractAlgorithm
    {
        public override void Run(ExecutionSettings executionSettings, bool printRules)
        {
            builder = new MsDataBuilder();
            var data = builder.BuildInstance(executionSettings);

            var frequentSets = data.Elements.Keys.Select(element => new List<int> { element }).ToList();

            frequentSets = frequentSets.Where(set => set.IsFrequent(data.Transactions, executionSettings.MinSup)).ToList();
            var frequentItemSets = frequentSets.ToDictionary(set => new FrequentItemSet<int>(set),
                                                             set => set.GetSupport(data.Transactions));
            List<List<int>> candidates;

            while ((candidates = GenerateCandidates(frequentSets)).Count > 0)
            {
                //! sprawdź czy któryś podzbiór k-1 elementowy kadydatów nie jest w frequentSets => wywal go!

                // leave only these sets which are frequent
                candidates =
                    candidates.Where(set => set.IsFrequent(data.Transactions, executionSettings.MinSup)).ToList();

                if (candidates.Count > 0)
                {
                    frequentSets = candidates;
                    foreach (var candidate in candidates)
                    {
                        var fItemSet = new FrequentItemSet<int>(candidate);

                        if (!frequentItemSets.ContainsKey(fItemSet))
                        {
                            frequentItemSets.Add(fItemSet, candidate.GetSupport(data.Transactions));
                        }
                    }
                }
                else
                {
                    // we don't have any more candidates
                    break;
                }
            }

            //here we should do something with the candidates
            var decisionRules = new List<DecisionRule<int>>();

            foreach (var frequentSet in frequentSets)
            {
                var subSets = EnumerableHelper.GetSubsets(frequentSet);

                foreach (var t in subSets)
                {
                    var leftSide = new FrequentItemSet<int>(t);
                    for (var j = 0; j < subSets.Count; j++)
                    {
                        var rightSide = new FrequentItemSet<int>(subSets[j]);
                        if (rightSide.ItemSet.Count != 1 || !FrequentItemSet<int>.SetsSeparated(rightSide, leftSide))
                        {
                            continue;
                        }

                        if (frequentItemSets.ContainsKey(leftSide))
                        {
                            var confidence = (double)frequentItemSets[new FrequentItemSet<int>(frequentSet)] / frequentItemSets[leftSide];
                            if (confidence >= executionSettings.MinConf)
                            {
                                var rule = new DecisionRule<int>(leftSide.ItemSet, rightSide.ItemSet, frequentItemSets[new FrequentItemSet<int>(frequentSet)], confidence);
                                decisionRules.Add(rule);
                            }
                        }
                    }
                }
            }

            if (!printRules) return;

            var result = PrintRules(decisionRules, executionSettings.DataSourcePath, executionSettings.MinSup,
                                    executionSettings.MinConf, data.Transactions.Keys.Count, data.Elements);
            Console.WriteLine(result);
        }

        public List<List<int>> GenerateCandidates(List<List<int>> source)
        {
            var candidates = new List<List<int>>();
            for (var i = 0; i < source.Count; i++)
            {
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (source[i].Take(source[i].Count - 1).AreEqual(source[j].Take(source[j].Count - 1)))
                    {
                        candidates.Add((List<int>)source[i].Merge(source[j]));
                    }
                }
            }


            return candidates;
        }
    }
}
