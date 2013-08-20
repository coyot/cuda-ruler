using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using aCudaResearch.Cuda;
using aCudaResearch.Data;
using aCudaResearch.Data.MsWeb;
using aCudaResearch.Helpers;
using System.IO;
using System.Linq;
using System.Reflection;
using GASS.CUDA;
using GASS.CUDA.Types;

namespace aCudaResearch.Algorithms
{
    public class MsWebCudaAprioriAlgorithm : MsWebAbstractAlgorithm
    {
        private static Color one = Color.FromArgb(1);

        public override void Run(ExecutionSettings executionSettings, bool printRules)
        {
            builder = new MsDataBuilder();
            var data = builder.BuildInstance(executionSettings);
            var elementsList = data.Elements.Keys.ToList();
            var transactionsList = data.Transactions.Keys.ToList();
            var bitmapWrapper = PrepareBitmapWrapper(data, elementsList, transactionsList);
            var elementsFrequencies = CalculateElementsFrequencies(bitmapWrapper);
            var frequentSets = elementsList
                .Where(e => elementsFrequencies[elementsList.IndexOf(e)] >= executionSettings.MinSup * transactionsList.Count)
                .Select(element => new List<int> { element })
                .ToList();
            var frequentItemSets = frequentSets.ToDictionary(set => new FrequentItemSet<int>(set),
                                                             set => elementsFrequencies[elementsList.IndexOf(set[0])]);
            List<List<int>> candidates;

            if (frequentSets.Count == 0) return;

            var bitmapTransposed = new Bitmap(transactionsList.Count, frequentSets.Count);
            var newElementsList = new List<int>(frequentSets.Count);
            var jj = 0;
            foreach (var set in frequentSets)
            {
                newElementsList.Add(set[0]);

                for (var i = 0; i < transactionsList.Count; i++)
                {
                    var pixel = bitmapWrapper.Bitmap.GetPixel(elementsList.IndexOf(set[0]), i);

                    bitmapTransposed.SetPixel(i, jj, pixel);
                }
                jj++;
            }
            var newBitmapWrapper = BitmapWrapper.ConvertBitmap(bitmapTransposed);

            while ((candidates = GenerateCandidates(frequentSets)).Count > 0)
            {
                // 1. tranlate into elements Id's
                foreach (var candidate in candidates)
                {
                    for (var i = 0; i < candidate.Count; i++)
                    {
                        candidate[i] = newElementsList.IndexOf(candidate[i]);
                    }
                }

                // 2. execute CUDA counting
                candidates = GetFrequentSets(candidates, executionSettings.MinSup, newBitmapWrapper, transactionsList.Count);

                // 3. translate back from elements Id's
                foreach (var candidate in candidates)
                {
                    for (var i = 0; i < candidate.Count; i++)
                    {
                        candidate[i] = newElementsList[candidate[i]];
                    }
                }

                if (candidates.Count > 0)
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    frequentSets = candidates;
                    foreach (var candidate in candidates)
                    {
                        frequentItemSets.Add(new FrequentItemSet<int>(candidate), candidate.GetSupport(data.Transactions));
                    }
                    sw.Stop();
                    //Console.WriteLine("CAND: {0}", sw.ElapsedMilliseconds);
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

            var result = PrintRules(decisionRules, executionSettings.DataSourcePath, executionSettings.MinSup, executionSettings.MinConf, data.Transactions.Keys.Count, data.Elements);
            Console.WriteLine(result);
        }

        private List<List<int>> GetFrequentSets(IList<List<int>> candidates, double minSup, BitmapWrapper bitmapWrapper, int numberOfTransactions)
        {
            var borderValue = minSup * numberOfTransactions;
            var output = CalculateCandidatesFrequencies<int>(candidates, bitmapWrapper, Math.Ceiling(borderValue));
            var result = new List<List<int>>();

            int i = 0;
            foreach (var candidate in candidates)
            {
                if (output[i] >= borderValue)
                {
                    result.Add(candidate);
                }
                i++;
            }

            return result;
        }


        private static int[] CalculateCandidatesFrequencies<T>(IList<List<int>> candidates, BitmapWrapper bitmapWrapper, double borderValue)
        {
            using (var cuda = new CUDA(0, true))
            {
                var elementsFrequencies = new int[candidates.Count];

                var inputSets = new int[candidates.Count * candidates[0].Count];
                var ind = 0;
                foreach (var candidate in candidates)
                {
                    foreach (var i in candidate)
                    {
                        inputSets[ind] = i;
                        ind++;
                    }
                }
                
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        Names.CudaAprioriCountModule);
                cuda.LoadModule(path);
                var inputData = cuda.CopyHostToDevice(bitmapWrapper.RgbValues);
                var inputSetData = cuda.CopyHostToDevice(inputSets);
                var frequenciesOnHost = cuda.Allocate(new int[candidates.Count]);
                var sw = new Stopwatch();
                sw.Start();
                CallTheSetsFrequenciesCount(cuda, inputData, inputSetData, frequenciesOnHost, bitmapWrapper, candidates[0].Count, candidates.Count, borderValue);
                sw.Stop();

                //Console.WriteLine("CalculateCandidatesFrequencies: {0} ms", sw.ElapsedMilliseconds);
                cuda.CopyDeviceToHost(frequenciesOnHost, elementsFrequencies);

                cuda.Free(inputData);
                cuda.Free(inputSetData);
                cuda.Free(frequenciesOnHost);
                cuda.UnloadModule();

                return elementsFrequencies;
            }
        }

