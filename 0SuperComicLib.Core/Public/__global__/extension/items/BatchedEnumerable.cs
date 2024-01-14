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
    internal readonly struct BatchedEnumerable<T> : IEnumerable<T[]>
    {
        private readonly IEnumerable<T> _collection;
        private readonly ulong _batchSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchedEnumerable(IEnumerable<T> collection, int batchSize)
        {
            _collection = collection;
            _batchSize = (uint)batchSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T[]> GetEnumerator() => new Enumerator(in this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<T[]>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly T[] _buffer;
            private T[] _current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(in BatchedEnumerable<T> enumerable)
            {
                _enumerator = enumerable._collection.GetEnumerator();
                
                var buffer = new T[(int)enumerable._batchSize];
                _buffer = buffer;
                _current = buffer;
            }

            public T[] Current => _current;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var enumerator = _enumerator;

                T[] buffer = _buffer;
                
                int i = 0;
                for (; enumerator.MoveNext() && i < buffer.Length; ++i)
                    buffer[i] = enumerator.Current;

                if (i == 0)
                    return false;

                if (i != buffer.Length)
                {
                    var newBuffer = new T[i];

                    Array.Copy(buffer, newBuffer, i);

                    _current = newBuffer;
                }

                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _current = _buffer;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
                _current = null;
            }
        }
    }
}