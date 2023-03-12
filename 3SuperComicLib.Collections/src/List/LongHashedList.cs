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
    [DebuggerDisplay("Count = {m_count}")]
    public unsafe class LongHashedList<T> : ILongHashedList<T>
    {
        protected const long bitmask = 0x7FFF_FFFF_FFFF_FFFF;
        protected const int maxlen = 0x7FEF_FFFF;
        protected const int default_capacity = 4;

        private int[] buckets;
        private Slot[] slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;
        private uint m_version;

        public LongHashedList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            m_freeList = -1;

            IncreaseCapacity(capacity);
        }

        public LongHashedList(IEnumerable<T> collection) : this(default_capacity)
        {
            if (collection == null)
                return;

            foreach (T val in collection)
                if (val != null)
                    Add(val.GetHashCode(), val);
        }

        public LongHashedList() : this(default_capacity) { }

        #region property
        public T this[long hashcode]
        {
            get => Get(hashcode);
            set => Set(hashcode, value);
        }

        public int Count => m_count;
        public int Capacity => buckets.Length;
        #endregion

        #region methods
        public void Set(long hashcode, T newvalue)
        {
            for (int i = buckets[(int)((hashcode & bitmask) % buckets.Length)] - 1; i >= 0; i = slots[i].next)
            {
                Slot item = slots[i];
                if (item.hashCode == hashcode)
                {
                    item.value = newvalue;
                    m_version++;
                }
            }
        }

        public T Get(long hashcode)
        {
            for (int i = buckets[(int)((hashcode & bitmask) % buckets.Length)] - 1; i >= 0; i = slots[i].next)
            {
                Slot item = slots[i];
                if (item.hashCode == hashcode)
                    return item.value;
            }

            return default;
        }

        public bool TryGet(long hashcode, out T result)
        {
            for (int i = buckets[(int)((hashcode & bitmask) % buckets.Length)] - 1; i >= 0; i = slots[i].next)
            {
                Slot item = slots[i];
                if (item.hashCode == hashcode)
                {
                    result = item.value;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public bool TrySet(long hashcode, T newvalue)
        {
            for (int i = buckets[(int)((hashcode & bitmask) % buckets.Length)] - 1; i >= 0; i = slots[i].next)
            {
                Slot item = slots[i];
                if (item.hashCode == hashcode)
                {
                    item.value = newvalue;
                    return true;
                }
            }
            return false;
        }

        public bool Contains(long hashcode)
        {
            for (int i = buckets[(int)((hashcode & bitmask) % buckets.Length)] - 1; i >= 0; i = slots[i].next)
                if (slots[i].hashCode == hashcode)
                    return true;
            return false;
        }

        public void Add(long hashcode, T value)
        {
            if (value == null)
                return;

            long code = hashcode & bitmask;
            int bucket = (int)(code % buckets.Length);
            for (int i = buckets[bucket] - 1; i >= 0; i = slots[i].next)
                if (slots[i].hashCode == hashcode)
                    return;

            int index;
            if (m_freeList >= 0)
            {
                index = m_freeList;
                m_freeList = slots[index].next;
            }
            else
            {
                if (m_count == buckets.Length)
                {
                    IncreaseCapacity(buckets.Length + default_capacity);
                    bucket = (int)(code % buckets.Length);
                }
                index = m_lastIndex;
                m_lastIndex++;
            }

            ref int temp = ref buckets[bucket];

            Slot item = slots[index];
            item.hashCode = hashcode;
            item.value = value;
            item.next = temp - 1;

            temp = index + 1;

            m_count++;
            m_version++;
        }

        public void Remove(long hashcode)
        {
            int idx = (int)((hashcode & bitmask) % buckets.Length);
            int last = -1;
            for (int i = buckets[idx] - 1; i >= 0; last = i, i = slots[i].next)
                if (slots[i].hashCode == hashcode)
                {
                    if (last < 0)
                        buckets[idx] = slots[i].next + 1;
                    else
                        slots[last].next = slots[i].next;

                    Slot item = slots[i];
                    item.hashCode = -1;
                    item.value = default;
                    item.next = m_freeList;

                    m_count--;
                    m_version++;
                    if (m_count == 0)
                    {
                        m_lastIndex = 0;
                        m_freeList = -1;
                    }
                    else
                        m_freeList = i;

                    return;
                }
        }

        public void Clear()
        {
            for (int x = 0; x < m_count; x++)
            {
                buckets[x] = 0;
                slots[x].Clear();
            }

            m_count = 0;
            m_lastIndex = 0;

            m_freeList = -1;

            m_version = 0;
        }

        public T[] ToArray()
        {
            T[] vs = new T[m_count];

            for (int x = 0; x < m_lastIndex; x++)
                if (slots[x].hashCode >= 0)
                    vs[x] = slots[x].value;

            return vs;
        }
        #endregion

        #region capacity
        private void IncreaseCapacity(int newsize)
        {
            if (newsize > maxlen)
            {
                newsize = maxlen;
                if (newsize == m_count)
                    throw new InvalidOperationException("MAX_SIZE");
            }

            ref Slot[] slots = ref this.slots;

            Slot[] nslots = new Slot[newsize];
            int[] nbks = new int[newsize];

            int x = 0;
            for (; x < m_lastIndex; x++)
            {
                Slot item = slots[x];
                ref int val = ref nbks[(int)(item.hashCode % newsize)];
                item.next = val - 1;
                nslots[x] = item;
                val = x + 1;
            }
            for (; x < newsize; x++)
                nslots[x] = new Slot();

            slots = nslots;
            buckets = nbks;
        }
        #endregion

        #region interface_enumerable
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<long> Keys => new KeyEnumerator(this);

        public IEnumerable<T> Values => new ValueEnumerator(this);
        #endregion

        #region disposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (buckets != null)
            {
                for (int x = m_count; --x >= 0;)
                    slots[x].Clear();

                buckets = null;
                slots = null;

                m_count = 0;
                m_lastIndex = 0;
                m_freeList = 0;
                m_version = 0;
            }
        }
        #endregion

        #region item
        private sealed class Slot
        {
            public long hashCode;
            public int next = -1;
            public T value;

            public void Clear()
            {
                hashCode = 0;
                next = -1;
                value = default;
            }

#if DEBUG
            public override string ToString() => $"{hashCode} : {value} : {next}";
#endif
        }
        #endregion

        #region structs
        protected struct Enumerator : IEnumerator<T>
        {
            private LongHashedList<T> inst;
            private int index;
            private uint version;
            private T current;

            public Enumerator(LongHashedList<T> inst)
            {
                this.inst = inst;
                version = inst.m_version;
                index = 0;
                current = default;
            }

            public T Current => current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                inst = null;
                index = 0;
                version = 0;
                current = default;
            }

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                while (index < inst.m_lastIndex)
                {
                    Slot item = inst.slots[index];
                    if (item.hashCode >= 0)
                    {
                        current = item.value;
                        index++;
                        return true;
                    }
                    index++;
                }

                current = default;
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default;
            }
        }

        protected struct KeyEnumerator : IEnumerable<long>, IEnumerator<long>
        {
            private LongHashedList<T> inst;
            private int index;
            private uint version;
            private long current;

            public KeyEnumerator(LongHashedList<T> inst)
            {
                this.inst = inst;
                version = inst.m_version;
                index = 0;
                current = 0;
            }

            public IEnumerator<long> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public long Current => current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                inst = null;
                index = 0;
                version = 0;
            }

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                while (index < inst.m_count)
                {
                    Slot item = inst.slots[index];
                    if (item.hashCode >= 0)
                    {
                        current = item.hashCode;
                        index++;
                        return true;
                    }
                    index++;
                }

                Reset();
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = 0;
            }
        }

        protected struct ValueEnumerator : IEnumerable<T>, IEnumerator<T>
        {
            private LongHashedList<T> inst;
            private int index;
            private uint version;
            private T current;

            public ValueEnumerator(LongHashedList<T> inst)
            {
                this.inst = inst;
                version = inst.m_version;
                index = 0;
                current = default;
            }

            public IEnumerator<T> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public T Current => current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                inst = null;
                index = 0;
                version = 0;
                current = default;
            }

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                while (index < inst.m_count)
                {
                    Slot item = inst.slots[index];
                    if (item.hashCode >= 0)
                    {
                        current = item.value;
                        index++;
                        return true;
                    }
                    index++;
                }

                Reset();
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default;
            }
        }
        #endregion
    }
}
