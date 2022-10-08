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
    public class Map<T> : IMap<T>
    {
        protected const int bitmask = 0x7FFF_FFFF;
        protected const int maxlen = 0x7FEF_FFFF;
        protected const int default_capacity = 4;

        private int[] buckets;
        private Slot[] slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;
        private uint m_version;

        #region constructor
        public Map(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            m_freeList = -1;
            if (capacity == 0)
            {
                buckets = Array.Empty<int>();
                slots = Array.Empty<Slot>();
            }
            else
                IncreaseCapacity(capacity);
        }

        public Map(KeyValuePair<int, T>[] items)
        {
            if (items == null)
                return;

            m_freeList = -1;

            int max;
            if ((max = items.Length) == 0)
            {
                buckets = Array.Empty<int>();
                slots = Array.Empty<Slot>();
            }
            else
                IncreaseCapacity(max);

            for (int x = 0; x < max; x++)
            {
                KeyValuePair<int, T> now = items[x];
                Add(now.Key, now.Value);
            }
        }

        public Map(IEnumerable<T> collection) : this(default_capacity)
        {
            if (collection == null)
                return;

            foreach (T val in collection)
                if (val != null)
                    Add(val.GetHashCode(), val);
        }

        public Map() : this(default_capacity) { }
        #endregion

        #region property
        public int Count => m_count;
        public int Capacity => buckets.Length;

        public T this[int hashcode] => Get(hashcode);
        #endregion

        #region methods
        public bool SetEquals(IEnumerable<KeyValuePair<int, T>> other)
        {
            foreach (KeyValuePair<int, T> kv in other)
                if (!Contains(kv.Key))
                    return false;

            return true;
        }

        public bool Contains(int hashcode)
        {
            hashcode &= bitmask;
            for (int i = buckets[hashcode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                if (slots[i].hashCode == hashcode)
                    return true;
            return false;
        }

        public bool TryGet(int hashcode, out T result)
        {
            hashcode &= bitmask;
            for (int i = buckets[hashcode % buckets.Length] - 1; i >= 0; i = slots[i].next)
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

        public T Get(int hashcode)
        {
            hashcode &= bitmask;
            for (int i = buckets[hashcode % buckets.Length] - 1; i >= 0; i = slots[i].next)
            {
                Slot item = slots[i];
                if (item.hashCode == hashcode)
                    return item.value;
            }

            return default;
        }

        public void Add(int hashcode, T value)
        {
            if (value == null)
                return;

            hashcode &= bitmask;
            int bucket = hashcode % buckets.Length;
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
                    bucket = hashcode % buckets.Length;
                }
                index = m_lastIndex++;
            }

            ref int temp = ref buckets[bucket];

            Slot item = slots[index++];
            item.hashCode = hashcode;
            item.value = value;
            item.next = temp - 1;

            temp = index;

            m_count++;
            m_version++;
        }

        public void Remove(int hashcode)
        {
            hashcode &= bitmask;
            int idx = hashcode % buckets.Length;
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
            for (int x = 0; x < buckets.Length; x++)
            {
                buckets[x] = 0;
                slots[x].Clear();
            }

            m_count = 0;
            m_lastIndex = 0;

            m_freeList = -1;

            m_version = 0;
        }

        public KeyValuePair<int, T>[] ToArray()
        {
            int x = m_lastIndex;
            KeyValuePair<int, T>[] result = new KeyValuePair<int, T>[x];

            Slot[] slots = this.slots;
            while (--x >= 0)
                if (slots[x].hashCode >= 0)
                {
                    Slot item = slots[x];
                    result[x] = new KeyValuePair<int, T>(item.hashCode, item.value);
                }

            return result;
        }

        public T[] ToValueArray()
        {
            int x = m_lastIndex;
            T[] result = new T[x];

            Slot[] slots = this.slots;
            while (--x >= 0)
                if (slots[x].hashCode >= 0)
                    result[x] = slots[x].value;

            return result;
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
            for (; x < m_lastIndex;)
            {
                Slot item = slots[x];
                ref int val = ref nbks[item.hashCode % newsize];
                item.next = val - 1;
                nslots[x++] = item;
                val = x;
            }
            while (x < newsize)
                nslots[x++] = new Slot();

            slots = nslots;
            buckets = nbks;
        }
        #endregion

        #region interface_enumerable
        public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<int> Keys => new KeyEnumerator(this);

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
            public int hashCode;
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
        protected struct Enumerator : IEnumerator<KeyValuePair<int, T>>
        {
            private Map<T> inst;
            private int index;
            private uint version;
            private KeyValuePair<int, T> current;

            public Enumerator(Map<T> inst)
            {
                this.inst = inst;
                version = inst.m_version;
                index = 0;
                current = default;
            }

            public KeyValuePair<int, T> Current => current;
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

                ref int index = ref this.index;
                while (index < inst.m_lastIndex)
                    if (inst.slots[index].hashCode >= 0)
                    {
                        Slot item = inst.slots[index++];
                        current = new KeyValuePair<int, T>(item.hashCode, item.value);
                        return true;
                    }

                index = inst.m_lastIndex + 1;
                current = default;
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default;
            }
        }

        protected struct KeyEnumerator : IEnumerable<int>, IEnumerator<int>
        {
            private Map<T> inst;
            private int index;
            private uint version;
            private int current;

            public KeyEnumerator(Map<T> inst)
            {
                this.inst = inst;
                index = 0;
                version = inst.m_version;
                current = -1;
            }

            public IEnumerator<int> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int Current => current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                inst = null;
                index = 0;
                version = 0;
                current = 0;
            }

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                ref int index = ref this.index;
                while (index < inst.m_lastIndex)
                    if (inst.slots[index].hashCode >= 0)
                    {
                        current = inst.slots[index++].hashCode;
                        return true;
                    }

                index = inst.m_lastIndex + 1;
                current = -1;
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = -1;
            }
        }

        protected struct ValueEnumerator : IEnumerable<T>, IEnumerator<T>
        {
            private Map<T> inst;
            private int index;
            private uint version;
            private T current;

            public ValueEnumerator(Map<T> inst)
            {
                this.inst = inst;
                index = 0;
                version = inst.m_version;
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

                ref int index = ref this.index;
                while (index < inst.m_lastIndex)
                    if (inst.slots[index].hashCode >= 0)
                    {
                        current = inst.slots[index++].value;
                        return true;
                    }

                index = inst.m_lastIndex + 1;
                current = default;
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
