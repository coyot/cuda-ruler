using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CudaRuler.Algorithms;

namespace CudaRuler
{
    /// <summary>
    /// This class will be used to execute research experiments.
    /// 
    /// This will also include time measuring process, loading data, execution of proper algorithms.
    /// </summary>
    public class ExecutionEngine
    {
        private readonly ExecutionSettings _settings;
        private Dictionary<AlgorithmType, IAlgorithm> Algorithms;
        private int NUMBER_OF_RUNS = 1;

        public static List<long> HackTimes { get; set; }

        /// <summary>
        /// Programm execution settings.
        /// </summary>
        public ExecutionSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public ExecutionEngine(ExecutionSettings settings)
        {
            _settings = settings;

            // prepare the pool of algorithms
            Algorithms = new Dictionary<AlgorithmType, IAlgorithm>();
            PrepareAlgorithms();
            HackTimes = new List<long>();
        }

        /// <summary>
        /// Engine main method to execute computation process.
        /// </summary>
        /// <param name="printRules">Specifies if the association rules</param>
        /// <returns>Dictionary of times per each executed algorithm</returns>
        public Dictionary<AlgorithmType, long> ExecuteComputation(bool printRules)
        {
            var result = new Dictionary<AlgorithmType, long>();

            Console.WriteLine("Computation will be executed.");
            Console.WriteLine(_settings.ToString());

            foreach (var algorithm in Settings.Algorithms)
            {
                for (int i = 0; i < NUMBER_OF_RUNS; i++)
                {
                    var stopWatch = new Stopwatch();

                    stopWatch.Start();
                    Algorithms[algorithm].Run(Settings, printRules);
                    stopWatch.Stop();

                    var time = algorithm == AlgorithmType.CudaApriori
                                   ? stopWatch.ElapsedMilliseconds - HackTimes[i]
                                   : stopWatch.ElapsedMilliseconds;

                    if (result.ContainsKey(algorithm))
                    {
                        result[algorithm] += time;
                    }
                    else
                    {
                        result.Add(algorithm, time);
                    }

                }
                result[algorithm] = result[algorithm]/NUMBER_OF_RUNS;
            }

            return result;
        }

        private void PrepareAlgorithms()
        {
            AlgorithmBuilder.DataSourceType = Settings.DataSourceType;

            foreach (var algorithm in Settings.Algorithms.Where(algorithm => !Algorithms.ContainsKey(algorithm)))
            {
                Algorithms.Add(algorithm, AlgorithmBuilder.BuildAlgorithm(algorithm));
            }
        }
    }
}
