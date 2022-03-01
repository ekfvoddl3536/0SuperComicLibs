using System;

namespace SuperComicLib.Collections
{
    internal sealed class RangeIterator<T> : RangeArrayForEachBase<T>, IValueIterator<T>
    {
        public RangeIterator(T[] arr, int start, int end) : base(arr, start, end) => 
            idx = start;

        #region iterator
        public bool IsAlive => idx < end;

        public int Count => end - start;

        public ref T Value => ref arr[idx];

        public void Add() => idx++;

        public bool LazyAdd() => ++idx < end;

        public void Reset() => idx = start;

        public T[] ToArray() => arr;
        #endregion
    }
}
