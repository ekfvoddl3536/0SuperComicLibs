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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerTypeProxy(typeof(EnumerableView<>))]
    [DebuggerDisplay("Count = {size}")]
    public sealed class FixedList<T> : IEnumerable<T>, ICollection<T>, IList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        private readonly T[] arr;
        private int count;

        public FixedList(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            arr = new T[capacity];
        }

        public T this[int index]
        {
            get => arr[index];
            set => arr[index] = value;
        }

        public int Count => count;

        public int Capacity => arr.Length;

        public bool IsReadOnly => false;

        public void Add(T item) => arr[count++] = item;

        public void Insert(int index, T item)
        {
            var vs = arr;
            int cnt = count;

            if (cnt >= vs.Length)
                throw new IndexOutOfRangeException();

            for (int src = cnt - 1; src >= index; cnt = src, src--)
                vs[cnt] = vs[src];

            vs[index] = item;
        }

        public bool Contains(T item) =>
            IndexOf(item) >= 0;

        public int IndexOf(T item) =>
            IndexOf(item, EqualityComparer<T>.Default);

        public int IndexOf(T item, IEqualityComparer<T> comparer)
        {
            var vs = arr;

            for (int i = 0, max = count; i < max; i++)
                if (comparer.Equals(vs[i], item))
                    return i;

            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex) =>
            Array.Copy(arr, 0, array, arrayIndex, count);

        public bool Remove(T item) =>
            Remove(item, EqualityComparer<T>.Default);

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            int idx = IndexOf(item, comparer);
            if (idx >= 0)
            {
                RemoveAt(idx);
                return true;
            }
            else
                return false;
        }

        public void RemoveAt(int index)
        {
            int cnt = count;
            if (index < 0 || index >= cnt)
                throw new ArgumentOutOfRangeException(nameof(index));

            var vs = arr;
            for (int src = index + 1; src < cnt; index = src, src++)
                vs[index] = vs[src];

            vs[index] = default;
        }

        public void Clear(bool overwriteMode)
        {
            if (overwriteMode)
                count = 0;
            else
                Clear();
        }

        public void Clear()
        {
            Array.Clear(arr, 0, count);
            count = 0;
        }

        #region special methods
        public Memory<T> AsMemory() => new Memory<T>(arr, 0, count);
        #endregion

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)arr).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
