using System;

namespace SuperComicLib.Collections
{
    internal sealed class Array_Iterator<T> : IIterator<T>
    {
        private T[] src;
        private int max; // fast
        private int idx;

        public Array_Iterator(T[] src)
        {
            this.src = src;
            max = src.Length;
        }

        public bool IsAlive => idx < max;
        public int Count => max;
        public T Value
        {
            get => src[idx];
            set => src[idx] = value;
        }

        public void Add() => idx++;

        public void Reset() => idx = 0;

        public bool LazyAdd() => ++idx < max;

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
