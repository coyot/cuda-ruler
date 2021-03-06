﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CudaRuler.Data.MsWeb;

namespace CudaRuler.Algorithms
{
    /// <summary>
    /// Representation of the algorithm which uses Kosarak Data as the input.
    /// </summary>
    public abstract class KosarakAbstractAlgorithm : IAlgorithm
    {
        private MsDataBuilder builder;

        public abstract void Run(ExecutionSettings executionSettings, bool printRules);
    }
}
