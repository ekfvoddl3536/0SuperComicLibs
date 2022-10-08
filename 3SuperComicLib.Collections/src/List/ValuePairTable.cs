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
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerTypeProxy(typeof(ValuePairTable<,>))]
    [DebuggerDisplay("Count = {m_count}")]
    public class ValuePairTable<T1, T2> : IDisposable, IEnumerable<KeyValuePair<T1, T2>>, IEnumerablePair<T1, T2>
    {
        private T1[] t1s;
        private T2[] t2s;
        private int count;

        public ValuePairTable() : this(8) { }

        public ValuePairTable(int size) => IncreaseCapacity(0, size);

        public int Count => count;

        public void Add(T1 v1, T2 v2)
        {
            int idx = count;
            if (idx >= t1s.Length)
                IncreaseCapacity(idx, idx + 8);

            t1s[idx] = v1;
            t2s[idx] = v2;
        }

        private void IncreaseCapacity(int oldsize, int newsize)
        {
            T1[] old1 = t1s;
            T2[] old2 = t2s;

            T1[] new1 = new T1[newsize];
            T2[] new2 = new T2[newsize];

            while (--oldsize >= 0)
            {
                new1[oldsize] = old1[oldsize];
                new2[oldsize] = old2[oldsize];
            }

            t1s = new1;
            t2s = new2;
        }

        #region enumerable
        private RemoteEnumerator Create() => new RemoteEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Create();
        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => Create();
        public IEnumeratorPair<T1, T2> GetEnumeratorPair() => Create();
        #endregion

        public void Dispose()
        {
            if (t1s != null)
            {
                t1s = null;
                t2s = null;
                count = 0;
            }
            GC.SuppressFinalize(this);
        }


        #region enumerator
        private struct RemoteEnumerator : IEnumerator<KeyValuePair<T1, T2>>, IEnumeratorPair<T1, T2>
        {
            private ValuePairTable<T1, T2> table;
            private int idx;

            public RemoteEnumerator(ValuePairTable<T1, T2> inst)
            {
                table = inst;
                idx = -1;
            }

            public T1 Item1 => table.t1s[idx];
            public T2 Item2 => table.t2s[idx];

            public KeyValuePair<T1, T2> Current => new KeyValuePair<T1, T2>(Item1, Item2);
            object IEnumerator.Current => Current;

            public bool MoveNext() => ++idx < table.count;

            public void Reset() => idx = -1;

            public void Dispose()
            {
                table = null;
                idx = 0;
            }
        }
        #endregion
    }
}
