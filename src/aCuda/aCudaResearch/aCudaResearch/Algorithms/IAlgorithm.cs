using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CudaRuler.Algorithms
{
    /// <summary>
    /// Representation of the algorithm implementations
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// Main algorithm method to execute the computation.
        /// 
        /// This method should also contain the measurement process!
        /// </summary>
        void Run(ExecutionSettings executionSettings, bool printResults);
    }
}
