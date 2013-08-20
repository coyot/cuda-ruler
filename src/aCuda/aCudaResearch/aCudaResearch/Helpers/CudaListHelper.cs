using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GASS.CUDA;
using GASS.CUDA.Types;
using aCudaResearch.Cuda;

namespace aCudaResearch.Helpers
{
    public static class CudaListHelper
    {

        /// <summary>
        /// Checks if the list of elements is frequent for supported
        /// transactions and specified support.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="elements"></param>
        /// <param name="bitmapWrapper"></param>
        /// <param name="support"></param>
        /// <returns>True if the set is frequent, false otherwise</returns>
        public static bool IsFrequent<T>(this IList<T> list, IList<T> elements, BitmapWrapper bitmapWrapper, int numberOfTransactions, double support)
        {
            var result = false;
            var occurances = GetSupport(list, elements, bitmapWrapper);

            return occurances >= (support * numberOfTransactions);
        }

        public static int GetSupport<T>(this IList<T> list, IList<T> elements, BitmapWrapper bitmapWrapper)

        {
            var occurances = 0;
            var elementsPositions = list.Select(elements.IndexOf).ToList();

            var frequency = CalculateElementsFrequencies(elementsPositions, bitmapWrapper);
            for (var i = 0; i < frequency.Count(); i++)
            {
                occurances += frequency[i];
            }
            return occurances;
        }

        private static int[] CalculateElementsFrequencies<T>(IList<T> elements, BitmapWrapper bitmapWrapper)
        {
            int frequencesSize = ProjectConstants.BlockSize * ProjectConstants.GridSize;
            var elementsFrequencies = new int[frequencesSize];

            using (var cuda = new CUDA(0, true))
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        "apriori_count1.cubin");

                cuda.LoadModule(path);

                var inputData = cuda.CopyHostToDevice(bitmapWrapper.RgbValues);
                var inputSetData = cuda.CopyHostToDevice(elements.ToArray());

                var answer = cuda.Allocate(new int[frequencesSize]);


                CallTheFrequencyCount(cuda, inputData, inputSetData, answer, bitmapWrapper, elements.Count);
                cuda.CopyDeviceToHost(answer, elementsFrequencies);

                cuda.Free(inputData);
                cuda.Free(answer);
                cuda.UnloadModule();
            }
            return elementsFrequencies;
        }

        private static void CallTheFrequencyCount(CUDA cuda, CUdeviceptr deviceInput, CUdeviceptr deviceInputSet, CUdeviceptr deviceOutput, BitmapWrapper wrapper, int setSize)
        {
            new CudaFunctionCall(cuda, Names.CountSetsFrequencies)
                .AddParameter(deviceInput)
                .AddParameter(deviceInputSet)
                .AddParameter(deviceOutput)
                .AddParameter((uint)wrapper.Width)
                .AddParameter((uint)wrapper.Height)
                .AddParameter((uint)setSize)
                .Execute(ProjectConstants.BlockSize, ProjectConstants.BlockSize, 1, ProjectConstants.GridSize, ProjectConstants.GridSize);
        }
    }
}
