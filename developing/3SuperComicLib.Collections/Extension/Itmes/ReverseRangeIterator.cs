namespace SuperComicLib.Collections
{
    internal sealed class ReverseRangeIterator<T> : RangeArrayForEachBase<T>, IValueIterator<T>
    {
        public ReverseRangeIterator(T[] arr, int start, int end) : base(arr, start, end) => 
            idx = end - 1;

        public bool IsAlive => idx >= start;

        public int Count => end - start;

        public T Value
        {
            get => arr[idx];
            set => arr[idx] = value;
        }

        public void Add() => idx--;

        public bool LazyAdd() => --idx >= start;

        public void Reset() => idx = end - 1;

        public T[] ToArray() => arr;
    }
}
