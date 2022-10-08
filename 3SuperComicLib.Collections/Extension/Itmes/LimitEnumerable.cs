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
    internal sealed class LimitEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IEnumerator<T> collection;
        private int limit;
        private int x;

        public LimitEnumerable(IEnumerable<T> collection, int limit)
        {
            this.collection = collection.GetEnumerator();
            this.limit = limit;
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public T Current => collection.Current;
        object IEnumerator.Current => collection.Current;

        public bool MoveNext() => ++x < limit && collection.MoveNext();

        public void Reset()
        {
            collection.Reset();
            x = 0;
        }

        public void Dispose()
        {
            if (collection != null)
            {
                collection.Dispose();
                collection = null;

                limit = x = 0;
            }
            GC.SuppressFinalize(this);
        }
    }
}