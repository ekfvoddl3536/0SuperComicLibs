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

using SuperComicLib.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SuperComicLib.Collections.Concurrent
{
    public class ConcurrentSequentialBag<T> : IEnumerable<T>
    {
        private Item[] m_items;
        private SpinCountBarrierLockSlim m_barrierLock;
        private VolatileInt32 m_lastIndex;
        private volatile int m_vCount;
        private volatile int m_version;
        private readonly int m_expandSize;

        public ConcurrentSequentialBag() : this(Environment.ProcessorCount)
        {
        }

        public ConcurrentSequentialBag(int workers)
        {
            int size = workers << 4;
            m_items = new Item[m_expandSize = size];
        }

        public int Count
        {
            get
            {
                int count = m_vCount;
                return count;
            }
        }

        /// <summary>
        /// Trying add item
        /// </summary>
        /// <param name="gIndex">0-based index number</param>
        /// <param name="item">value to be added</param>
        /// <exception cref="Exception">All critical exceptions (rethrow)</exception>
        /// <returns>return true if added</returns>
        public bool TryAdd(int gIndex, T item)
        {
            if (gIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(gIndex));

            Item[] items = m_items;
            int count = m_vCount;

            ref SpinCountBarrierLockSlim block = ref m_barrierLock;

            if (Math.Max(count, gIndex) >= items.Length)
            {
                block.Blocking();
                try
                {
                    IncreaseCapacity(Math.Max(count, gIndex));
                    block.ReleaseAndEnter();
                }
                catch (Exception criticalExc)
                {
                    block.Release();
                    throw criticalExc;
                }
            }
            else
                block.Enter();

            try
            {
                items = m_items; // re-cache
                if (Interlocked.CompareExchange(ref items[gIndex], new Item(item), null) != null)
                {
                    block.Exit();
                    return false;
                }

                Interlocked.Increment(ref m_version);
                Interlocked.Increment(ref m_vCount);
                m_lastIndex.Max(gIndex);

                block.Exit();
                return true;
            }
            catch
            {
                block.Exit();
                return false;
            }
        }

        public bool TryTake(out T item)
        {
            ref SpinCountBarrierLockSlim block = ref m_barrierLock;
            block.Blocking();

            int count = m_vCount;
            if (count <= 0 ||
                m_lastIndex.Value != count)
            {
                block.Release();

                item = default;
                return false;
            }

            ref Item sample = ref m_items[--m_vCount];
            item = sample.m_value;
            sample = null;

            m_lastIndex = new VolatileInt32(m_lastIndex.Value - 1);
            m_version++;

            block.Release();
            return true;
        }

        public T[] ToArray()
        {
            ref SpinCountBarrierLockSlim block = ref m_barrierLock;
            block.Blocking();

            try
            {
                Item[] items = m_items;
                int n = m_vCount;

                T[] result = new T[n];

                while (--n >= 0)
                    result[n] = items[n].m_value;

                block.Release();
                return result;
            }
            catch (Exception criticalExc)
            {
                block.Release();
                throw criticalExc;
            }
        }

        public void Clear()
        {
            ref SpinCountBarrierLockSlim block = ref m_barrierLock;
            block.Blocking();

            try
            {
                Array.Clear(m_items, 0, m_lastIndex.Value);

                m_vCount = 0;

                m_lastIndex = default;
                m_version++;

                block.Release();
            }
            catch (Exception criticalExc)
            {
                block.Release();
                throw criticalExc;
            }
        }

        private void IncreaseCapacity(int p_size)
        {
            int n = p_size + m_expandSize;

            Item[] oldArr = m_items;
            Item[] newArr = new Item[n];

            for (int i = oldArr.Length; --i >= 0;)
                newArr[i] = oldArr[i];

            m_items = newArr;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class Item
        {
            public readonly T m_value;

            public Item(T value) =>
                m_value = value;

            public static T ToValue(Item item) => item.m_value;
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private ConcurrentSequentialBag<T> inst;
            private int version;
            private int index;
            private T value;

            public Enumerator(ConcurrentSequentialBag<T> inst)
            {
                this.inst = inst;
                version = inst.m_version;
                index = -1;
            }

            public T Current => value;
            object IEnumerator.Current => value;

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                int idx = index + 1;
                if (idx < inst.m_vCount)
                {
                    value = inst.m_items[index = idx].m_value;
                    return true;
                }
                else
                    return false;
            }

            public void Reset()
            {
                index = -1;
                value = default;
            }

            public void Dispose()
            {
                inst = null;
                version = 0;
                index = 0;
                value = default;

                GC.SuppressFinalize(this);
            }
        }
    }
}
