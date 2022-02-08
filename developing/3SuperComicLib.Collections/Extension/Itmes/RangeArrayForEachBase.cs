using System;

namespace SuperComicLib.Collections
{
    internal abstract class RangeArrayForEachBase<T> : IDisposable
    {
        protected T[] arr;
        protected int start;
        protected int end;
        protected int idx;

        protected RangeArrayForEachBase(T[] arr, int start, int end)
        {
            this.arr = arr;
            this.start = start;
            this.end = end;
        }

        ~RangeArrayForEachBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                arr = null;
                start = 0;
                end = 0;
                idx = 0;
            }
        }
    }
}
