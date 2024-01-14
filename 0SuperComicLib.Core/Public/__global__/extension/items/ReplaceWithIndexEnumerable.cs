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
    internal readonly struct ReplaceWithIndexEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly T _replace;
        private readonly Func<T, long, bool> _predicateWithIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReplaceWithIndexEnumerable(IEnumerable<T> source, T replace, Func<T, long, bool> predicateWithIndex)
        {
            _source = source;
            _replace = replace;
            _predicateWithIndex = predicateWithIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new Enumerator(in this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly Func<T, long, bool> _predicateWithIndex;
            private readonly T _replace;
            private T V_Current;
            private long V_Index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in ReplaceWithIndexEnumerable<T> enumerable)
            {
                _enumerator = enumerable._source.GetEnumerator();
                _predicateWithIndex = enumerable._predicateWithIndex;

                _replace = enumerable._replace;

                V_Current = default;
                V_Index = -1;
            }

            public T Current => V_Current;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var e = _enumerator;
                if (!e.MoveNext())
                    return false;

                var v = e.Current;
                V_Current =
                    _predicateWithIndex.Invoke(v, V_Index)
                    ? _replace
                    : v;

                V_Index++;

                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
                V_Index = -1;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }
    }
}