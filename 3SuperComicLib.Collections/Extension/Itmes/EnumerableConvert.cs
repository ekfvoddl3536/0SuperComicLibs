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
    internal sealed class EnumerableConvert<TIn, TOut> : IEnumerable<TOut>, IEnumerator<TOut>
    {
        private IEnumerator<TIn> collection;
        private Converter<TIn, TOut> converter;
        private TOut current;

        public EnumerableConvert(IEnumerable<TIn> collection, Converter<TIn, TOut> converter)
        {
            this.collection = collection.GetEnumerator();
            this.converter = converter;
        }

        public IEnumerator<TOut> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public TOut Current => current;

        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (collection.MoveNext())
            {
                current = converter.Invoke(collection.Current);
                return true;
            }

            return false;
        }

        public void Reset() => collection.Reset();

        public void Dispose()
        {
            collection.Dispose();
            collection = null;

            converter = null;

            current = default;

            GC.SuppressFinalize(this);
        }
    }
}
