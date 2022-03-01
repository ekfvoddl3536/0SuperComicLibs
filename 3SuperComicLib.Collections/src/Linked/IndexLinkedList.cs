using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    [DebuggerTypeProxy(typeof(IIterableView<>))]
    [DebuggerDisplay("Count = {m_size}")]
    public class IndexLinkedList<T> : ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, IValueIterable<T>, ILinkedListSlim_Internal<T>
    {
        private const int NULL_PTR = -1;

        private Node[] m_list;
        private int m_head_idx;
        private int m_free_idx; // default -1
        private int m_size;
        private uint m_version;

        #region constructors
        public IndexLinkedList(int capacity)
        {
            m_list = new Node[Arrays.AutoClampSize(capacity)];
            m_free_idx = -1;
        }

        public IndexLinkedList(IEnumerable<T> collection) : this(4)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var e1 = collection.GetEnumerator();
            while (e1.MoveNext())
                AddLast(e1.Current);
        }

        public IndexLinkedList() : this(4)
        {
        }
        #endregion

        #region properties
        public int Capacity => m_list.Length;

        public int Count => m_size;

        public NodeIndex<T> First => new NodeIndex<T>(this, m_head_idx);

        public NodeIndex<T> Last => new NodeIndex<T>(this, LastIndex);

        private int LastIndex =>
            m_size != 0
            ? m_list[m_head_idx].prev
            : NULL_PTR;

        public NodeIndex<T> this[int rawIndex]
        {
            get
            {
                ref Node n = ref m_list[rawIndex];
                if (n.next < 0 || // deleted slot (NULL_PTR)
                    (n.next | n.prev) == 0 && (rawIndex != 0 || m_size != 1)) // invalid reference
                    throw new NullReferenceException(nameof(rawIndex));

                return new NodeIndex<T>(this, rawIndex);
            }
        }
        #endregion

        #region internal interface impl
        ref T IByReferenceIndexer_Internal<T>.ByRefValue(int index) => ref m_list[index].value;

        int ILinkedListSlim_Internal<T>.GetNextNode(int node) 
        {
            int next = m_list[node].next;
            return 
                next < 0 // invalid reference
                ? node 
                : next;
        }

        int ILinkedListSlim_Internal<T>.GetPrevNode(int node)
        {
            ref Node n = ref m_list[node];
            return
                n.next < 0 // invalid reference
                ? node
                : n.prev;
        }
        #endregion

        #region add item method
        private int P_InsertBefore(int __node_idx, ref T __value)
        {
            int tidx_;
            if (m_free_idx >= 0)
            {
                tidx_ = m_free_idx;
                m_free_idx = m_list[tidx_].prev;
            }
            else if ((tidx_ = m_size) >= m_list.Length) // no space
                IncreaseCapacity();

            Node[] list_ = m_list;

            ref Node newNode_ = ref list_[tidx_];
            newNode_.value = __value;

            ref Node baseNode_ = ref list_[__node_idx];

            newNode_.next = __node_idx;
            newNode_.prev = baseNode_.prev;

            list_[baseNode_.prev].next = tidx_;
            baseNode_.prev = tidx_;

            m_size++; // added
            m_version++;

            return tidx_;
        }

        public NodeIndex<T> AddLast(T value) => new NodeIndex<T>(this, P_InsertBefore(m_head_idx, ref value));

        public NodeIndex<T> AddFirst(T value) => new NodeIndex<T>(this, m_head_idx = P_InsertBefore(m_head_idx, ref value));

        /// <exception cref="ArgumentException">not match owner of node</exception>
        /// <exception cref="InvalidOperationException">referenced empty slot</exception>
        public NodeIndex<T> AddAfter(NodeIndex<T> node, T value)
        {
            ValidateNodeIndex(ref node);
            return new NodeIndex<T>(this, P_InsertBefore(m_list[node.m_index].next, ref value));
        }

        /// <exception cref="ArgumentException">not match owner of node</exception>
        /// <exception cref="InvalidOperationException">referenced empty slot</exception>
        public NodeIndex<T> AddBefore(NodeIndex<T> node, T value)
        {
            ValidateNodeIndex(ref node);
            int result = P_InsertBefore(node.m_index, ref value);
            
            if (node.m_index == m_head_idx) // head 위치 이전에 삽입 = AddFirst와 같음
                m_head_idx = result;

            return new NodeIndex<T>(this, result);
        }
        #endregion

        #region remove item & clear method
        private void Internal_RemoveAt(int node_idx)
        {
            Node[] list_ = m_list;

            ref Node target_ = ref list_[node_idx];

            list_[target_.next].prev = target_.prev;
            list_[target_.prev].next = target_.next;

            if (m_head_idx == node_idx)
                m_head_idx = target_.next;

            m_version++;
            if (--m_size == 0) // empty list
            {
                // reset variables
                m_head_idx = 0;
                m_free_idx = NULL_PTR;
            }
            else
            {
                target_.prev = m_free_idx;
                m_free_idx = node_idx;
            }

            target_.next = NULL_PTR; // flag empty slot
        }

        /// <exception cref="ArgumentException">not match owner of node</exception>
        /// <exception cref="InvalidOperationException">referenced empty slot</exception>
        public void RemoveAt(NodeIndex<T> node)
        {
            ValidateNodeIndex(ref node);
            Internal_RemoveAt(node.m_index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T value) => Remove(value, EqualityComparer<T>.Default);

        public bool Remove(T value, EqualityComparer<T> comparer)
        {
            var node = FindFirst(value, comparer);
            if (node.m_owner != null)
            {
                RemoveAt(node);
                return true;
            }

            return false;
        }

        public bool TryRemove(NodeIndex<T> node)
        {
            if (node.m_owner != this || node.m_index < 0)
                return false;

            Internal_RemoveAt(node.m_index);
            return true;
        }

        public void Clear()
        {
            Node[] list_ = m_list;

            Node defval_ = default;
            // reset values
            for (int i = m_head_idx, sz = m_size; --sz >= 0;)
            {
                ref Node n = ref list_[i];
                i = n.next;
                
                n = defval_; // reset
            }

            // reset variables
            m_size = 0;
            m_head_idx = 0;
            m_free_idx = NULL_PTR;
            m_version++;
        }
        #endregion

        #region find & contains
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NodeIndex<T> FindFirst(T value) => FindFirst(value, EqualityComparer<T>.Default);

        public NodeIndex<T> FindFirst(T value, EqualityComparer<T> comparer)
        {
            Node[] list = m_list;
            for (int i = m_head_idx, sz = m_size; --sz >= 0; i = list[i].next)
                if (comparer.Equals(list[i].value, value))
                    return new NodeIndex<T>(this, i);

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NodeIndex<T> FindLast(T value) => FindLast(value, EqualityComparer<T>.Default);

        public NodeIndex<T> FindLast(T value, EqualityComparer<T> comparer)
        {
            Node[] list = m_list;
            for (int i = m_head_idx, sz = m_size; --sz >= 0;)
            {
                i = list[i].prev;
                if (comparer.Equals(list[i].value, value))
                    return new NodeIndex<T>(this, i);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value) => FindFirst(value).m_owner != null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value, EqualityComparer<T> comparer) => FindFirst(value, comparer).m_owner != null;
        #endregion

        #region util methods & interface impl
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IValueIterator<T> Begin() => new Iterator(this);
        public IValueIterator<T> RBegin() => new ReverseIterator(this);

        public T[] ToArray()
        {
            int sz;
            if ((sz = m_size) == 0)
                return Array.Empty<T>();

            T[] result = new T[sz];

            Node[] list = m_list;
            for (int idx = list[m_head_idx].prev; --sz >= 0;)
            {
                ref Node n = ref list[idx];
                
                result[sz] = n.value;

                idx = n.prev;
            }

            return result;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if ((uint)arrayIndex >= (uint)array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            int sz;
            if (array.Length - arrayIndex < (sz = m_size))
                throw new ArgumentException("insufficient space");

            Node[] list = m_list;
            for (int idx = m_head_idx; --sz >= 0;)
            {
                ref Node n = ref list[idx];

                array[arrayIndex++] = n.value;

                idx = n.next;
            }
        }
        #endregion

        #region collection impl
        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item) => AddLast(item);
        #endregion

        #region helper method
        private void IncreaseCapacity()
        {
            Node[] old = m_list;

            int x;
            Node[] arr = new Node[Arrays.GetNextSize(x = m_size)];

            while (--x >= 0)
                arr[x] = old[x];

            m_list = arr;
        }

        private void ValidateNodeIndex(ref NodeIndex<T> node)
        {
            if (node.m_owner != this)
                throw new ArgumentException($"This '{nameof(IndexLinkedList<T>)}' is not the owner of '{nameof(NodeIndex<T>)}'");

            if ((uint)node.m_index >= (uint)m_list.Length)
                throw new InvalidOperationException("Attempting to access a deleted slot");

            ref Node n = ref m_list[node.m_index];
            if (n.next < 0 ||
                (n.next | n.prev) == 0 && (node.m_index != 0 || m_size != 1))
                throw new InvalidOperationException($"Dereference to {nameof(NULL_PTR)}");
        }
        #endregion

        #region struct & nested class
        private struct Node
        {
            public int next;
            public int prev;
            public T value;
        }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly IndexLinkedList<T> _list;
            private readonly uint _version;
            private int _curnode;
            private T _current;

            public Enumerator(IndexLinkedList<T> inst)
            {
                _list = inst;
                _version = inst.m_version;
                _curnode = inst.m_head_idx;
                _current = default;
            }

            public T Current => _current;
            object IEnumerator.Current => Current;

            private void ValidateVersion()
            {
                if (_list.m_version != _version)
                    throw new InvalidOperationException("modified collection");
            }

            public bool MoveNext()
            {
                ValidateVersion();

                if (_list.m_size == 0 || _curnode < 0) // empty list OR indexOutOfRng(end enumerator)
                    return false;

                ref Node node = ref _list.m_list[_curnode];
                
                _current = node.value;

                if ((_curnode = node.next) == _list.m_head_idx)
                    _curnode = NULL_PTR;

                return true;
            }

            public void Reset()
            {
                ValidateVersion();

                _current = default;
                _curnode = _list.m_head_idx;
            }

            public void Dispose() { }
        }

        private class Iterator : IValueIterator<T>
        {
            protected readonly IndexLinkedList<T> _list;
            protected uint _version;
            protected int _node;
            protected int _index;

            public Iterator(IndexLinkedList<T> inst)
            {
                _list = inst;
                Reset();
            }

            public int Count => _list.m_size;
            public bool IsAlive => _index < _list.m_size;
            public ref T Value => 
                ref (_version != _list.m_version
                ? ref Reference(_index, ref _node).value
                : ref _list.m_list[_node].value);

            public void Add() => _index++;

            public bool LazyAdd() => ++_index < _list.m_size;

            public virtual void Reset()
            {
                _version = _list.m_version;
                _node = _list.m_head_idx;
                _index = 0;
            }

            public T[] ToArray() => _list.ToArray();

            protected virtual ref Node Reference(int index, ref int node_)
            {
                var inst = _list; 
                var nodes = inst.m_list;

                int i = inst.m_head_idx;
                while (--index >= 0)
                    i = nodes[i].next;

                // synchronize
                _version = inst.m_version;

                node_ = i;
                return ref nodes[i];
            }

            public void Dispose()
            {
            }
        }

        private sealed class ReverseIterator : Iterator
        {
            public ReverseIterator(IndexLinkedList<T> inst) : base(inst)
            {
            }

            public override void Reset()
            {
                _version = _list.m_version;
                _node = _list.LastIndex;
                _index = 0;
            }

            protected override ref Node Reference(int index, ref int node_)
            {
                var inst = _list;
                var nodes = inst.m_list;

                int i = inst.LastIndex;
                while (--index >= 0)
                    i = nodes[i].prev;

                // synchronize
                _version = inst.m_version;

                node_ = i;
                return ref nodes[i];
            }
        }
        #endregion
    }
}
