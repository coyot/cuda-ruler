﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CudaRuler.Algorithms;

namespace CudaRuler
{
    public static class AlgorithmBuilder
    {
        public static DataSourceType DataSourceType { get; set; }

        public static IAlgorithm BuildAlgorithm(AlgorithmType algorithmType)
        {
            // Algorithms for MsData
            if (DataSourceType == DataSourceType.MsData)
            {
                switch (algorithmType)
                {
                    case AlgorithmType.FpGrowth:
                        return new MsWebFpGrowthAlgorithm();
                    case AlgorithmType.Apriori:
                        return new MsWebAprioriAlgorithm();
                    case AlgorithmType.ParallelApriori:
                        return new MsWebParallelAprioriAlgorithm();
                    case AlgorithmType.CudaApriori:
                        return new MsWebCudaAprioriAlgorithm();
                }
            }

            throw new Exception("Wrong setting!");
        }
    }
}
