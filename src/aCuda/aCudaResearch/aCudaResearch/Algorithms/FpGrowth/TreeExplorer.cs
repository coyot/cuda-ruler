using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aCudaResearch.Data;
using aCudaResearch.Helpers;

namespace aCudaResearch.Algorithms.FpGrowth
{
    public class TreeExplorer<T>
    {
        readonly int _minSupport;

        /// <summary>
        /// Dictionary where keys are frequent sets, and the values their supports.
        /// </summary>
        public Dictionary<FrequentItemSet<T>, int> FrequentItemSets { get; private set; }

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="minSupport">
        /// Minimal support which is used during the FP-tree exploration process.
        /// </param>
        public TreeExplorer(int minSupport)
        {
            FrequentItemSets = new Dictionary<FrequentItemSet<T>, int>();
            _minSupport = minSupport;
        }

        

        /// <summary>
        /// Fp-growth procedure for specified tree and the conditional base pattern.
        /// </summary>
        /// <param name="tree">FP-tree</param>
        /// <param name="alpha">The cinditional base pattern</param>
        private void FpGrowth(FpTree<T> tree, IEnumerable<T> alpha)
        {
            if (tree.HasSinglePath())
            {
                var path = tree.GetSinglePath();

                var elements = path.Select(node => node.Value).ToList();

                var values = EnumerableHelper.GetSubsets(elements);
                foreach (var list in values)
                {
                    if (list.Count <= 0) continue;

                    var support = int.MaxValue;
                    foreach (var node in path)
                    {
                        if (node.TransactionCounter < support)
                        {
                            support = node.TransactionCounter;
                        }
                    }

                    var itemSet = new List<T>(list);
                    itemSet.AddRange(alpha);
                    var frequentItemSet = new FrequentItemSet<T>(itemSet);
                    FrequentItemSets.Add(frequentItemSet, support);
                }
                return;
            }

            var sortedFrequencyList = tree.GetFrequencyList();

            foreach (var list in sortedFrequencyList)
            {
                foreach (var value in list.Value)
                {
                    var beta = new List<T>(alpha);
                    beta.Insert(0, value);
                    var patternBase = tree.GetCondPatternBase(value);
                    FrequentItemSets.Add(new FrequentItemSet<T>(beta), tree.GetSupport(value));

                    var baseGenerator = new CondPatternBaseGenerator<T>();
                    var database = baseGenerator.Generate(patternBase, _minSupport);
                    if (database.Count > 0)
                    {
                        var newTree = new FpTree<T>();
                        newTree.BuildConditionalTree(database);
                        FpGrowth(newTree, beta);
                    }
                }
            }
        }

        /// <summary>
        /// Generate the decision rules based on the specisied FP-tree and the minimal
        /// confidence.
        /// </summary>
        /// <param name="tree">FP-tree</param>
        /// <param name="minConfidence">The minimal confidence</param>
        /// <returns></returns>
        public List<DecisionRule<T>> GenerateRuleSet(FpTree<T> tree, double minConfidence)
        {
            FpGrowth(tree, new List<T>());

            var decisionRules = new List<DecisionRule<T>>();
            foreach (var frequentItemSet in FrequentItemSets.Keys)
            {
                if (frequentItemSet.ItemSet.Count < 2)
                {
                    continue;
                }

                var subSets = EnumerableHelper.GetSubsets(frequentItemSet.ItemSet);

                foreach (var t in subSets)
                {
                    var leftSide = new FrequentItemSet<T>(t);
                    for (var j = 0; j < subSets.Count; j++)
                    {
                        var rightSide = new FrequentItemSet<T>(subSets[j]);
                        if (rightSide.ItemSet.Count != 1 || !FrequentItemSet<T>.SetsSeparated(rightSide, leftSide))
                        {
                            continue;
                        }

                        if (FrequentItemSets.ContainsKey(leftSide))
                        {
                            var confidence = (double)FrequentItemSets[frequentItemSet] / FrequentItemSets[leftSide];
                            if (confidence >= minConfidence)
                            {
                                var rule = new DecisionRule<T>(leftSide.ItemSet, rightSide.ItemSet, FrequentItemSets[frequentItemSet], confidence);
                                decisionRules.Add(rule);
                            }
                        }
                    }
                }
            }
            return decisionRules;
        }

    }
}
