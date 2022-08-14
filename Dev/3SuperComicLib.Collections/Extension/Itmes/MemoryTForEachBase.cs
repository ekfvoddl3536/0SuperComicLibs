using System;

namespace SuperComicLib.Collections
{
    internal abstract class MemoryTForEachBase<T> : IDisposable
    {
        protected T[] arr;
        protected int start;
        protected int end;
        protected int idx;

        protected MemoryTForEachBase(T[] arr, int start, int end)
        {
            this.arr = arr;
            this.start = start;
            this.end = end;
        }

        void IDisposable.Dispose() { }
    }
}
