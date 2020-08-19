using System;

namespace SuperComicLib.Collections
{
    internal sealed class Array_ReverseIterator<T> : IForwardIterator<T>, IIndexable
    {
        private T[] src;
        private int max; // fast
        private int idx;

        public Array_ReverseIterator(T[] src)
        {
            this.src = src;

            ref int tmp = ref max;
            tmp = src.Length;
            idx = tmp - 1;
        }

        public bool IsAlive => idx > 0;
        public int Count => max;
        public T Value
        {
            get => src[idx];
            set => src[idx] = value;
        }

        public int Position
        {
            get => idx;
            set => idx = value;
        }

        public void Add() => idx--;

        public void Reset() => idx = max - 1;

        public bool LazyAdd() => --idx < max;

        public T[] ToArray()
        {
            int max = this.max;
            T[] temp = new T[max];

            for (int x = 0; x < max; x++)
                temp[x] = src[x];

            return temp;
        }

        public void Dispose()
        {
            src = null;
            max = 0;
            idx = 0;

            GC.SuppressFinalize(this);
        }
    }
}
