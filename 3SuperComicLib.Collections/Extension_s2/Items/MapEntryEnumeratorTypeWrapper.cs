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
    internal sealed class MapEntryEnumeratorTypeWrapper<TKey, TValue> :
        IFailableEnumerator<Tuple<TKey, TValue>>,
        IFailableEnumerator<KeyValuePair<TKey, TValue>>
    {
        internal readonly IMapEntryEnumerator<TKey, TValue> _innerEnumerator;

        public MapEntryEnumeratorTypeWrapper(IMapEntryEnumerator<TKey, TValue> innerEnumerator) =>
            _innerEnumerator = innerEnumerator;

        public bool IsFailed => _innerEnumerator.IsFailed;

        object IEnumerator.Current => Current;
        KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
        {
            get
            {
                var current = _innerEnumerator.Current;
                return new KeyValuePair<TKey, TValue>(current.Item1, current.Item2);
            }
        }
        public Tuple<TKey, TValue> Current
        {
            get
            {
                var current = _innerEnumerator.Current;
                return new Tuple<TKey, TValue>(current.Item1, current.Item2);
            }
        }

        public bool MoveNext() => _innerEnumerator.MoveNext();
        public void Reset() => _innerEnumerator.Reset();
        public void Dispose() => _innerEnumerator.Dispose();
    }
}
