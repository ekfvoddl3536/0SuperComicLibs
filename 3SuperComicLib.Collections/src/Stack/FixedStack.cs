using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    [System.Diagnostics.DebuggerDisplay("Count = {size}")]
    public sealed class FixedStack<T> : IEnumerable<T>, IStack<T>
    {
        private T[] arr;
        private int cnt;

        public FixedStack(int initsize) => arr = new T[initsize];

        public T[] Source => arr;

        public int Count => cnt;

        public int Capacity => arr.Length;

        public void Push(T item) => arr[cnt++] = item;

        public T Peek() => arr[cnt - 1];

        public T Pop()
        {
            ref T now = ref arr[--cnt];

            T result = now;
            now = default;

            return result;
        }

        public T FastPop() => arr[--cnt];

        public T[] ToArray()
        {
            int length = arr.Length;
            T[] result = new T[length];

            Array.Copy(arr, result, length);

            return result;
        }

        public void Clear()
        {
            int x = cnt;
            while (--x >= 0)
                arr[x] = default;

            cnt = 0;
        }

        public IEnumerator<T> GetEnumerator() => arr.Slice(0, cnt).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            if (arr != null)
            {
                Clear();
                arr = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}