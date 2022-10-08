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

using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed unsafe class RawIteratorEnumerator<T> : IEnumerator<T> 
        where T : unmanaged
    {
        private readonly T* start;
        private readonly T* end;
        private T* current;
        private T item;

        public RawIteratorEnumerator(IRawContainer<T> container)
        {
            current = start = container.begin()._ptr;
            end = container.end()._ptr;
        }

        public T Current => item;
        object IEnumerator.Current => item;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (current != end)
            {
                item = *current++;
                return true;
            }

            return false;
        }

        public void Reset() => current = start;
    }
}
