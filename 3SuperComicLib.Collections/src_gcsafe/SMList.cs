// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#pragma warning disable IDE1006
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using SuperComicLib.RuntimeMemoryMarshals;

namespace SuperComicLib.Collections
{
    /// <summary>
    /// Semi-Managed List&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(RawContainerView<>))]
    [DebuggerDisplay("{Count}")]
    public unsafe class SMList<T> : IList<T>, IList, ICollection, IReadOnlyList<T>, IRawList<T>, IReadOnlyRawContainer<T>, IStack<T>
        where T : unmanaged
    {
        private const string ERR_MESSAGE_EMPTYCOLLECTION = "EmptyCollection";
        private const string ERR_MESSAGE_INVALIDOFFLEN = "InvalidOffLen";

        private static long _tempLock;
        private static arrayref<T> _emptyArray;

        private static ref readonly arrayref<T> EmptyArray
        {
            get
            {
                if (_emptyArray.IsNull)
                {
                    for (SpinWait w = default; Interlocked.CompareExchange(ref _tempLock, 1, 0) != 0;)
                        w.SpinOnce();

                    try
                    {
                        if (_emptyArray.IsNull)
                            _emptyArray = arrayref<T>.newf(0);
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _tempLock, 0);
                    }
                }

                return ref _emptyArray;
            }
        }

        private arrayref<T> _items;
        private int _size;
        private int _version;
        [NonSerialized]
        private object _syncRoot;

        #region constructors
        public SMList() => _items = EmptyArray;

