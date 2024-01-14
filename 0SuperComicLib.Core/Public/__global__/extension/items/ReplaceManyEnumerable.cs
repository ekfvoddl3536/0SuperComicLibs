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
    internal readonly struct ReplaceManyEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly IEnumerable<T> _replace;
        private readonly Predicate<T> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReplaceManyEnumerable(IEnumerable<T> source, IEnumerable<T> replace, Predicate<T> predicate)
        {
            _source = source;
            _replace = replace;
            _predicate = predicate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new Enumerator(in this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly Predicate<T> _predicate;
            private readonly IEnumerable<T> _replace;
            private IEnumerator<T> E_replace;
            private T V_Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in ReplaceManyEnumerable<T> enumerable)
            {
                _enumerator = enumerable._source.GetEnumerator();
                _predicate = enumerable._predicate;
                
                _replace = enumerable._replace;
                
                E_replace = null;

                V_Current = default;
            }

            public T Current => V_Current;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                for (; ; )
                {
                    if (E_replace != null)
                        if (E_replace.MoveNext())
                        {
                            V_Current = E_replace.Current;
                            return true;
                        }
                        else
                        {
                            E_replace.Dispose();
                            E_replace = null;
                        }

                    var e = _enumerator;
                    if (!e.MoveNext())
                        return false;

                    var v = e.Current;
                    if (_predicate.Invoke(v))
                    {
                        E_replace = _replace.GetEnumerator();
                        continue;
                    }
                    else
                    {
                        V_Current = v;
                        return true;
                    }
                }
            }

            public void Reset()
            {
                _enumerator.Reset();
                
                SetNull();
            }

            public void Dispose()
            {
                _enumerator.Dispose();

                SetNull();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void SetNull()
            {
                E_replace?.Dispose();
                E_replace = null;
            }
        }
    }
}