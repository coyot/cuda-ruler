using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aCudaResearch.Data.MsWeb;

namespace aCudaResearch.Data
{
    /// <summary>
    /// Representation of the decision rule in the system.
    /// </summary>
    /// <typeparam name="T">Type of data in the rule.</typeparam>
    public class DecisionRule<T>
    {
        /// <summary>
        /// Objects in the premision of the rule ({X] => {..}).
        /// </summary>
        public HashSet<T> Premise { get; private set; }

        /// <summary>
        /// Objects in the decision of the rule ({..} => {X}).
        /// </summary>
        public HashSet<T> Decision { get; private set; }

        /// <summary>
        /// Support of the rule.
        /// </summary>
        public int Support { get; private set; }

        /// <summary>
        /// Confidence of the rule.
        /// </summary>
        public double Confidence { get; private set; }

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="premise">Premision set.</param>
        /// <param name="decision">Decision set.</param>
        /// <param name="support">Support of the rule.</param>
        /// <param name="confidence">Confidence of the rule.</param>
        public DecisionRule(HashSet<T> premise, HashSet<T> decision, int support, double confidence)
        {
            Premise = premise;
            Decision = decision;
            Support = support;
            Confidence = confidence;
        }

        /// <summary>
        /// Prepare rule as the string object. It uses information from the MsWeb data input.
        /// </summary>
        /// <param name="dataBaseSize">Number of elements in the input data.</param>
        /// <param name="attributes">Set of attributes of the data (<see cref="aCudaResearch.Data.MsWeb.MsInstance{T}"/>)</param>
        /// <returns>String which represents the rule.</returns>
        public string ToString(int dataBaseSize, Dictionary<T, MsElement> attributes)
        {
            var sb = new StringBuilder("( ");
            foreach (var element in Premise)
            {
                sb.Append(attributes[element].Title + ", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" ) => ( ");
            foreach (var item in Decision)
            {
                sb.Append(attributes[item].Title + " )");
            }
            sb.Append("\nSupport: " + (double)Support / dataBaseSize);
            sb.Append("\nConfidence: " + Confidence);

            return sb.ToString();
        }

        /// <summary>
        /// Prepare rule as the string object. It uses information from the kosarak data input.
        /// </summary>
        /// <param name="dataBaseSize">Number of elements in the input data.</param>
        /// <returns>String which represents the rule.</returns>
        public string ToString(int dataBaseSize)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in Premise)
            {
                sb.Append(element + ", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" => ");
            foreach (var item in Decision)
            {
                sb.Append(item);
            }
            sb.Append("\nSupport = " + (double)Support / dataBaseSize);
            sb.Append("\nConfidence = " + Confidence);

            return sb.ToString();
        }
    }
}
