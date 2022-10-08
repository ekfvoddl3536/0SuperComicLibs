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
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    [System.Diagnostics.DebuggerDisplay("Count = {size}")]
    public sealed class FixedStack<T> : IEnumerable<T>, IStack<T>
    {
        private T[] arr;
        private int cnt;

        public FixedStack(int initsize) => arr = new T[initsize];

        public T[] Source => arr;

        public int Count => cnt;

        public int Capacity => arr.Length;

        public void Push(T item) => arr[cnt++] = item;

        public T Peek() => arr[cnt - 1];

        public T Pop()
        {
            ref T now = ref arr[--cnt];

            T result = now;
            now = default;

            return result;
        }

        public T FastPop() => arr[--cnt];

        public T[] ToArray()
        {
            int length = arr.Length;
            T[] result = new T[length];

            Array.Copy(arr, result, length);

            return result;
        }

        public void Clear()
        {
            int x = cnt;
            while (--x >= 0)
                arr[x] = default;

            cnt = 0;
        }

        public IEnumerator<T> GetEnumerator() => arr.Slice(0, cnt).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            if (arr != null)
            {
                Clear();
                arr = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}