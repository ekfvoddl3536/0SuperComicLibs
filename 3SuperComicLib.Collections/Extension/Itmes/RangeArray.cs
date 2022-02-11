using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class RangeArray<T> : IRangeRefArray<T>
    {
        private readonly T[] arr;
        private readonly int start;
        private readonly int end;

        public RangeArray(T[] arr, int start, int end)
        {
            this.arr = arr;
            this.start = start;
            this.end = end;
        }

        public T this[int index]
        {
            get => arr[index + start];
            set => arr[index + start] = value;
        }

        public int Length => end - start;

        public IValueIterator<T> Begin() =>
            new RangeIterator<T>(arr, start, end);

        public IValueIterator<T> RBegin() =>
            new ReverseRangeIterator<T>(arr, start, end);

        public IEnumerator<T> GetEnumerator() =>
            new RangeArrayEnumerator<T>(arr, start, end);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IRangeRefArray<T> Slice(int begin, int count)
        {
            int ed = begin + count - 1;
            return 
                begin >= 0 && ed <= end 
                ? new RangeArray<T>(arr, begin + start, ed) 
                : null;
        }

        public T[] Source() => arr;

        T[] IValueIterable<T>.ToArray() => Source();
    }
}
