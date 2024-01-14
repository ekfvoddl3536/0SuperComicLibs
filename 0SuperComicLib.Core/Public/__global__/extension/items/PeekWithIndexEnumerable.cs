// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    internal readonly struct PeekWithIndexEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _collection;
        private readonly Action<T, long> _body;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PeekWithIndexEnumerable(IEnumerable<T> collection, Action<T, long> body)
        {
            _collection = collection;
            _body = body;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new Enumerator(in this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly Action<T, long> _body;
            private long _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in PeekWithIndexEnumerable<T> enumerable)
            {
                _enumerator = enumerable._collection.GetEnumerator();
                _body = enumerable._body;
                _index = 0;
            }

            public T Current => _enumerator.Current;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var e = _enumerator;

                bool result = e.MoveNext();

                if (result)
                {
                    _body.Invoke(e.Current, _index);
                    ++_index;
                }

                return result;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _index = 0;
            }

            public void Dispose() => _enumerator.Dispose();
        }
    }
}