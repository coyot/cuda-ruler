using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aCudaResearch.Data
{
    /// <summary>
    /// Frequent Set representation
    /// </summary>
    /// <typeparam name="T">Type of data stored in the set</typeparam>
    public class FrequentItemSet<T>
    {
        /// <summary>
        /// Elementy zbioru częstego.
        /// </summary>
        public HashSet<T> ItemSet { get; private set; }

        /// <summary>
        /// Konstruktor klasy.
        /// </summary>
        /// <param name="itemSet">lista elementów składających sie na zbiór częsty</param>
        public FrequentItemSet(IEnumerable<T> itemSet)
        {
            ItemSet = new HashSet<T>();
            foreach (var element in itemSet)
            {
                this.ItemSet.Add(element);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals((FrequentItemSet<T>)obj);
        }

        /// <summary>
        /// Checks if the set is equal to the another set
        /// </summary>
        /// <param name="other">The other set to be compared to</param>
        /// <returns>true, if two sets contains the same elements, false otherwise</returns>
        public bool Equals(FrequentItemSet<T> other)
        {
            return other.ItemSet.Count == ItemSet.Count && ItemSet.All(item => other.ItemSet.Contains(item));
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            if (ItemSet != null)
            {
                foreach (var item in ItemSet)
                {
                    hashCode += item.GetHashCode();
                }
            }
            return hashCode * 397;
        }


        /// <summary>
        /// Checks if two sets are disjunctive.
        /// </summary>
        /// <param name="rightSide">First set</param>
        /// <param name="leftSide">Second set</param>
        /// <returns>True if two sets are disjunctive, false otherwise.</returns>
        public static bool SetsSeparated(FrequentItemSet<T> rightSide, FrequentItemSet<T> leftSide)
        {
            return leftSide.ItemSet.All(item => !rightSide.ItemSet.Contains(item));
        }
    }
}
