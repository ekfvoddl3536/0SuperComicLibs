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

namespace SuperComicLib.Collections
{
    public class BarrieredStack<T> : LookaheadStack<T>
    {
        protected Stack<int> barrier;

        public BarrieredStack() : this(null)
        {
        }

        public BarrieredStack(T[] source) : base(source) => 
            barrier = new Stack<int>();

        public BarrieredStack(int initalCapacity) : base(initalCapacity) =>
            barrier = new Stack<int>();

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

        public virtual T[] PopAuto()
        {
            T[] vs, arr = this.arr;
            int x, ncnt;
        loop:
            if (barrier.Count > 0)
            {
                ncnt = size - barrier.Pop();
                if (ncnt == 0)
                    return Array.Empty<T>();
                else if (ncnt < 0)
                    goto loop;

                vs = new T[ncnt];
                x = size - ncnt;
                while (--ncnt >= 0)
                    vs[ncnt] = arr[x++];
            }
            else
            {
                ncnt = size;
                vs = new T[ncnt];

                for (x = 0; x < ncnt;)
                    vs[x++] = Pop();
            }

            return vs;
        }
    }
}