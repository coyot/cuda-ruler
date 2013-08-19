using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using aCudaResearch.Algorithms;

namespace aCudaResearch
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
        }

        /// <summary>
        /// Engine main method to execute computation process.
        /// </summary>
        public Dictionary<AlgorithmType, long> ExecuteComputation()
        {
            //! here the computation time measuring should be placed!!!
            var result = new Dictionary<AlgorithmType, long>();

            Console.WriteLine("Computation will be executed.");
            Console.WriteLine(_settings.ToString());

            foreach (var algorithm in Settings.Algorithms)
            {
                var stopWatch = new Stopwatch();
                
                stopWatch.Start();
                Algorithms[algorithm].Run(Settings);
                stopWatch.Stop();

                result.Add(algorithm, stopWatch.ElapsedMilliseconds);
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
