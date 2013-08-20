using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Helpers
{
    public static class ParallelListHelper
    {
        /// <summary>
        /// Checks if the list of elements is frequent for supported
        /// transactions and specified support.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">List of values which should be checked</param>
        /// <param name="transactions">List of transactions</param>
        /// <param name="support">The minimal support for the frequency</param>
        /// <returns>True if the set is frequent, false otherwise</returns>
        public static bool IsFrequentParallel<T>(this IList<T> list, Dictionary<T, T[]> transactions, double support)
        {
            return list.GetSupportParallel(transactions) >= (support * transactions.Count);
        }

        /// <summary>
        /// Gets the support for the list
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">List to be checked</param>
        /// <param name="transactions">List of transactions</param>
        /// <returns>Support for the set of elements</returns>
        public static int GetSupportParallel<T>(this IList<T> list, Dictionary<T, T[]> transactions)
        {
            var transactionsList = transactions.Values.ToList();

            foreach (var element in list)
            {
                var tmpTransactionsList =
                    transactionsList.Where(transaction => transaction.Contains(element)).AsParallel().ToList();

                transactionsList = tmpTransactionsList;
            }
            return transactionsList.Count;
        }

        /// <summary>
        /// Checks if two lists contains the same elements in the parallel way (usage of PLINQ).
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="first">First list</param>
        /// <param name="second">Second list</param>
        /// <returns>True if two lists are the same, false otherwise</returns>
        public static bool AreEqualParallel<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstCount = first.Count();
            var secondCount = second.Count();

            if (firstCount == 0 && secondCount == 0)
            {
                return true;
            }

            return firstCount == secondCount && first.Where(element => second.Contains(element)).AsParallel().Any();
        }
    }
}
