using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class MemoryTEnumerator<T> : MemoryTForEachBase<T>, IEnumerator<T>
    {
        public MemoryTEnumerator(T[] arr, int start, int end) : base(arr, start, end) =>
            idx = start - 1;

        public T Current => arr[idx];
        object IEnumerator.Current => Current;

        public bool MoveNext() => ++idx < end;

        public void Reset() => idx = start - 1;
    }
}
