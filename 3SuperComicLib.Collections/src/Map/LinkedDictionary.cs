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
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Collections
{
    [DebuggerDisplay("Count = {m_count}")]
    public class LinkedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        protected const int bitmask = 0x7FFF_FFFF;
        protected const int maxlen = 0x7FEF_FFFF;
        protected const int default_capacity = 4;

        private int[] buckets;
        private Slot[] slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;
        private IEqualityComparer<TKey> m_comparer;
        private uint m_version;
        private LinkedNode<TTKv> m_head;

        #region constructor
        public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            m_freeList = -1;
            m_comparer = comparer;

            if (collection is ICollection<KeyValuePair<TKey, TValue>> item)
                IncreaseCapacity(item.Count.Max(default_capacity));
            else
                IncreaseCapacity(default_capacity);

            foreach (KeyValuePair<TKey, TValue> now in collection)
                Add(now.Key, now.Value);
        }

        public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, EqualityComparer<TKey>.Default) { }

        public LinkedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            m_freeList = -1;
            m_comparer = comparer;

            IncreaseCapacity(capacity.Max(default_capacity));
        }

        public LinkedDictionary(int capacity) : this(capacity, EqualityComparer<TKey>.Default) { }

        public LinkedDictionary() : this(default_capacity, EqualityComparer<TKey>.Default) { }
        #endregion

        #region property
        public int Count => m_count;

        public int Capacity => buckets.Length;

        public bool IsReadOnly => false;

        public ValueCollection Values => new ValueCollection(this);

        public KeyCollection Keys => new KeyCollection(this);

        public IEqualityComparer<TKey> Comparer
        {
            get => m_comparer;
            set => m_comparer = value ?? EqualityComparer<TKey>.Default;
        }

        public TValue this[TKey key]
        {
            get
            {
                var slots = this.slots;
                int hc = key.GetHashCode() & bitmask;
                for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                    if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value.key, key))
                        return slots[x].node.m_value.value;

                throw new KeyNotFoundException();
            }
            set
            {
                var slots = this.slots;
                int hc = key.GetHashCode() & bitmask;
                for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                    if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value.key, key))
                        slots[x].node.m_value.value = value;

                throw new KeyNotFoundException();
            }
        }
        #endregion

        #region methods
        public KeyValuePair<TKey, TValue> First()
        {
            FastContract.Requires<IndexOutOfRangeException>(m_head != null);

            ref TTKv temp = ref m_head.m_value;
            return new KeyValuePair<TKey, TValue>(temp.key, temp.value);
        }

        public KeyValuePair<TKey, TValue> Last()
        {
            FastContract.Requires<IndexOutOfRangeException>(m_head != null);

            ref TTKv temp = ref m_head.m_prev.m_value;
            return new KeyValuePair<TKey, TValue>(temp.key, temp.value);
        }

        public bool ContainsKey(TKey key)
        {
            int hc = key.GetHashCode() & bitmask;
            for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value.key, key))
                    return true;

            return false;
        }

        public bool ContainsValue(TValue value)
        {
            LinkedNode<TTKv> head = m_head;

            if (head != null)
            {
                LinkedNode<TTKv> curr = head;
                IEqualityComparer<TValue> tcomp = EqualityComparer<TValue>.Default;
                do
                {
                    if (curr != null && tcomp.Equals(curr.m_value.value, value))
                        return true;

                    curr = curr.m_next;
                }
                while (head != curr);
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int hc = key.GetHashCode() & bitmask;
            var slots = this.slots;

            for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value.key, key))
                {
                    value = slots[x].node.m_value.value;
                    return true;
                }

            value = default;
            return false;
        }

        public bool Add(TKey key, TValue value)
        {
            int hc = key.GetHashCode() & bitmask;
            int bucket = hc % buckets.Length;
            for (int x = buckets[bucket] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value.key, key))
                    return false;

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
                    bucket = hc % buckets.Length;
                }
                index = m_lastIndex++;
            }

            ref int temp = ref buckets[bucket];

            Slot slot = slots[index++];
            slot.hashCode = hc;
            slot.next = temp - 1;

            temp = index;

            LinkedNode<TTKv> node = slot.node;
            node.m_value = new TTKv(key, value);
            if (m_head == null)
            {
                node.m_next = node;
                node.m_prev = node;
                m_head = node;
            }
            else
            {
                LinkedNode<TTKv> head = m_head;
                node.m_next = head;
                node.m_prev = head.m_prev;
                head.m_prev.m_next = node;
                head.m_prev = node;
            }

            m_count++;
            m_version++;

            return true;
        }

        public bool Remove(TKey key)
        {
            int hc = key.GetHashCode() & bitmask;
            int bucket = hc % buckets.Length;
            int last = -1;
            for (int x = buckets[bucket] - 1; x >= 0; last = x, x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value.key, key))
                {
                    Slot s = slots[x];
                    if (last < 0)
                        buckets[bucket] = s.next + 1;
                    else
                        slots[last].next = s.next;

                    s.hashCode = -1;
                    s.next = m_freeList;
                    s.node.Disconnect();

                    m_count--;
                    m_version++;
                    if (m_count == 0)
                    {
                        m_lastIndex = 0;
                        m_freeList = -1;
                    }
                    else
                        m_freeList = x;

                    return true;
                }

            return false;
        }

        public void Clear()
        {
            var buckets = this.buckets;
            var slots = this.slots;

            for (int x = 0; x < buckets.Length; x++)
            {
                buckets[x] = 0;
                slots[x].Clear();
            }

            m_count = 0;
            m_lastIndex = 0;

            m_freeList = -1;

            m_version++;
            m_head = null;
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            int x = m_count;
            if (x <= 0)
                return Array.Empty<KeyValuePair<TKey, TValue>>();

            KeyValuePair<TKey, TValue>[] vs = new KeyValuePair<TKey, TValue>[x];
            LinkedNode<TTKv> last = m_head.m_prev;
            while (--x >= 0)
            {
                TTKv kv = last.m_value;
                vs[x] = new KeyValuePair<TKey, TValue>(kv.key, kv.value);
                last = last.m_prev;
            }

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

#region item
        internal sealed class Slot
        {
            public int hashCode;
            public int next = -1;
            public LinkedNode<TTKv> node = new LinkedNode<TTKv>();

            public void Clear()
            {
                hashCode = 0;
                next = -1;
                node.Disconnect();
            }
        }

        internal struct TTKv
        {
            public TKey key;
            public TValue value;
        
            public TTKv(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }
#endregion

#region enumerable
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new KeyValueEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endregion

#region enumerators
        protected abstract class UnivEnumerator<UT> : IEnumerator<UT>
        {
            private LinkedDictionary<TKey, TValue> inst;
            private LinkedNode<TTKv> node;
            private UT value;
            private uint version;

            protected UnivEnumerator(LinkedDictionary<TKey, TValue> inst)
            {
                this.inst = inst;
                node = inst.m_head;
                version = inst.m_version;
                value = default;
            }

            public UT Current => value;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                if (node == null)
                    return false;

                TTKv temp = node.m_value;
                value = Convert(temp.key, temp.value);

                node = node.Next;
                if (node == inst.m_head)
                    node = null;

                return true;
            }

            protected abstract UT Convert(TKey key, TValue value);

            public void Reset()
            {
                node = inst.m_head;
                value = default;
            }

            public void Dispose()
            {
                if (inst != null)
                {
                    inst = null;
                    node = null;
                    version = 0;
                }
                GC.SuppressFinalize(this);
            }
        }

        protected sealed class ValueEnumerator : UnivEnumerator<TValue>
        {
            public ValueEnumerator(LinkedDictionary<TKey, TValue> inst) : base(inst)
            {
            }

            protected override TValue Convert(TKey key, TValue value) =>
                value;
        }

        protected sealed class KeyEnumerator : UnivEnumerator<TKey>
        {
            public KeyEnumerator(LinkedDictionary<TKey, TValue> inst) : base(inst)
            {
            }

            protected override TKey Convert(TKey key, TValue value) =>
                key;
        }

        protected sealed class KeyValueEnumerator : UnivEnumerator<KeyValuePair<TKey, TValue>>
        {
            public KeyValueEnumerator(LinkedDictionary<TKey, TValue> inst) : base(inst)
            {
            }

            protected override KeyValuePair<TKey, TValue> Convert(TKey key, TValue value) =>
                new KeyValuePair<TKey, TValue>(key, value);
        }
#endregion

#region collections
        public sealed class KeyCollection : IEnumerable<TKey>
        {
            private readonly LinkedDictionary<TKey, TValue> inst;

            public KeyCollection(LinkedDictionary<TKey, TValue> inst) =>
                this.inst = inst;

            public IEnumerator<TKey> GetEnumerator() => new KeyEnumerator(inst);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public sealed class ValueCollection : IEnumerable<TValue>
        {
            private readonly LinkedDictionary<TKey, TValue> inst;

            public ValueCollection(LinkedDictionary<TKey, TValue> inst) =>
                this.inst = inst;

            public IEnumerator<TValue> GetEnumerator() => new ValueEnumerator(inst);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
#endregion
    }
}
