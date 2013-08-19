using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace aCudaResearch
{
    [Serializable]
    public class ExecutionSettings
    {
        private double _minSup;
        private double _minConf;
        private int _startNumber;
        private int _endNumber;

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
        /// Initial number of transactions which will be taken into execution.
        /// </summary>
        public int StartNumber
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _startNumber;
            }

            set
            {
                Contract.Requires(value >= 0);
                _startNumber = value;
            }
        }

        /// <summary>
        /// Max number of transactions which will be taken into execution.
        /// </summary>
        public int EndNumber
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _endNumber;
            }

            set
            {
                Contract.Requires(value >= 0);
                _endNumber = value;
            }
        }

        /// <summary>
        /// Path to the file with transactions.
        /// </summary>
        public string DataSourcePath { get; set; }

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
            builder.Append("\n\tStart Number: ").Append(StartNumber);
            builder.Append("\n\tEnd Number: ").Append(EndNumber);
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
