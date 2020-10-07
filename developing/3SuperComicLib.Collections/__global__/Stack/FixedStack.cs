using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
#if DEBUG
    [System.Diagnostics.DebuggerTypeProxy(typeof(DEBUG_StackView<>))]
    [System.Diagnostics.DebuggerDisplay("Count = {size}")]
#endif
    public sealed class FixedStack<T> : IIterable<T>, IEnumerable<T>
    {
        private readonly T[] arr;
        private int size;

        public FixedStack(int initsize) => arr = new T[initsize];

        public T[] Source => arr;

        public int Count => size;

        public int Capacity => arr.Length;

        public void Push(T item) => arr[size++] = item;

        public T Peek() => arr[size - 1];

        public T Pop()
        {
            ref T now = ref arr[--size];

            T result = now;
            now = default;

            return result;
        }

        public T FastPop() => arr[--size];

        public T[] ToArray()
        {
            int length = arr.Length;
            T[] result = new T[length];

            Array.Copy(arr, result, length);

            return result;
        }

        public IForwardIterator<T> Begin() => arr.Slice(0, size).Begin();

        public IForwardIterator<T> RBegin() => arr.Slice(0, size).RBegin();

        public IEnumerator<T> GetEnumerator() => arr.Slice(0, size).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}