using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace CudaRuler
{
    [Serializable]
    public class ExecutionSettings
    {
        private double _minSup;
        private double _minConf;
        private int _transactionsNumber;

        public ExecutionSettings()
        {
            Algorithms = new List<AlgorithmType>();
        }

        public double MinSup
        {
            get
            {
                Contract.Ensures(Contract.Result<double>() >= 0);

                return _minSup;
            }

            set
            {
                Contract.Requires(value >= 0);
                _minSup = value;
            }
        }

        public double MinConf
        {
            get
            {
                Contract.Ensures(Contract.Result<double>() >= 0);

                return _minConf;
            }

            set
            {
                Contract.Requires(value >= 0);
                _minConf = value;
            }
        }

        /// <summary>
        /// Number of transactions which will be taken into execution.
        /// </summary>
        public int TransactionsNumber
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _transactionsNumber;
            }

            set
            {
                Contract.Requires(value >= 0);
                _transactionsNumber = value;
            }
        }

        /// <summary>
        /// Path to the file with transactions.
        /// </summary>
        public string DataSourcePath { get; set; }

        /// <summary>
        /// Pathe to the file where the output values should be placed
        /// </summary>
        public string OutputFile { get; set; }

        /// <summary>
        /// The type of the source which will be used for computation.
        /// </summary>
        public DataSourceType DataSourceType { get; set; }

        /// <summary>
        /// List of algorithms which will be used for computation.
        /// </summary>
        [XmlArray(ElementName="Algorithms")]
        [XmlArrayItem(ElementName="Algorithm")]
        public List<AlgorithmType> Algorithms { get; set; }

        /// <summary>
        /// Override base ToString() method with one which is more useful.
        /// </summary>
        /// <returns>Settings description</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Data Source: ").Append(DataSourcePath);
            builder.Append("\nMinSup: ").Append(MinSup);
            builder.Append("\nMinConf: ").Append(MinConf);
            builder.Append("\nOutputFile: ").Append(OutputFile);
            builder.Append("\n\tTransactions Number: ").Append(TransactionsNumber);
            builder.Append("\n\tData Source Type: ").Append(DataSourceType);
            builder.Append("\nAlgorithms: ");

            foreach (var algorithm in Algorithms)
            {
                builder.Append("\n\t - ").Append(algorithm);
            }

            return builder.ToString();
        }
    }
}
