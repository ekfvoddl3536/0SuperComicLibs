using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperComicLib.Collections
{
    public class ArrayStream<T> : IArrayStream<T>
    {
        protected T[] arr;
        protected int idx;
        protected int len;

        #region constructor
        public ArrayStream(T[] source) : this(source, 0) { }

        public ArrayStream(T[] source, int begin)
        {
            arr = source ?? Array.Empty<T>();
            len = arr.Length;
            idx = begin;
        }

        public ArrayStream(ICollection<T> collection, int begin)
        {
            arr = collection?.ToArray() ?? Array.Empty<T>();
            len = arr.Length;
            idx = begin;
        }

        protected ArrayStream() { }
        #endregion

        public virtual bool EndOfStream => idx < 0 || idx >= len;

        public virtual int Length => len;

        public virtual int Position
        {
            get => idx;
            set => idx = value;
        }

        public T Read()
        {
            if (idx >= 0 && idx < len)
            {
                int temp = idx;
                idx++;
                return arr[temp];
            }
            else
                return default;
        }

        public IArrayStream<T> Read(int count)
        {
            int idx = this.idx;
            int end = idx + count;

            if (idx >= 0 && count >= 0 && end <= len)
            {
                this.idx = end;
                return new RangeArrayStream<T>(arr, idx, end);
            }
            return null;
        }

        public T Peek() => idx >= 0 && idx < len ? arr[idx] : default;

        public IArrayStream<T> Peek(int count)
        {
            int idx = this.idx;
            int end = idx + count;

            return
                idx >= 0 && count >= 0 && end <= len
                ? new RangeArrayStream<T>(arr, idx, end)
                : null;
        }

        public virtual void Reset() => idx = 0;

        public bool CanRead(int count) => idx <= len - count;

        public void Move() => idx++;

        public void Move(int count) => idx += count;

        #region disposing
        protected virtual void Dispose(bool disposing)
        {
            if (arr != null)
            {
                arr = null;
                idx = 0;
                len = 0;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
