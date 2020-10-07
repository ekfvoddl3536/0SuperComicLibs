using System;

namespace SuperComicLib.Collections
{
    internal sealed class RangeIterator<T> : IForwardIterator<T>
    {
        private T[] arr;
        private int start;
        private int end;
        private int idx;

        public RangeIterator(T[] arr, int start, int end)
        {
            this.arr = arr;
            this.start = start;
            this.end = end;
            idx = start;
        }

        public bool IsAlive => idx < end;
        public T Value
        {
            get => arr[idx];
            set => arr[idx] = value;
        }
        public int Count => end - start;

        public void Add() => idx++;

        public void Dispose()
        {
            arr = null;
            start = 0;
            end = 0;
            idx = 0;

            GC.SuppressFinalize(this);
        }

        public bool LazyAdd() => ++idx < end;

        public void Reset() => idx = start;

        public T[] ToArray() => arr;
    }
}
