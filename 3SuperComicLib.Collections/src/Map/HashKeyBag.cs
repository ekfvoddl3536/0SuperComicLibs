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
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerTypeProxy(typeof(EnumerableView<int>))]
    [DebuggerDisplay("Count = {m_count}")]
    public class HashKeyBag<T> : IEnumerable<int>
    {
        private readonly int[] m_buckets;
        private int m_count;
        private uint m_version;

        public HashKeyBag(int size) => m_buckets = new int[Math.Max(4, size)];

        public int Count => m_count;

        public int Capacity => m_buckets.Length;

        public void Clear()
        {
            Array.Clear(m_buckets, 0, m_buckets.Length);
            m_count = 0;

            m_version++;
        }

        public bool TryAdd(T value)
        {
            int hashcode = value.GetHashCode();
            int key = hashcode % m_buckets.Length;

            if (m_buckets[key] != 0) // not empty
                return false;

            m_buckets[key] = hashcode;
            m_version++;
            m_count++;

            return true;
        }

        public bool Contains(T value) =>
            IndexOf(value) > 0;

        public int IndexOf(T value)
        {
            int key = value.GetHashCode() % m_buckets.Length;
            return
                m_buckets[key] != 0
                ? key
                : -1;
        }

        public int[] ArraySource() => m_buckets;

        #region enumerable
        public IEnumerator<int> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        private struct Enumerator : IEnumerator<int>
        {
            private HashKeyBag<T> inst;
            private int index;
            private readonly uint version;

            public Enumerator(HashKeyBag<T> inst)
            {
                this.inst = inst;
                version = inst.m_version;
                index = -1;
            }

            public int Current => inst.m_buckets[index];
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                HashKeyBag<T> inst = this.inst;
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                int[] b = inst.m_buckets;
                int len = b.Length;

                while (++index < len)
                    if (b[index] != 0)
                        return true;

                return false;
            }

            public void Reset() => index = -1;
            public void Dispose() => inst = null;
        }
    }
}
