using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class TreeNode<T>
    {
        public T Value;
        internal TreeNode<T> root;
        internal TreeNode<T> prev;
        internal TreeNode<T> child_tail; // same root

        public TreeNode(T value) : this(null, value)
        {
        }

        internal TreeNode(TreeNode<T> root, T value)
        {
            this.root = root;
            Value = value;
        }

        public TreeNode<T> RootNode => root;

        public bool IsRootNode => root == null;

        public bool IsLeafNode => child_tail == null;

        public void Expand(T value)
        {
            TreeNode<T> child = new TreeNode<T>(this, value);

            child.prev = child_tail;
            child_tail = child;
        }

        public bool Shrink(T value) =>
            Shrink(value, EqualityComparer<T>.Default);

        public bool Shrink(T value, IEqualityComparer<T> comparer)
        {
            TreeNode<T> _next = null;
            TreeNode<T> _curr = child_tail;
            
            while (_curr != null)
            {
                if (comparer.Equals(value, _curr.Value))
                {
                    _curr.root = null;
                    if (_next != null)
                        _next.prev = _curr.prev;
                    else
                        child_tail = _curr.prev;

                    return true;
                }

                _next = _curr; 
                _curr = _curr.prev;
            }

            return false;
        }

        public TreeNode<T> FindChild(T value) =>
            FindChild(value, EqualityComparer<T>.Default);

        public TreeNode<T> FindChild(T value, IEqualityComparer<T> comparer)
        {
            TreeNode<T> curr = child_tail;
            for (; curr != null; curr = curr.prev)
                if (comparer.Equals(curr.Value, value))
                    return curr;

            return null;
        }
    }
}
