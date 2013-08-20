using System.Collections.Generic;
using System.Linq;

namespace CudaRuler.Algorithms.FpGrowth
{
    public class TreeNode<T>
    {
        private readonly Dictionary<T, TreeNode<T>> _children;
        //private TreeNode<T> parentNode;
        private readonly FpTree<T> _parentTree;

        /// <summary>
        /// Current node's parent.
        /// </summary>
        public TreeNode<T> ParentNode { get; private set; }

        /// <summary>
        /// Value represented by the node.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Transaction counter for the node.
        /// </summary>
        public int TransactionCounter { get; private set; }

        /// <summary>
        /// Ctor for the class.
        /// </summary>
        /// <param name="value">Value represented by the node.</param>
        /// <param name="parentNode">Parent of the node in the tree</param>
        /// <param name="parentTree">Tree where the node belongs to.</param>
        public TreeNode(T value, TreeNode<T> parentNode, FpTree<T> parentTree)
        {
            Value = value;
            ParentNode = parentNode;
            _parentTree = parentTree;
            TransactionCounter = 1;
            _children = new Dictionary<T, TreeNode<T>>();
        }

        /// <summary>
        /// Ctor for the class.
        /// </summary>
        /// <param name="value">Value represented by the node.</param>
        /// <param name="parentNode">Parent of the node in the tree</param>
        /// <param name="parentTree">Tree where the node belongs to.</param>
        /// <param name="support">Support for the node</param>
        public TreeNode(T value, TreeNode<T> parentNode, FpTree<T> parentTree, int support)
        {
            Value = value;
            ParentNode = parentNode;
            _parentTree = parentTree;
            TransactionCounter = support;
            _children = new Dictionary<T, TreeNode<T>>();
        }

        /// <summary>
        /// Add new elements to the tree, where the node belongs.
        /// </summary>
        /// <param name="set">Elements which should be added</param>
        /// <param name="index">Index of the actual adding element in the input set.</param>
        public void AddSubset(T[] set, int index)
        {
            if (!_children.ContainsKey(set[index]))
            {
                var node = new TreeNode<T>(set[index], this, _parentTree);
                _children[set[index]] = node;
                _parentTree.NodeAdded(node);
            }
            else
            {
                _children[set[index]].IncreaseCounter();
            }
            if (set.Length > index + 1)
            {
                _children[set[index]].AddSubset(set, index + 1);
            }
        }

        /// <summary>
        /// Add new elements to the tree, where the node belongs, with the specified support.
        /// </summary>
        /// <param name="set">Elements which should be added</param>
        /// <param name="index">Index of the actual adding element in the input set.</param>
        /// <param name="support">Support for the new node.</param>
        public void AddSet(T[] set, int index, int support)
        {
            if (!_children.ContainsKey(set[index]))
            {
                var node = new TreeNode<T>(set[index], this, _parentTree, support);
                _children[set[index]] = node;
                _parentTree.NodeAdded(node);
            }
            else
            {
                _children[set[index]].IncreaseCounter(support);
            }
            if (set.Length > index + 1)
            {
                _children[set[index]].AddSet(set, index + 1, support);
            }
        }

        /// <summary>
        /// Check if the node is the leaf.
        /// </summary>
        /// <returns>True if the node is leaf, fale otherwise.</returns>
        public bool IsLastNode()
        {
            return _children.Count <= 0;
        }

        /// <summary>
        /// Increase the number of transaction by 1.
        /// </summary>
        public void IncreaseCounter()
        {
            IncreaseCounter(1);
        }

        /// <summary>
        /// Increase the transaction counter by the specified value.
        /// </summary>
        /// <param name="support">Value by which the counter should be incremented.</param>
        public void IncreaseCounter(int support)
        {
            TransactionCounter += support;
        }



        /// <summary>
        /// Zwraca informację, czy wierzchołek znajduje się w ścieżce pojedynczej w drzewie.
        /// </summary>
        /// <returns>true, jeżeli wierzchołek należy do pojedynczej ścieżki, w przeciwnym razie false</returns>
        public bool IsInSinglePath()
        {
            if (_children.Count == 0)
            {
                return true;
            }
            if (_children.Count > 1)
            {
                return false;
            }
            return _children.Values.First().IsInSinglePath();
        }

        /// <summary>
        /// Get the first node from the successors list.
        /// </summary>
        /// <returns>
        /// The first node from the successors list or null if the current node is leaf.
        /// </returns>
        public TreeNode<T> GetFirstChild()
        {
            return _children.Count == 0 ? null : _children.Values.First();
        }
    }
}
