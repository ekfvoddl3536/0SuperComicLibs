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
    public class ArrayReverseEnumerator<T> : IEnumerator<T>
    {
        public readonly Memory<T> Source;
        protected int index;

        public ArrayReverseEnumerator(Memory<T> memory)
        {
            Source = memory;
            index = memory._start + memory.Length;
        }

        public ArrayReverseEnumerator(T[] array, int start, int count)
        {
            Source = new Memory<T>(array, start, count);
            index = start + count;
        }

        public T Current => Source._source[index];
        object IEnumerator.Current => Current;

        public virtual bool MoveNext() => --index >= Source._start;
        public virtual void Reset() => index = Source._start + Source.Length;

        void IDisposable.Dispose() => GC.SuppressFinalize(this);
    }
}
