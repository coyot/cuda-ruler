using System;
using System.Collections.Generic;
using System.Linq;
using aCudaResearch.Data;
using aCudaResearch.Data.MsWeb;
using aCudaResearch.Helpers;

namespace aCudaResearch.Algorithms
{
    public class MsWebParallelAprioriAlgorithm : MsWebAbstractAlgorithm
    {
        public override void Run(ExecutionSettings executionSettings, bool printRules)
        {
            builder = new MsDataBuilder();
            var data = builder.BuildInstance(executionSettings);

            var frequentSets = data.Elements.Keys.Select(element => new List<int> { element }).AsParallel().ToList();

            frequentSets = frequentSets.Where(set => set.IsFrequent(data.Transactions, executionSettings.MinSup)).AsParallel().ToList();
            var frequentItemSets = frequentSets.AsParallel().ToDictionary(set => new FrequentItemSet<int>(set),
                                                             set => set.GetSupport(data.Transactions));
            List<List<int>> candidates;

            while ((candidates = GenerateCandidates(frequentSets)).Count > 0)
            {
                //! sprawdź czy któryś podzbiór k-1 elementowy kadydatów nie jest w frequentSets => wywal go!

                // leave only these sets which are frequent
                candidates =
                    candidates.Where(set => set.IsFrequentParallel(data.Transactions, executionSettings.MinSup)).AsParallel().ToList();

                if (candidates.Count > 0)
                {
                    frequentSets = candidates;
                    foreach (var candidate in candidates)
                    {
                        frequentItemSets.Add(new FrequentItemSet<int>(candidate), candidate.GetSupportParallel(data.Transactions));
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

                        if (!frequentItemSets.ContainsKey(leftSide)) continue;
                        var confidence = (double)frequentItemSets[new FrequentItemSet<int>(frequentSet)] / frequentItemSets[leftSide];
                        if (confidence >= executionSettings.MinConf)
                        {
                            var rule = new DecisionRule<int>(leftSide.ItemSet, rightSide.ItemSet, frequentItemSets[new FrequentItemSet<int>(frequentSet)], confidence);
                            decisionRules.Add(rule);
                        }
                    }
                }
            }

            if (!printRules) return;

            var result = PrintRules(decisionRules, executionSettings.DataSourcePath, executionSettings.MinSup, executionSettings.MinConf, data.Transactions.Keys.Count, data.Elements);
            Console.WriteLine(result);
        }

        public List<List<int>> GenerateCandidates(List<List<int>> source)
        {
            var candidates = new List<List<int>>();
            for (var i = 0; i < source.Count; i++)
            {
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (source[i].Take(source[i].Count - 1).AreEqualParallel(source[j].Take(source[j].Count - 1)))
                    {
                        candidates.Add(source[i].Merge(source[j]).AsParallel().ToList());
                    }
                }
            }

            return candidates;
        }
    }
}