        private static void CallTheSetsFrequenciesCount(CUDA cuda, CUdeviceptr deviceInput, CUdeviceptr deviceInputSet, CUdeviceptr deviceOutput, BitmapWrapper wrapper, int setSize, int sets, double borderValue)
        {
            new CudaFunctionCall(cuda, Names.CountSetsFrequencies)
                .AddParameter(deviceInput)
                .AddParameter(deviceInputSet)
                .AddParameter(deviceOutput)
                .AddParameter((uint)wrapper.Width)
                .AddParameter((uint)wrapper.Height)
                .AddParameter((uint)setSize)
                .AddParameter((uint)sets)
                .AddParameter((uint)borderValue)
                .Execute(ProjectConstants.BlockSize, ProjectConstants.BlockSize, 1, ProjectConstants.GridSize, ProjectConstants.GridSize);
        }

        private BitmapWrapper PrepareBitmapWrapper(MsInstance<int> data, List<int> elementsList, List<int> transactionsList)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bitmap = BuildTransactionsBitmap(data, transactionsList, elementsList);
            stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            ExecutionEngine.HackTimes.Add(stopwatch.ElapsedMilliseconds);
            var bitmapWrapper = BitmapWrapper.ConvertBitmap(bitmap);
            return bitmapWrapper;
        }

        private Bitmap BuildTransactionsBitmap(MsInstance<int> data, IList<int> transactionsList, IList<int> elementsList)
        {
            var bitmap = new Bitmap(elementsList.Count, transactionsList.Count);
            for (var i = 0; i < elementsList.Count; i++)
            {
                for (var j = 0; j < transactionsList.Count; j++)
                {
                    if (data.Transactions[transactionsList[j]].Contains(elementsList[i]))
                    {
                        bitmap.SetPixel(i, j, one);
                    }
                }
            }
            return bitmap;
        }

        private int[] CalculateElementsFrequencies(BitmapWrapper bitmapWrapper)
        {
            var elementsFrequencies = new int[bitmapWrapper.Width];
            using (var cuda = new CUDA(0, true))
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        Names.CudaAprioriCountModule);

                cuda.LoadModule(path);

                var inputData = cuda.CopyHostToDevice(bitmapWrapper.RgbValues);
                var answer = cuda.Allocate(new int[bitmapWrapper.Width]);
                CallTheFrequencyCount(cuda, inputData, answer, bitmapWrapper);
                cuda.CopyDeviceToHost(answer, elementsFrequencies);

                cuda.Free(inputData);
                cuda.Free(answer);
                cuda.UnloadModule();
            }
            return elementsFrequencies;
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

        protected virtual void CallTheFrequencyCount(CUDA cuda, CUdeviceptr deviceInput, CUdeviceptr deviceOutput, BitmapWrapper wrapper)
        {
            new CudaFunctionCall(cuda, Names.CountFrequency)
                .AddParameter(deviceInput)
                .AddParameter(deviceOutput)
                .AddParameter((uint)wrapper.Width)
                .AddParameter((uint)wrapper.Height)
                .Execute(ProjectConstants.BlockSize, ProjectConstants.BlockSize, 1, ProjectConstants.GridSize, ProjectConstants.GridSize);
        }
    }
}
