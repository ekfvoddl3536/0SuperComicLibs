using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerTypeProxy(typeof(EnumerableView<>))]
    [DebuggerDisplay("Count = {size}")]
    public sealed class FixedQueue<T> : IEnumerable<T>
    {
        private readonly T[] arr;
        private int head;
        private int tail;
        private int size;

        #region constructor
        public FixedQueue(int initalSize) => arr = new T[initalSize];
        #endregion

        #region property
        public int Count => size;

        public int Capacity => arr.Length;
        #endregion

        #region methods
        public void Enqueue(T item)
        {
            T[] arr = this.arr;
            if (size++ >= arr.Length)
                throw new IndexOutOfRangeException();

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

        private T GetElement(int idx) => arr[(head + idx) % arr.Length];
        #endregion

        #region enumerator
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T[] ToArray()
        {
            int len = size;
            if (len == 0)
                return Array.Empty<T>();

            T[] res = new T[len];
            int x = head;

            for (int z = 0; x < len;)
                res[z++] = GetElement(x++);

            return res;
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private FixedQueue<T> q;
            private int index;
            private T current;

            public Enumerator(FixedQueue<T> q)
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
                    current = q.GetElement(index++);
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