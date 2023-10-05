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
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Collections
{
    [DebuggerTypeProxy(typeof(EnumerableView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class UnsafeList<T> : IList<T>, IList, IReadOnlyList<T>, IReverseEnumerable<T>, IAddOnlyList<T>, IStack<T>, IVersionControlledCollection
    {
        protected T[] _array;
        [ContractPublicPropertyName("Count")]
        protected int _size;
        protected int _version;
        [NonSerialized]
        protected object _syncRoot;

        #region constructors
        public UnsafeList() : this(4) { }

        public UnsafeList(int initial_capacity)
        {
            if (initial_capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initial_capacity));

            _array = initial_capacity == 0 ? Array.Empty<T>() : new T[initial_capacity];
            _size = 0;
            _version = 0;
            _syncRoot = null;
        }

        public UnsafeList(IEnumerable<T> collection)
        {
            _array = new T[4];
            _size = 0;
            _version = 0;
            _syncRoot = null;

            AddRange(collection);
        }
        #endregion

        #region property
        public int Count => _size;
        public int Capacity
        {
            get => _array.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException(nameof(value));

                SetCapacityAlign4_Core(value);
            }
        }
        public bool IsReadOnly => false;
        #endregion

        #region array access
        [AssumeInputsValid]
        public ref T this[[ValidRange] int index] => ref _array[index];

        [AssumeInputsValid]
        public Memory<T> this[[ValidRange] Range range] => new Memory<T>(_array, range.start, range.end - range.start);

        [AssumeInputsValid]
        public Memory<T> this[[ValidRange] int index, [ValidRange] int count] => new Memory<T>(_array, index, count);

        T IReadOnlyList<T>.this[int index] => at(index);
        T IList<T>.this[int index]
        {
            get => at(index);
            set => at(index) = value;
        }

        public ref T at(int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _array[index];
        }

        public Memory<T> at(Range range)
        {
            if ((range.start | range.end) < 0 ||
                range.end - range.start > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(range));

            return new Memory<T>(_array, range.start, range.end - range.start);
        }

        public Memory<T> at(int index, int count)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Memory<T>(_array, index, count);
        }
        #endregion

        #region add + addrange + insert
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add([AllowNull] T item)
        {
            Reserve(1);

            _array[_size] = item;
            ++_size;
            ++_version;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is ICollection t0)
                Reserve(t0.Count);
            else if (collection is IReadOnlyCollection<T> t1)
                Reserve(t1.Count);

            var e1 = collection.GetEnumerator() ?? throw new ArgumentNullException(nameof(collection), "null returned from " + nameof(IEnumerable.GetEnumerator));
            AddRange_Core(e1);
            e1.Dispose();
        }

        public void AddRange([KeepAlive] IEnumerator<T> enumerator, [Optional] int count_hint)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            if (count_hint > 0)
                Reserve(count_hint);

            AddRange_Core(enumerator);
        }

        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange_Core([DisallowNull, KeepAlive] IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                Add(enumerator.Current);

            ++_version;
        }

        public void AddRange(Memory<T> memory)
        {
            if (!memory.IsValid)
                throw new ArgumentOutOfRangeException(nameof(memory));

            AddRange_Core(memory);
        }

        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange_Core([DisallowNull, ValidRange] Memory<T> memory)
        {
            Reserve(memory.Length);

            Array.Copy(memory._source, memory._start, _array, _size, memory.Length);
            _size += memory.Length;
            ++_version;
        }

        public void Insert(int index, [AllowNull] T item)
        {
            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == _size)
                Add(item);
            else
                Insert_Core(index, item);
        }

        [AssumeInputsValid, AssumeOperationValid]
        public void Insert_Core([ValidRange] int index, [AllowNull] T item)
        {
            Reserve(1);

            Array.Copy(_array, index, _array, index + 1, _size - index);
            _array[index] = item;

            ++_size;
            ++_version;
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            int cnt;
            if (collection is ICollection t0)
                cnt = t0.Count;
            else if (collection is IReadOnlyCollection<T> t1)
                cnt = t1.Count;
            else
            {
                InsertRange(index, collection.GetEnumerator());
                return;
            }

            Reserve(cnt);

            var list = _array;
            Array.Copy(list, index, list, index + cnt, _size - index);

            var e1 = collection.GetEnumerator() ?? throw new ArgumentNullException(nameof(collection), "null returned from GetEnumerator()");
            for (int si = index, di = index + cnt; si < di && e1.MoveNext();)
                list[si++] = e1.Current;

            e1.Dispose();

            _size += cnt;
        }

        public void InsertRange(int index, IEnumerator<T> enumerator, [Optional] int count_hint)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count_hint > 0)
            {
                Reserve(count_hint);

                var list = _array;
                Array.Copy(list, index, list, index + count_hint, _size - index);

                for (int si = index, di = index + count_hint; si < di && enumerator.MoveNext();)
                    list[si++] = enumerator.Current;

                _size += count_hint;
            }
            else
            {
                for (; enumerator.MoveNext(); ++index)
                    Insert_Core(index, enumerator.Current);
            }
        }

        public void InsertRange(int index, Memory<T> memory)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!memory.IsValid)
                throw new ArgumentOutOfRangeException(nameof(memory));

            InsertRange_Core(index, memory);
        }

        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange_Core([ValidRange] int index, [DisallowNull, ValidRange] Memory<T> memory)
        {
            Reserve(memory.Length);

            Array.Copy(_array, index, _array, index + memory.Length, _size - index);
            Array.Copy(memory._source, memory._start, _array, index, memory.Length);

            _size += memory.Length;
            ++_version;
        }
        #endregion

        #region asreadonly, binarysearch
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyCollection<T> AsReadOnly() => new ReadOnlyCollection<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch([AllowNull] T item) =>
            Array.BinarySearch(_array, 0, _size, item, Comparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch([AllowNull] T item, [AllowNull] IComparer<T> comparer) =>
            Array.BinarySearch(_array, 0, _size, item, comparer ?? Comparer<T>.Default);

        public int BinarySearch(int index, int count, [AllowNull] T item, [AllowNull] IComparer<T> comparer)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if ((uint)count > _size - index)
                throw new ArgumentOutOfRangeException(nameof(count));

            return Array.BinarySearch(_array, index, count, item, comparer ?? Comparer<T>.Default);
        }

        [AssumeInputsValid]
        public int BinarySearch_Core([ValidRange] int index, [ValidRange] int count, [AllowNull] T item, [DisallowNull] IComparer<T> comparer) =>
            Array.BinarySearch(_array, index, count, item, comparer);
        #endregion

        #region indexof + lastindexof
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf([AllowNull] T item) =>
            IndexOf_Core(item, 0, _size, EqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf([AllowNull] T item, [AllowNull] IEqualityComparer<T> comparer) =>
            IndexOf_Core(item, 0, _size, comparer ?? EqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf([AllowNull] T item, int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return IndexOf_Core(item, index, _size - index, EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf([AllowNull] T item, int index, [AllowNull] IEqualityComparer<T> comparer)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return IndexOf_Core(item, index, _size - index, comparer ?? EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf([AllowNull] T item, int index, int count)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            return IndexOf_Core(item, index, count, EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf([AllowNull] T item, int index, int count, [AllowNull] IEqualityComparer<T> comparer)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            return IndexOf_Core(item, index, count, comparer ?? EqualityComparer<T>.Default);
        }

        [AssumeInputsValid]
        public int IndexOf_Core([AllowNull] T item, [ValidRange] int index, [ValidRange] int count, [DisallowNull] IEqualityComparer<T> comparer)
        {
            var list = _array;
            for (var di = index + count; index < di; ++index)
                if (comparer.Equals(list[index], item))
                    return index;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf([AllowNull] T item) =>
            LastIndexOf_Core(item, 0, _size, EqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf([AllowNull] T item, [AllowNull] IEqualityComparer<T> comparer) =>
            LastIndexOf_Core(item, 0, _size, comparer ?? EqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf([AllowNull] T item, int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return LastIndexOf_Core(item, index, _size - index, EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf([AllowNull] T item, int index, [AllowNull] IEqualityComparer<T> comparer)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            return LastIndexOf_Core(item, index, _size - index, comparer ?? EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf([AllowNull] T item, int index, int count)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            return LastIndexOf_Core(item, index, count, EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf([AllowNull] T item, int index, int count, [AllowNull] IEqualityComparer<T> comparer)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            return LastIndexOf_Core(item, index, count, comparer ?? EqualityComparer<T>.Default);
        }

        [AssumeInputsValid]
        public int LastIndexOf_Core([AllowNull] T item, [ValidRange] int index, [ValidRange] int count, [DisallowNull] IEqualityComparer<T> comparer)
        {
            var list = _array;
            for (var si = index + count; --si >= index;)
                if (comparer.Equals(list[index], item))
                    return index;

            return -1;
        }
        #endregion

        #region remove + removeat + removerange + removeall + clear
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove([AllowNull] T item) =>
            Remove_Core(item, EqualityComparer<T>.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove([AllowNull] T item, [AllowNull] IEqualityComparer<T> comparer) =>
            Remove_Core(item, comparer ?? EqualityComparer<T>.Default);

        [AssumeInputsValid]
        public bool Remove_Core([AllowNull] T item, [DisallowNull] IEqualityComparer<T> comparer)
        {
            int idx = IndexOf_Core(item, 0, _size, comparer);
            if (idx < 0) return false;

            RemoveAt_Core(idx);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            RemoveAt_Core(index);
        }

        [AssumeInputsValid, AssumeOperationValid]
        public void RemoveAt_Core([ValidRange] int index)
        {
            Debug.Assert(_size > 0, nameof(ArgumentOutOfRangeException));

            --_size;
            if (_size != index)
                Array.Copy(_array, index + 1, _array, index, _size - index);

            _array[index] = default;
            ++_version;
        }

        public void RemoveRange(int index, int count)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 1)
                RemoveAt_Core(index);
            else
                RemoveRange_Core(index, count);
        }

        [AssumeInputsValid, AssumeOperationValid]
        public void RemoveRange_Core([ValidRange] int index, [ValidRange] int count)
        {
            Debug.Assert(_size >= count, nameof(ArgumentOutOfRangeException));

            _size -= count;
            if (_size != index)
            {
                Array.Copy(_array, index + count, _array, index, _size - index);
                Array.Clear(_array, index, count);
            }

            ++_version;
        }

        public int RemoveAll(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return RemoveAll_Core(predicate);
        }

        [AssumeInputsValid]
        public int RemoveAll_Core([DisallowNull] Predicate<T> predicate)
        {
            var list = _array;

            Range freeRng = default;
            int count = _size;

            // find the first item which needs to be removed
            while (freeRng.start < count && !predicate.Invoke(list[freeRng.start]))
                ++freeRng.start;

            if (freeRng.start == count)
                return 0;

            int freeIdx = freeRng.start;
            for (; ; )
            {
                freeRng.end = freeRng.start + 1;
                while (freeRng.end < count && predicate.Invoke(list[freeRng.end]))
                    ++freeRng.end;

                freeRng.start = freeRng.end + 1;
                while (freeRng.start < count && !predicate.Invoke(list[freeRng.start]))
                    ++freeRng.start;

                int length = freeRng.start - freeRng.end;
                Array.Copy(list, freeRng.end, list, freeIdx, length);
                freeIdx += length;

                if (freeRng.start == count)
                    break;
            }

            freeRng.start = _size - freeIdx;
            Array.Clear(list, freeIdx, freeRng.start);
            _size = freeIdx;
            ++_version;

            return freeRng.start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
            ++_version;
        }

        public void FastClear()
        {
            _size = 0;
            ++_version;
        }
        #endregion

        #region setcapacity + contains + copyto + getenumerator + convertall
        public UnsafeList<TOut> ConvertAll<TOut>(Converter<T, TOut> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return ConvertAll_Core(converter);
        }

        [AssumeInputsValid]
        public UnsafeList<TOut> ConvertAll_Core<TOut>([DisallowNull] Converter<T, TOut> converter)
        {
            var res = new UnsafeList<TOut>(_size);

            var dst = res._array;
            var src = _array;
            for (int si = 0; si < dst.Length; ++si)
                dst[si] = converter.Invoke(src[si]);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reserve(int count)
        {
            if (_size + count > _array.Length)
                SetCapacityAlign4_Core(_size + count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EnsureCapacity(int capacity)
        {
            if (capacity > _array.Length)
                SetCapacityAlign4_Core(capacity);

            return _array.Length;
        }

        [AssumeInputsValid, AssumeOperationValid]
        public void SetCapacityAlign4_Core([ValidRange] int capacity)
        {
            const int ALIGN = 4 - 1;

            int align4 = (capacity + ALIGN) & ~ALIGN;

            var ns = new T[align4];

            Array.Copy(_array, ns, _size);

            _array = ns;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains([AllowNull] T item) => IndexOf_Core(item, 0, _size, EqualityComparer<T>.Default) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains([AllowNull] T item, [AllowNull] IEqualityComparer<T> comparer) => IndexOf(item, comparer) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_array, 0, array, arrayIndex, _size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (_size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            Array.Copy(_array, index, array, arrayIndex, count);
        }

        public IEnumerator<T> GetReverseEnumerator() => new VersionControlledArrayReverseEnumerator<T>(this, _array, 0, _size);
        public IEnumerator<T> GetEnumerator() => new VersionControlledArrayEnumerator<T>(this, _array, 0, _size);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IVersionControlledCollection.LatestVersion => _version;
        #endregion

        #region list api
        public bool Exist(Predicate<T> match) => FindIndex(match) != -1;

        [AssumeInputsValid]
        public bool Exist_Core([DisallowNull] Predicate<T> match) => FindIndex_Core(0, _size, match) != -1;

        public int FindIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindIndex_Core(0, _size, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            if ((uint)startIndex >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindIndex_Core(startIndex, _size - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if ((uint)count > _size - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindIndex_Core(startIndex, count, match);
        }

        [AssumeInputsValid]
        public int FindIndex_Core([ValidRange] int startIndex, [ValidRange] int count, [DisallowNull] Predicate<T> match)
        {
            var list = _array;
            for (int si = startIndex, di = si + count; si < di; ++si)
                if (match.Invoke(list[si]))
                    return si;

            return -1;
        }

        public T Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return Find_Core(match);
        }

        [AssumeInputsValid]
        public T Find_Core([DisallowNull] Predicate<T> match)
        {
            var list = _array;
            for (int si = 0, di = _size; si < di; ++si)
                if (match.Invoke(list[si]))
                    return list[si];

            return default;
        }

        public UnsafeList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentOutOfRangeException(nameof(match));

            return FindAll_Core(match);
        }

        [AssumeInputsValid]
        public UnsafeList<T> FindAll_Core([DisallowNull] Predicate<T> match)
        {
            var res = new UnsafeList<T>();

            var list = _array;
            for (int si = 0, di = _size; si < di; ++si)
                if (match.Invoke(list[si]))
                    res.Add(list[si]);

            return res;
        }

        public T FindLast(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindLast_Core(match);
        }

        [AssumeInputsValid]
        public T FindLast_Core([DisallowNull] Predicate<T> match)
        {
            var list = _array;
            for (int si = _size; --si >= 0;)
                if (match.Invoke(list[si]))
                    return list[si];

            return default;
        }

        public int FindLastIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindLastIndex_Core(0, _size, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            if ((uint)startIndex >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindLastIndex_Core(startIndex, _size - startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if ((uint)count > _size - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return FindLastIndex_Core(startIndex, count, match);
        }

        [AssumeInputsValid]
        public int FindLastIndex_Core([ValidRange] int startIndex, [ValidRange] int count, [DisallowNull] Predicate<T> match)
        {
            var list = _array;
            for (int si = startIndex + count; --si >= startIndex;)
                if (match.Invoke(list[si]))
                    return si;

            return -1;
        }

        public void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            ForEach_Core(action);
        }

        [AssumeInputsValid]
        public void ForEach_Core([DisallowNull] Action<T> action)
        {
            var list = _array;
            for (int si = 0, di = _size; si < di; ++si)
                action.Invoke(list[si]);
        }

        public UnsafeList<T> GetRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            return GetRange_Core(index, count);
        }

        [AssumeInputsValid]
        public UnsafeList<T> GetRange_Core([ValidRange] int index, [ValidRange] int count)
        {
            var list = new UnsafeList<T>(count);
            Array.Copy(_array, index, list._array, 0, count);
            list._size = count;

            return list;
        }

        public void Reverse() => Reverse_Core(0, _size);

        public void Reverse(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            Reverse_Core(index, count);
        }

        [AssumeInputsValid]
        public void Reverse_Core([ValidRange] int index, [ValidRange] int count)
        {
            var list = _array;

            for (int si = index, di = index + count - 1; si < di;)
            {
                ref T psi = ref list[si++];
                ref T pdi = ref list[di--];

                T vtmp = psi;
                psi = pdi;
                pdi = vtmp;
            }
        }

        public void Sort() => Sort_Core(0, _size, Comparer<T>.Default);


        public void Sort([AllowNull] IComparer<T> comparer) => Sort_Core(0, _size, comparer ?? Comparer<T>.Default);

        public void Sort(int index, int count, [AllowNull] IComparer<T> comparer)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || _size - index < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            Sort_Core(index, count, comparer ?? Comparer<T>.Default);
        }

        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            Sort_Core(0, _size, new FunctorComparer<T>(comparison));
        }

        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort_Core([ValidRange] int index, [ValidRange] int count, [DisallowNull] IComparer<T> comparer) =>
            Array.Sort(_array, index, count, comparer);

        public T[] ToArray()
        {
            if (_size == 0)
                return Array.Empty<T>();

            var res = new T[_size];
            Array.Copy(_array, res, _size);

            return res;
        }

        public void TrimExcess()
        {
            if (_size == 0)
                _array = Array.Empty<T>();
            else if (_size != _array.Length)
            {
                var ns = new T[_size];
                Array.Copy(_array, ns, _size);
                _array = ns;
            }
        }

        public bool TrueForAll(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return TrueForAll_Core(predicate);
        }

        [AssumeInputsValid]
        public bool TrueForAll_Core([DisallowNull] Predicate<T> predicate)
        {
            var list = _array;
            for (int si = 0, di = _size; si < di; ++si)
                if (!predicate.Invoke(list[si]))
                    return false;

            return true;
        }
        #endregion

        #region special method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference() => ref _array[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> AsMemory() => new Memory<T>(_array, 0, _size);
        #endregion

        #region istack
        void IStack<T>.Push(T item) => Add(item);

        T IStack<T>.Pop()
        {
            if (_size <= 0)
                throw new InvalidOperationException("Empty collection");

            ref T pitem = ref _array[--_size];
            T res = pitem;
            pitem = default;

            return res;
        }

        T IStack<T>.Peek()
        {
            if (_size <= 0)
                throw new InvalidOperationException("Empty collection");

            return _array[_size - 1];
        }
        #endregion

        #region explicit impl
        int IList.Add(object value)
        {
            if (value is T v0)
            {
                Add(v0);
                return _size - 1;
            }

            throw new ArgumentException(value.GetType().FullName, nameof(value));
        }

        bool IList.Contains(object value)
        {
            if (value is T v0)
                return Contains(v0);

            throw new ArgumentException(value.GetType().FullName, nameof(value));
        }

        int IList.IndexOf(object value)
        {
            if (value is T v0)
                return IndexOf(v0);

            throw new ArgumentException(value.GetType().FullName, nameof(value));
        }

        void IList.Insert(int index, object value)
        {
            if (value is T v0)
                Insert(index, v0);
            else
                throw new ArgumentException(value.GetType().FullName, nameof(value));
        }

        void IList.Remove(object value)
        {
            if (value is T v0)
                Remove(v0);
            else
                throw new ArgumentException(value.GetType().FullName, nameof(value));
        }

        object IList.this[int index]
        {
            get => at(index);
            set
            {
                if (value is T v0)
                    at(index) = v0;
                else
                    throw new ArgumentException(value.GetType().FullName, nameof(value));
            }
        }

        bool IList.IsFixedSize => false;

        void ICollection.CopyTo(Array array, int index)
        {
            try
            {
                Array.Copy(_array, 0, array, index, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(array.GetType().FullName, nameof(array));
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);

                return _syncRoot;
            }
        }
        bool ICollection.IsSynchronized => false;
        #endregion
    }
}
