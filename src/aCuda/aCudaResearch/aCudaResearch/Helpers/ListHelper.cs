using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Helpers
{
    public static class ListHelper
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
        public static bool IsFrequent<T>(this IList<T> list, Dictionary<T, T[]> transactions, double support)
        {
            var localSupport = list.GetSupport(transactions);

            return localSupport >= (support * transactions.Count);
        }

        /// <summary>
        /// Gets the support for the list
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">List to be checked</param>
        /// <param name="transactions">List of transactions</param>
        /// <returns>Support for the set of elements</returns>
        public static int GetSupport<T>(this IList<T> list, Dictionary<T, T[]> transactions)
        {
            var transactionsList = transactions.Values.ToList();

            foreach (var element in list)
            {
                var tmpTransactionsList =
                    transactionsList.Where(transaction => transaction.Contains(element)).ToList();

                transactionsList = tmpTransactionsList;
            }
            return transactionsList.Count;
        }

        /// <summary>
        /// Checks if two lists contains the same elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="first">First list</param>
        /// <param name="second">Second list</param>
        /// <returns>True if two lists are the same, false otherwise</returns>
        public static bool AreEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstCount = first.Count();
            var secondCount = second.Count();

            if (firstCount == 0 && secondCount == 0)
            {
                return true;
            }

            return firstCount == secondCount && first.Where(element => second.Contains(element)).Any();
        }

        /// <summary>
        /// Returns the complement of the list 
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="baseList">Base list</param>
        /// <param name="firstPart">The rest of the list</param>
        /// <returns>The complement</returns>
        public static IList<T> GetComplement<T>(this IList<T> baseList, IList<T> firstPart)
        {
            return baseList.Where(element => !firstPart.Contains(element)).ToList();
        }

        /// <summary>
        /// Creates new list which is the merge of two inputs
        /// </summary>
        /// <typeparam name="T">Type of data stored in the list</typeparam>
        /// <param name="first">First list</param>
        /// <param name="second">Second list</param>
        /// <returns>A list which contains elements both from first, and the second input list</returns>
        public static IList<T> Merge<T>(this IList<T> first, IList<T> second)
        {
            var result = first.ToList();

            result.Add(second[second.Count - 1]);

            return result;
        }
    }
}
