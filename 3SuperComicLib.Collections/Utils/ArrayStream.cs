using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperComicLib.Collections
{
    public class ArrayStream<T> : IArrayStream<T>
    {
        protected T[] arr;
        protected int idx;

        #region constructor
        public ArrayStream(T[] source) : this(source, 0) { }

        public ArrayStream(T[] source, int begin)
        {
            arr = source ?? Array.Empty<T>();
            idx = begin;
        }

        public ArrayStream(ICollection<T> collection, int begin)
        {
            arr = collection?.ToArray() ?? Array.Empty<T>();
            idx = begin;
        }

        protected ArrayStream() { }
        #endregion

        public virtual bool EndOfStream => (uint)idx >= (uint)arr.Length;

        public virtual int Length => arr.Length;

        public virtual int Position
        {
            get => idx;
            set => idx = value;
        }

        public T Read()
        {
            if ((uint)idx < (uint)arr.Length)
            {
                int temp = idx++;
                return arr[temp];
            }
            else
                return default;
        }

        public Memory<T> Read(int count)
        {
            int idx = this.idx;
            int end = idx + count;

            if ((uint)idx < (uint)end && (uint)end <= (uint)arr.Length)
            {
                this.idx = end;
                return new Memory<T>(arr, idx, count);
            }

            return default;
        }

        public T Peek() => (uint)idx < (uint)arr.Length ? arr[idx] : default;

        public Memory<T> Peek(int count)
        {
            int idx = this.idx;
            int end = idx + count;

            return
                (uint)idx < (uint)end && (uint)end <= (uint)arr.Length
                ? new Memory<T>(arr, idx, count)
                : default;
        }

        public virtual void Reset() => idx = 0;

        public bool CanRead(int count) => idx <= arr.Length - count;

        public void Move() => idx++;

        public void Move(int count) => idx += count;

        #region disposing
        protected virtual void Dispose(bool disposing)
        {
            if (arr != null)
            {
                arr = null;
                idx = 0;
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
