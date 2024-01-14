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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    internal readonly struct IndexWithEnumerable<T> : IEnumerable<(T, long)>
    {
        private readonly IEnumerable<T> _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IndexWithEnumerable(IEnumerable<T> source) => _source = source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<(T, long)> GetEnumerator() => new Enumerator(in this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<(T, long)>
        {
            private readonly IEnumerator<T> _enumerator;
            private long _index;

            public Enumerator(in IndexWithEnumerable<T> enumerable)
            {
                _enumerator = enumerable._source.GetEnumerator();
                _index = -1;
            }

            public (T, long) Current => (_enumerator.Current, _index);
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var result = _enumerator.MoveNext();
                
                if (result)
                    ++_index;

                return result;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _index = -1;
            }

            public void Dispose() => _enumerator.Dispose();
        }
    }
}