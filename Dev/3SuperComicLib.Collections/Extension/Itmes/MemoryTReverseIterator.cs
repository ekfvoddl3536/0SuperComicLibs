namespace SuperComicLib.Collections
{
    internal sealed class MemoryTReverseIterator<T> : MemoryTForEachBase<T>, IValueIterator<T>
    {
        public MemoryTReverseIterator(T[] arr, int start, int end) : base(arr, start, end) => 
            idx = end - 1;

        public bool IsAlive => idx >= start;

        public int Count => end - start;

        public ref T Value => ref arr[idx];

        public void Add() => idx--;

        public bool LazyAdd() => --idx >= start;

        public void Reset() => idx = end - 1;

        public T[] ToArray() => arr;
    }
}
