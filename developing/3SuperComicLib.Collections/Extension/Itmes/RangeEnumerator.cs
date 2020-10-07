using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class RangeEnumerator<T> : IEnumerator<T>
    {
        private T[] arr;
        private int start;
        private int end;
        private int idx;

        public RangeEnumerator(T[] arr, int start, int end)
        {
            this.arr = arr;
            this.start = start;
            this.end = end;
            idx = start;
        }

        public T Current => arr[idx];
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            arr = null;
            start = 0;
            end = 0;
            idx = 0;

            GC.SuppressFinalize(this);
        }

        public bool MoveNext() => ++idx < end;

        public void Reset() => idx = start;
    }
}
