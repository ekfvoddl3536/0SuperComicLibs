using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
#if DEBUG
    [System.Diagnostics.DebuggerTypeProxy(typeof(DEBUG_StackView<>))]
    [System.Diagnostics.DebuggerDisplay("Count = {size}")]
#endif
    public sealed class FixedStack<T> : IIterable<T>, IEnumerable<T>, IStack<T>
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

        public IForwardIterator<T> Begin() => arr.Slice(0, cnt).Begin();

        public IForwardIterator<T> RBegin() => arr.Slice(0, cnt).RBegin();

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