using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CudaRuler.Algorithms.FpGrowth
{
    public class FpTree<T>
    {
        readonly Dictionary<T, TreeNode<T>> _children = new Dictionary<T, TreeNode<T>>();
        readonly Dictionary<T, List<TreeNode<T>>> _headerTable = new Dictionary<T, List<TreeNode<T>>>();

        /// <summary>
        /// Header dictionary of the FP-tree.
        /// </summary>
        public Dictionary<T, List<TreeNode<T>>> HeaderTable
        {
            get { return _headerTable; }
        }

        /// <summary>
        /// Build the FP-Tree based on the input data.
        /// </summary>
        /// <param name="data">
        /// Dictionary with the data to build the FP-tree. Kes is the ID, and the Value is the table
        /// of the elements.
        /// </param>
        public void BuildFpTreeFromData(Dictionary<int, T[]> data)
        {
            foreach (var set in data.Values)
            {
                AddSet(set);
            }
        }

        /// <summary>
        /// Creates the conditional tree based on the input data.
        /// </summary>
        /// <param name="data">
        /// Dictionary of data to build the conditional tree. Key is the table of the elements, and the Value
        /// is the support.
        /// </param>
        public void BuildConditionalTree(Dictionary<T[], int> data)
        {
            foreach (var key in data.Keys)
            {
                AddSet(key, data[key]);
            }
        }

        /// <summary>
        /// An event after adding the element during the FP-tree building process.
        /// </summary>
        /// <param name="newNode">New node in the tree.</param>
        public void NodeAdded(TreeNode<T> newNode)
        {
            if (!HeaderTable.ContainsKey(newNode.Value))
            {
                HeaderTable[newNode.Value] = new List<TreeNode<T>>();
            }
            HeaderTable[newNode.Value].Add(newNode);
        }

        /// <summary>
        /// Returns the conditioned pattern base in the tree for specified element.
        /// </summary>
        /// <param name="element">Specified element.</param>
        /// <returns>
        /// Dicitonary of values. Key is the list of elements, and the Value is the support.
        /// </returns>
        public Dictionary<List<TreeNode<T>>, int> GetCondPatternBase(T element)
        {
            var patternBase = new Dictionary<List<TreeNode<T>>, int>();
            foreach (var node in _headerTable[element])
            {
                var prefixPath = GetPrefixPath(node);
                if (prefixPath.Count > 0)
                    patternBase.Add(prefixPath, node.TransactionCounter);
            }
            return patternBase;
        }

        /// <summary>
        /// Returns the prefix path in the tree for specified element.
        /// </summary>
        /// <param name="node">Node in the tree for which the prefix path is requested.</param>
        /// <returns>Prefix path as te list of nodes.</returns>
        public List<TreeNode<T>> GetPrefixPath(TreeNode<T> node)
        {
            var path = new List<TreeNode<T>>();
            node = node.ParentNode;
            while (node != null)
            {
                var newNode = new TreeNode<T>(node.Value, node.ParentNode, null);
                path.Insert(0, newNode);
                node = node.ParentNode;
            }
            return path;
        }

        /// <summary>
        /// Checks if the tree has a single path.
        /// </summary>
        /// <returns>true, if there is only one single path, false otherwise</returns>
        public bool HasSinglePath()
        {
            if (_children.Count > 1)
            {
                return false;
            }
            if (_children.Values.Count == 0)
            {
                return false;
            }

            return _children.Values.First().IsInSinglePath();
        }

        /// <summary>
        /// Return the single path.
        /// </summary>
        /// <returns>Path represented as the list of nodes.</returns>
        public List<TreeNode<T>> GetSinglePath()
        {
            if (HasSinglePath())
            {
                var list = new List<TreeNode<T>>();
                var node = _children.Values.First();
                while (node != null)
                {
                    list.Add(node);
                    node = node.GetFirstChild();
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// Return the list of elements with the number of its occurances in the tree.
        /// </summary>
        /// <returns>
        /// Sorted list where the key i the number of occurances, and the value is the list of elements with
        /// such number of occurances.
        /// </returns>
        public SortedList<int, List<T>> GetFrequencyList()
        {
            var sortedFrequencyList = new SortedList<int, List<T>>();
            foreach (var nodes in _headerTable.Values)
            {
                var count = nodes.Sum(node => node.TransactionCounter);

                if (!sortedFrequencyList.ContainsKey(count))
                {
                    sortedFrequencyList[count] = new List<T>();
                }
                sortedFrequencyList[count].Add(nodes[0].Value);
            }

            return sortedFrequencyList;
        }

        /// <summary>
        /// Get the support in the tree for input element.
        /// </summary>
        /// <param name="element">Element of the tree.</param>
        /// <returns>
        /// Support as the sum of the number of occurances of the element in the tree.
        /// </returns>
        public int GetSupport(T element)
        {
            return _headerTable.ContainsKey(element) ? _headerTable[element].Sum(node => node.TransactionCounter) : 0;
        }

        /// <summary>
        /// Add set of elements into the tree.
        /// </summary>
        /// <param name="set">Set which should be added into the tree.</param>
        private void AddSet(T[] set)
        {
            if (!_children.ContainsKey(set[0]))
            {
                var node = new TreeNode<T>(set[0], null, this);
                _children[set[0]] = node;
                NodeAdded(node);
            }
            else
            {
                _children[set[0]].IncreaseCounter();
            }

            if (set.Length > 1)
            {
                _children[set[0]].AddSubset(set, 1);
            }
        }

        /// <summary>
        /// Adds the set into the tree together with its support.
        /// </summary>
        /// <param name="set">Set of elements to be added into the tree.</param>
        /// <param name="support">Starting support for the elements.</param>
        private void AddSet(T[] set, int support)
        {
            if (!_children.ContainsKey(set[0]))
            {
                var node = new TreeNode<T>(set[0], null, this, support);
                _children[set[0]] = node;
                NodeAdded(node);
            }
            else
            {
                _children[set[0]].IncreaseCounter(support);
            }
            if (set.Length > 1)
            {
                _children[set[0]].AddSet(set, 1, support);
            }
        }


    }
}
