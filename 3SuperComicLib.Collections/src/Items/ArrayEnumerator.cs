// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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
    public class ArrayEnumerator<T> : IEnumerator<T>
    {
        protected readonly T[] array;
        protected readonly int start;
        protected readonly int end;
        protected int index;

        public ArrayEnumerator(Memory<T> memory)
        {
            array = memory._source;
            start = memory._start;
            end = memory._start + memory.Length;
            index = memory._start - 1;
        }

        public ArrayEnumerator(T[] array, int start, int count)
        {
            this.array = array;
            this.start = start;
            end = start + count;
            index = start - 1;
        }

        public Memory<T> Source => new Memory<T>(array, start, end - start);

        public T Current => array[index];
        object IEnumerator.Current => Current;

        public virtual bool MoveNext() => ++index < end;
        public virtual void Reset() => index = start - 1;

        void IDisposable.Dispose() => GC.SuppressFinalize(this);
    }
}
