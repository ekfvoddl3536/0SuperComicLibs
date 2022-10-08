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

using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class BarrieredQueue<T> : LookaheadQueue<T>
    {
        private Stack<int> barrier;

        public BarrieredQueue() : this (null)
        {
        }

        public BarrieredQueue(T[] source) : base(source) =>
            barrier = new Stack<int>(16);

        public void MarkPoint() => barrier.Push(size);

        public void UnmarkPoint()
        {
            if (barrier.Count > 0)
                barrier.Pop();
        }

        public void RemoveAuto()
        {
            if (barrier.Count > 0)
            {
                int ncnt = barrier.Pop();
                while (size > ncnt)
                    arr[--size] = default;
            }
            else
                Clear();
        }

        public override T[] ToArray()
        {
            loop:
            if (barrier.Count > 0)
            {
                int ncnt = size - barrier.Pop();
                if (ncnt <= 0)
                    goto loop;

                T[] vs = new T[ncnt];
                T[] arr = this.arr;

                int x, y;
                x = y = head;

                int len = arr.Length;
                for (int z = 0; z < ncnt;)
                    vs[z++] = arr[(x + y++) % len];

                return vs;
            }
            else
                return base.ToArray();
        }

        protected override void Dispose(bool disposing)
        {
            if (barrier != null)
            {
                barrier.Clear();
                barrier = null;
            }
            base.Dispose(disposing);
        }
    }
}