        public SMList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _items = 
                capacity == 0 
                ? EmptyArray 
                : arrayref<T>.newf(capacity);
        }

        public SMList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count == 0)
                    _items = EmptyArray;
                else
                {
                    _items = arrayref<T>.newf(count);
                    c.CopyTo(_items.AsManaged(), 0);
                    _size = count;
                }
            }
            else
            {
                _items = EmptyArray;
                foreach (var e in collection)
                    Add(e);
            }
        }

        public SMList(in NativeConstSpan<T> span)
        {
            if ((ulong)span.Length > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(span));

            int count = (int)span.Length;
            if (count == 0)
                _items = EmptyArray;
            else
            {
                _items = arrayref<T>.newf(count);
                span.CopyTo(_items.AsSpan());
                _size = count;
            }
        }
        #endregion

        #region properties
        public int Count => _size;
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (value == _items.Length)
                    return;

                if (value == 0)
                {
                    _items.Dispose();
                    _items = EmptyArray;
                    return;
                }

                var ns = arrayref<T>.newf(value);
                if (_size > 0)
                {
                    AsSpan().CopyTo(ns.AsSpan());

                    _items.Dispose();
                }

                _items = ns;
            }
        }
        #endregion

        #region indexer
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_size)
                    throw new IndexOutOfRangeException(nameof(index));

                return _items[index];
            }

            set
            {
                if ((uint)index >= (uint)_size)
                    throw new IndexOutOfRangeException(nameof(index));

                _version++;
                _items[index] = value;
            }
        }
        #endregion

        #region add, insert, contains
        public bool Contains(T item) => IndexOf(item) >= 0;

        public void Add(T item)
        {
            reserve(_size + 1);

            _items[_size++] = item;
            _version++;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is ICollection<T> c)
            {
                if (c.Count <= 0)
                    return;

                reserve(_size + c.Count);

                c.CopyTo(_items.AsManaged(), _size);

                _size += c.Count;
                _version++;
            }
            else
                foreach (var e in collection)
                    Add(e);
        }

        public void AddRange(in NativeConstSpan<T> span)
        {
            if ((ulong)(_size + span.Length) > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(span));

            if (span.Length <= 0)
                return;

            reserve(_size + (int)span.Length);

            span.CopyTo(_items.AsSpan().Slice((uint)_size));

            _size += (int)span.Length;

            _version++;
        }

        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            reserve(_size + 1);

            var psi = (T*)_items.GetDataPointer() + index;
            if (index != _size)
            {
                var cb = (ulong)(_size - index) * (uint)sizeof(T);

                MemoryBlock.Memmove(psi, psi + 1, cb);
            }

            *psi = item;

            _size++;
            _version++;
        }

        public void Insert(int index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (collection is ICollection<T> c)
            {
                if (c.Count <= 0)
                    return;

                reserve(_size + c.Count);

                if (index != _size)
                {
                    var psi = (T*)_items.GetDataPointer() + index;
                    var cb = (ulong)(_size - index) * (uint)sizeof(T);

                    MemoryBlock.Memmove(psi, psi + c.Count, cb);
                }

                c.CopyTo(_items.AsManaged(), index);

                _size++;
                _version++;
            }
            else
                foreach (var e in collection)
                    Insert(index++, e);
        }

        public void Insert(int index, in NativeConstSpan<T> span)
        {
            if ((ulong)(_size + span.Length) > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(span));

            if (span.Length <= 0)
                return;

            reserve(_size + (int)span.Length);

            var psi = (T*)_items.GetDataPointer() + index;
            if (index != _size)
            {
                var cb = (ulong)(_size - index) * (uint)sizeof(T);

                MemoryBlock.Memmove(psi, psi + (int)span.Length, cb);
            }

            MemoryBlock.Memmove(span.DangerousGetPointer(), psi, span.capacity());

            _size += (int)span.Length;
            _version++;
        }
        #endregion

        #region clear, remove
        public void Clear()
        {
            _items.AsSpan().Clear();

            // fast-clear
            _size = 0;
            _version++;
        }

        /// <summary>
        /// Skip zero-init of the array.
        /// </summary>
        public void FastClear()
        {
            _size = 0;
            _version++;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            var ptr = (T*)_items.GetDataPointer();
            _eraseCore(ptr, ptr + index);
        }

        public bool Remove(T item)
        {
            var idx = IndexOf(item);
            if (idx >= 0)
            {
                var ptr = (T*)_items.GetDataPointer();
                _eraseCore(ptr, ptr + idx);
                return true;
            }
            else
                return false;
        }
        #endregion

        #region copyTo
        public void CopyTo(int startIndex, T[] array, int arrayIndex, int count) =>
            _items.AsMemory().CopyTo(startIndex, array.Slice(arrayIndex), count);

        public void CopyTo(T[] array, int arrayIndex) =>
            _items.AsMemory().Slice(0, _size).CopyTo(array, arrayIndex);

        public void CopyTo(T[] array) =>
            _items.AsMemory().Slice(0, _size).CopyTo(array);
        #endregion

        #region util
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>((T*)_items.GetDataPointer(), _size);
        #endregion

        #region enumerator & iterator
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public _iterator<T> begin() => new _iterator<T>((T*)_items.GetDataPointer());
        public _iterator<T> end() => new _iterator<T>((T*)_items.GetDataPointer() + _size);
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>((T*)_items.GetDataPointer() + (_size - 1));
        public reverse_iterator<T> rend() => new reverse_iterator<T>((T*)_items.GetDataPointer() - 1);
        public const_iterator<T> cbegin() => begin();
        public const_iterator<T> cend() => end();
        public const_reverse_iterator<T> crbegin() => rbegin();
        public const_reverse_iterator<T> crend() => rend();

        public RawContainerBuffer getRawContainerBuffer() => new RawContainerBuffer(_items.GetDataPointer(), _size);
        #endregion

        #region explicit implements
        T IReadOnlyList<T>.this[int index] => this[index];
        bool IList.IsReadOnly => false;
        bool ICollection<T>.IsReadOnly => false;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);

                return _syncRoot;
            }
        }

        int IList.Add(object value)
        {
            if (value is T unbox)
            {
                var idx = _size;
                Add(unbox);
                return idx;
            }
            else
                return -1;
        }

        bool IList.Contains(object value) =>
            value is T unbox && Contains(unbox);

        int IList.IndexOf(object value) =>
            value is T unbox
            ? IndexOf(unbox)
            : -1;

        void IList.Insert(int index, object value) => Insert(index, (T)value);

        void IList.Remove(object value) => Remove((T)value);

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        bool IList.IsFixedSize => false;

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1)
                throw new ArgumentException("RankMultiDimNotSupported", nameof(array));

            Array.Copy(_items.AsManaged(), array, _size);
        }
        #endregion

        #region rawcontainer (explicit implements)
        ref readonly T IReadOnlyRawContainer<T>.this[long index] => ref _items[index];
        ref readonly T IReadOnlyRawContainer<T>.at(long index)
        {
            if ((ulong)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _items[index];
        }
        ref T IRawContainer<T>.this[long index] => ref _items[index];
        ref T IRawContainer<T>.at(long index)
        {
            if ((ulong)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _items[index];
        }

        long IRawContainer.size() => _size;
        long IRawContainer.capacity() => _items.Length;

        void IRawList<T>.push_back(in T item) => Add(item);

        T IRawList<T>.pop_back()
        {
            if (_size == 0)
                throw new InvalidOperationException(ERR_MESSAGE_EMPTYCOLLECTION);

            _version++;
            return _items[--_size];
        }

        void IRawList<T>.insert(long index, in T item)
        {
            if ((ulong)index > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index));

            Insert((int)index, item);
        }

        bool IRawList<T>.removeAt(long index)
        {
            if ((ulong)index >= (uint)_size)
                return false;

            RemoveAt((int)index);
            return true;
        }
        #endregion

        #region rawcontainer
        public void erase(_iterator<T> position)
        {
            var psi = (T*)_items.GetDataPointer();
            if ((ulong)(position._ptr - psi) >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(position));

            _eraseCore(psi, position._ptr);
        }

        public void erase(_iterator<T> first, _iterator<T> last)
        {
            var psi = (T*)_items.GetDataPointer();
            var pdi = psi + (uint)_size;

            if (psi > first._ptr || pdi < last._ptr || last._ptr < first._ptr)
                throw new ArgumentOutOfRangeException($"{nameof(first)} or {nameof(last)}");

            T* dst = last._ptr + 1;

            MemoryBlock.Memmove(dst, first._ptr, (ulong)((byte*)pdi - (byte*)dst));

            _size -= (int)(last._ptr - first._ptr);
            _version++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void reserve(int capacity)
        {
            if (capacity > _size)
                _increaseCapacity(capacity);
        }

        private void _eraseCore(T* psi, T* pos)
        {
            T* dst = pos + 1;
            T* last = psi + (uint)_size;

            MemoryBlock.Memmove(dst, pos, (ulong)((byte*)last - (byte*)dst));

            --_size;
            _version++;
        }

        private void _increaseCapacity(int capacity)
        {
            // align4
            capacity = (capacity + 3) & ~3;

            capacity = Math.Max(_items.Length << 1, capacity);

            var ns = arrayref<T>.newf(capacity);

            AsSpan().CopyTo(ns.AsSpan());

            if (EmptyArray.AsManaged() != _items.AsManaged())
                _items.Dispose();

            _items = ns;
        }
        #endregion

        #region stack
        void IStack<T>.Push(T item) => Add(item);

        T IStack<T>.Pop()
        {
            if (_size == 0)
                throw new InvalidOperationException(ERR_MESSAGE_EMPTYCOLLECTION);

            ++_version;
            return _items[--_size];
        }

        T IStack<T>.Peek() =>
            _size == 0 
            ? throw new InvalidOperationException(ERR_MESSAGE_EMPTYCOLLECTION) 
            : _items[_size - 1];
        #endregion

        #region List<T> API (.NET 7)
        #region binary search
        public int BinarySearch(T item) => _binarySearchCore(0, _size - 1, item, Comparer<T>.Default);

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            return _binarySearchCore(0, _size - 1, item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            if (((index >> 31) | (_size - index)) < (uint)count)
                throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN);

            if (comparer == null)
                comparer = Comparer<T>.Default;

            return _binarySearchCore(index, index + count - 1, item, comparer);
        }

        private int _binarySearchCore(int lo, int hi, in T item, IComparer<T> comparer)
        {
            T* first = (T*)_items.GetDataPointer();
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;

                int cmp = comparer.Compare(first[mid], item);
                if (cmp == 0)
                    return mid;
                else if (cmp < 0)
                    lo = mid + 1;
                else
                    hi = mid - 1;
            }

            return ~lo;
        }
        #endregion

        #region find, find-index
        /// <summary>
        /// <see cref="List{T}.Find(Predicate{T})"/>
        /// </summary>
        public T Find(Predicate<T> match)
        {
            var iter = (T*)_items.GetDataPointer();
            for (var end = iter + _size; iter != end; ++iter)
                if (match.Invoke(*iter))
                    return *iter;

            return default;
        }

        /// <summary>
        /// <see cref="List{T}.FindIndex(int, int, Predicate{T})"/>
        /// </summary>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if (((startIndex >> 31) | (_size - startIndex)) < (uint)count)
                throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN);

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return _findIndexCore(startIndex, count, match);
        }

        /// <summary>
        /// <see cref="List{T}.FindIndex(int, Predicate{T})"/>
        /// </summary>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            if ((uint)startIndex >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return _findIndexCore(startIndex, _size - startIndex, match);
        }

        /// <summary>
        /// <see cref="List{T}.FindIndex(Predicate{T})"/>
        /// </summary>
        public int FindIndex(Predicate<T> match) =>
            _findIndexCore(0, _size, match ?? throw new ArgumentNullException(nameof(match)));

        private int _findIndexCore(int si, int n, Predicate<T> m)
        {
            var first = (T*)_items.GetDataPointer();

            var iter = first + si;
            for (var end = iter + n; iter != end; ++iter)
                if (m.Invoke(*iter))
                    return (int)(iter - first);

            return -1;
        }
        #endregion

        #region findLast, findLast-index
        /// <summary>
        /// <see cref="List{T}.Find(Predicate{T})"/>
        /// </summary>
        public T FindLast(Predicate<T> match)
        {
            var rend = (T*)_items.GetDataPointer() - 1;
            for (var iter = rend + _size - 1; iter != rend; --iter)
                if (match.Invoke(*iter))
                    return *iter;

            return default;
        }

        /// <summary>
        /// <see cref="List{T}.FindIndex(int, int, Predicate{T})"/>
        /// </summary>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (((startIndex >> 31) | (_size - startIndex)) < (uint)count)
                throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN);

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return _findLastIndexCore(startIndex, count, match);
        }

        /// <summary>
        /// <see cref="List{T}.FindIndex(int, Predicate{T})"/>
        /// </summary>
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            if ((uint)startIndex >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return _findLastIndexCore(startIndex, _size - startIndex, match);
        }

        /// <summary>
        /// <see cref="List{T}.FindIndex(Predicate{T})"/>
        /// </summary>
        public int FindLastIndex(Predicate<T> match) =>
            _findLastIndexCore(0, _size, match ?? throw new ArgumentNullException(nameof(match)));

        private int _findLastIndexCore(int si, int n, Predicate<T> m)
        {
            var first = (T*)_items.GetDataPointer();

            var rend = first + si - 1;
            for (var iter = rend + n - 1; iter != rend; --iter)
                if (m.Invoke(*iter))
                    return (int)(iter - first);

            return -1;
        }
        #endregion

        #region indexOf
        public int IndexOf(T item) =>
            _indexOfCore(item, 0, _size);

        public int IndexOf(T item, int index) =>
            (uint)index >= (uint)_size 
            ? throw new ArgumentOutOfRangeException(nameof(index)) 
            : _indexOfCore(item, index, _size - index);

        public int IndexOf(T item, int index, int count) =>
            ((index >> 31) | (_size - index)) < (uint)count
            ? throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN)
            : _indexOfCore(item, index, count);

        private int _indexOfCore(in T item, int si, int n)
        {
            var ec = EqualityComparer<T>.Default;

            var first = (T*)_items.GetDataPointer();
            var iter = first + si;

            for (var end = iter + n; iter != end; ++iter)
                if (ec.Equals(*iter, item))
                    return (int)(iter - first);

            return -1;
        }
        #endregion

        #region lastIndexOf
        public int LastIndexOf(T item) =>
            _lastIndexOfCore(item, 0, _size);

        public int LastIndexOf(T item, int index) =>
            (uint)index >= (uint)_size
            ? throw new ArgumentOutOfRangeException(nameof(index))
            : _lastIndexOfCore(item, index, _size - index);

        public int LastIndexOf(T item, int index, int count) =>
            ((index >> 31) | (_size - index)) < (uint)count
            ? throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN)
            : _lastIndexOfCore(item, index, count);

        private int _lastIndexOfCore(in T item, int si, int n)
        {
            var ec = EqualityComparer<T>.Default;

            var first = (T*)_items.GetDataPointer();
            var rend = first + si - 1;

            for (var iter = rend + n - 1; iter != rend; --iter)
                if (ec.Equals(*iter, item))
                    return (int)(iter - first);

            return -1;
        }
        #endregion

        /// <summary>
        /// <see cref="List{T}.AsReadOnly"/>
        /// </summary>
        public ReadOnlyCollection<T> AsReadOnly() => new ReadOnlyCollection<T>(this);

        /// <summary>
        /// <see cref="List{T}.ConvertAll{TOutput}(Converter{T, TOutput})"/>
        /// </summary>
        public SMList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) where TOutput : unmanaged
        {
            var result = new SMList<TOutput>(_size);

            var pdi = (TOutput*)result._items.GetDataPointer();
            var psi = (T*)_items.GetDataPointer();

            for (var end = psi + _size; psi != end; ++psi, ++pdi)
                *pdi = converter.Invoke(*psi);
            
            return result;
        }

        /// <summary>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.ensurecapacity?view=net-7.0"/>
        /// </summary>
        public int EnsureCapacity(int capacity)
        {
            reserve(capacity);

            return _items.Length;
        }

        /// <summary>
        /// <see cref="List{T}.Exists(Predicate{T})"/>
        /// </summary>
        public bool Exist(Predicate<T> match)
        {
            var iter = (T*)_items.GetDataPointer();
            for (var end = iter + _size; iter != end; ++iter)
                if (match.Invoke(*iter))
                    return true;

            return false;
        }

        /// <summary>
        /// <see cref="List{T}.FindAll(Predicate{T})"/>
        /// </summary>
        public SMList<T> FindAll(Predicate<T> match)
        {
            var result = new SMList<T>();

            var iter = (T*)_items.GetDataPointer();
            for (var end = iter + _size; iter != end; ++iter)
                if (match.Invoke(*iter))
                    result.Add(*iter);

            return result;
        }

        /// <summary>
        /// <see cref="List{T}.ForEach(Action{T})"/>
        /// </summary>
        public void ForEach(Action<T> action)
        {
            var iter = (T*)_items.GetDataPointer();
            for (var end = iter + _size; iter != end;)
                action.Invoke(*iter++);
        }

        /// <summary>
        /// <see cref="List{T}.RemoveAll(Predicate{T})"/>
        /// </summary>
        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            int size = _size;
            int freeIndex = 0;

            T* items = (T*)_items.GetDataPointer();
            for (; ; ++freeIndex)
            {
                if (freeIndex >= size)
                    return 0;
                else if (match.Invoke(items[freeIndex]))
                    break;
            }

            int current = freeIndex + 1;
            while (current < size)
            {
                do
                    if (!match.Invoke(items[current]))
                    {
                        items[freeIndex++] = items[current++];
                        break;
                    }
                while (++current < size);
            }

            var cb = (ulong)(size - freeIndex) * (uint)sizeof(T);
            MemoryBlock.Clear((byte*)(items + freeIndex), cb);

            _size = freeIndex;
            _version++;

            return size - freeIndex;
        }

        #region reverse
        /// <summary>
        /// <see cref="List{T}.Reverse()"/>
        /// </summary>
        public void Reverse() => Reverse(0, _size);

        /// <summary>
        /// <see cref="List{T}.Reverse(int, int)"/>
        /// </summary>
        public void Reverse(int index, int count)
        {
            if (((index >> 31) | (_size - index)) < (uint)count)
                throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN);

            Array.Reverse(_items.AsManaged(), index, count);
            _version++;
        }
        #endregion

        #region sort
        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            if (_size > 0)
            {
                IComparer<T> comparer = new ComparerWrapper(comparison);
                Array.Sort(_items.AsManaged(), 0, _size, comparer);
            }
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (((index >> 31) | (_size - index)) < (uint)count)
                throw new ArgumentException(ERR_MESSAGE_INVALIDOFFLEN);

            Array.Sort(_items.AsManaged(), index, count, comparer);
            _version++;
        }

        public void Sort(IComparer<T> comparer) => Sort(0, _size, comparer);

        public void Sort() => Sort(0, _size, Comparer<T>.Default);
        #endregion

        public T[] ToArray()
        {
            if (_size <= 0)
                return Array.Empty<T>();

            var res = new T[_size];
            Array.Copy(_items.AsManaged(), res, _size);

            return res;
        }

        public SafeArrayref<T> ToSafeArrayref()
        {
            if (_size <= 0)
                return SafeArrayref<T>.Empty;

            var res = new SafeArrayref<T>(_size);
            Array.Copy(_items.AsManaged(), res.AsManaged(), _size);
            return res;
        }

        public arrayref<T> ToArrayref()
        {
            var res = arrayref<T>.newf(_size);
            Array.Copy(_items.AsManaged(), res.AsManaged(), _size);
            return res;
        }

        public NativeArray<T> ToNativeArray()
        {
            var res = new NativeArray<T>((uint)_size);
            AsSpan().CopyTo(res.AsSpan());
            return res;
        }

        public void TrimExcess()
        {
            int threshold = (int)(_items.Length * 0.9);
            if (_size < threshold)
                Capacity = _size;
        }

        public bool TrueForAll(Predicate<T> match)
        {
            var iter = (T*)_items.GetDataPointer();
            for (var end = iter + _size; iter != end; ++iter)
                if (!match.Invoke(*iter))
                    return false;

            return true;
        }
        #endregion

        #region finalize
        ~SMList()
        {
            if (EmptyArray.AsManaged() != _items.AsManaged())
                _items.Dispose();
        }
        #endregion

        #region nested
        private struct Enumerator : IEnumerator<T>
        {
            private readonly SMList<T> _list;
            private readonly int _version;
            private int _index;

            public Enumerator(SMList<T> list)
            {
                _list = list;
                _version = list._version;
                _index = -1;
            }

            public T Current => _list._items[_index];
            object IEnumerator.Current => Current;

            public bool MoveNext() =>
                _version != _list._version 
                ? throw new InvalidOperationException("CollectionModified!") 
                : ++_index < _list._size;

            public void Reset() => _index = -1;
            public void Dispose() { }
        }

        private readonly struct ComparerWrapper : IComparer<T>
        {
            private readonly Comparison<T> _func;

            public ComparerWrapper(Comparison<T> func) => _func = func;

            int IComparer<T>.Compare(T x, T y) => _func.Invoke(x, y);
        }
        #endregion
    }
}
