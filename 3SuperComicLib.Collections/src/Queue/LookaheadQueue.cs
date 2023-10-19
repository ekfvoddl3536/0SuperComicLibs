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
    public class LookaheadQueue<T> : IDisposable, IEnumerable<T>
    {
        private const int defaultCapacity = 8;

        protected T[] arr;
        protected int head;
        protected int tail;
        protected int size;

        public LookaheadQueue() => IncreaseCapacity(defaultCapacity);

        public LookaheadQueue(T[] source)
        {
            int x;
            if (source == null || (size = x = source.Length) <= 0)
            {
                IncreaseCapacity(defaultCapacity);
                return;
            }

            T[] vs = new T[x];

            while (--x >= 0)
                vs[x] = source[x];

            arr = vs;
        }

        #region property
        public int Count => size;

        public int Capacity => arr.Length;
        #endregion

        #region methods
        public void Enqueue(T item)
        {
            T[] arr = this.arr;
            if (size == arr.Length)
                IncreaseCapacity(Arrays.GetNextSize(size));
            size++;

            ref int t = ref tail;
            arr[t] = item;
            t = (t + 1) % arr.Length;
        }

        public T Dequeue()
        {
            if (--size < 0)
                throw new IndexOutOfRangeException();

            ref int h = ref head;
            T[] arr = this.arr;

            T result = arr[h];
            arr[h] = default;
            h = (h + 1) % arr.Length;

            return result;
        }

        public void FastEnqueue(T item)
        {
            size++;

            T[] arr = this.arr;
            ref int t = ref tail;
            arr[t] = item;
            t = (t + 1) % arr.Length;
        }

        public T FastDequeue()
        {
            size--;

            ref int h = ref head;
            T[] arr = this.arr;

            T result = arr[h];
            arr[h] = default;
            h = (h + 1) % arr.Length;

            return result;
        }

        public T Peek() =>
            size <= 0
            ? throw new IndexOutOfRangeException()
            : arr[head % arr.Length];

        public T PeekOrDefault() =>
            size <= 0
            ? default
            : arr[head];

        public T Peek(int idx) =>
            size - idx <= 0
            ? throw new IndexOutOfRangeException()
            : arr[(head + idx) % arr.Length];

        public T PeekOrDefault(int idx) =>
            size - idx <= 0
            ? default
            : arr[(head + idx) % arr.Length];

        public T FastPeek(int idx) => arr[(head + idx) % arr.Length];

        public void Clear()
        {
            T[] arr = this.arr;
            int size = this.size;
            int head = this.head;
            int len = arr.Length;
            for (int x = 0; x < size;)
                arr[(head + x++) % len] = default;
        }
        #endregion

        #region capacity
        protected void IncreaseCapacity(int nextsize)
        {
            T[] newarr = new T[nextsize];

            int num = size - head;
            Array.Copy(arr, head, newarr, 0, num);

            Array.Copy(arr, 0, newarr, num, head);

            head = 0;
            tail = size;
            arr = newarr;
        }
        #endregion

        #region disposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            arr = null;
            head = tail = size = 0;
        }
        #endregion

        #region enumerator
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual T[] ToArray()
        {
            int sz;
            if ((sz = size) == 0)
                return Array.Empty<T>();

            T[] res = new T[sz];
            int x = head;

            for (int z = 0; x < sz;)
                res[z++] = FastPeek(x++);

            return res;
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private LookaheadQueue<T> q;
            private int index;
            private T current;

            public Enumerator(LookaheadQueue<T> q)
            {
                this.q = q;
                index = 0;
                current = default;
            }

            public T Current => current;

            object IEnumerator.Current => current;

            public bool MoveNext()
            {
                if (index < q.size)
                {
                    current = q.FastPeek(index++);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default;
            }

            public void Dispose()
            {
                q = null;
                index = 0;
                current = default;

                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}