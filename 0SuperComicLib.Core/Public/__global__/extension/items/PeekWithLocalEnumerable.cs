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
    internal readonly struct PeekWithLocalEnumerable<TSource, TLocal> : IEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _collection;
        private readonly Func<TLocal> _localinit;
        private readonly Func<TSource, TLocal, TLocal> _body;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PeekWithLocalEnumerable(IEnumerable<TSource> collection, Func<TLocal> localinit, Func<TSource, TLocal, TLocal> body)
        {
            _collection = collection;
            _localinit = localinit;
            _body = body;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<TSource> GetEnumerator() => new Enumerator(in this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<TSource>
        {
            private readonly IEnumerator<TSource> _enumerator;
            private readonly Func<TSource, TLocal, TLocal> _body;
            private TLocal _local;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in PeekWithLocalEnumerable<TSource, TLocal> enumerable)
            {
                _enumerator = enumerable._collection.GetEnumerator();
                _body = enumerable._body;
                _local = enumerable._localinit.Invoke();
            }

            public TSource Current => _enumerator.Current;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var e = _enumerator;

                bool result = e.MoveNext();

                if (result)
                    _local = _body.Invoke(e.Current, _local);

                return result;
            }

            public void Reset() => _enumerator.Reset();

            public void Dispose() => _enumerator.Dispose();
        }
    }
}