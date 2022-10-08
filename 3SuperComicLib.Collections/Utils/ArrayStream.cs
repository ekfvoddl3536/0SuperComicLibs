// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
