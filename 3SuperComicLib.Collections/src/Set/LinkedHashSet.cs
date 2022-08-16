using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    [DebuggerDisplay("Count = {m_count}")]
    public class LinkedHashSet<T> : ISet<T>, IEnumerable<T>
    {
        protected const int bitmask = 0x7FFF_FFFF;
        protected const int maxlen = 0x7FEF_FFFF;
        protected const int default_capacity = 4;

        private int[] buckets;
        private Slot[] slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;
        private IEqualityComparer<T> m_comparer;
        private uint m_version;
        private LinkedNode<T> m_head;

        #region constructor
        public LinkedHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            m_freeList = -1;
            m_comparer = comparer;

            if (collection is ICollection<T> item)
                IncreaseCapacity(item.Count.Max(default_capacity));
            else
                IncreaseCapacity(default_capacity);

            foreach (T now in collection)
                Add(now);
        }

        public LinkedHashSet(IEnumerable<T> collection) : this(collection, EqualityComparer<T>.Default) { }

        public LinkedHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            m_freeList = -1;
            m_comparer = comparer;

            IncreaseCapacity(capacity.Max(default_capacity));
        }

        public LinkedHashSet(int capacity) : this(capacity, EqualityComparer<T>.Default) { }

        public LinkedHashSet() : this(default_capacity, EqualityComparer<T>.Default) { }
        #endregion

        #region property
        public int Count => m_count;

        public int Capacity => buckets.Length;

        public LinkedNode<T> First=> m_head;

        public LinkedNode<T> Last => m_head?.m_prev;

        public bool IsReadOnly => false;

        public IEqualityComparer<T> Comparer
        {
            get => m_comparer;
            set => m_comparer = value ?? EqualityComparer<T>.Default;
        }
        #endregion

        #region methods
        public bool Contains(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value, item))
                    return true;

            return false;
        }

        public bool Add(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            int bucket = hc % buckets.Length;
            for (int x = buckets[bucket] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value, item))
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

            LinkedNode<T> node = slot.node;
            node.m_value = item;
            if (m_head == null)
            {
                node.m_next = node;
                node.m_prev = node;
                m_head = node;
            }
            else
            {
                LinkedNode<T> head = m_head;
                node.m_next = head;
                node.m_prev = head.m_prev;
                head.m_prev.m_next = node;
                head.m_prev = node;
            }

            m_count++;
            m_version++;

            return true;
        }

        public bool Remove(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            int bucket = hc % buckets.Length;
            int last = -1;
            for (int x = buckets[bucket] - 1; x >= 0; last = x, x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value, item))
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

            m_version = 0;
            m_head = null;
        }

        public T[] ToArray()
        {
            int x = m_count;
            if (x <= 0)
                return Array.Empty<T>();

            T[] vs = new T[x];
            LinkedNode<T> last = m_head.m_prev;
            while (--x >= 0)
            {
                vs[x] = last.m_value;
                last = last.m_prev;
            }

            return vs;
        }
        #endregion

        #region iset interface
        void ICollection<T>.Add(T item) => Add(item);

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (T item in other)
                Add(item);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (m_count == 0)
                return;

            if (other is ICollection<T> col)
            {
                if (col.Count == 0)
                {
                    Clear();
                    return;
                }

                IntersectWithCollection(col);
            }

            IntersectWithEnumerable(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (m_count == 0)
                UnionWith(other);
            else if (other == this)
                Clear();
            else
            {
                foreach (T item in other)
                    if (!Remove(item))
                        Add(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other == this)
                Clear();
            else if (m_count > 0)
                foreach (T item in other)
                    Remove(item);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (m_count == 0)
                return true;

            foreach (T item in other)
                if (Contains(item))
                    return true;

            return false;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (m_count == 0)
                return false;

            foreach (T item in other)
                if (!Contains(item))
                    return false;

            return true;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (other == this)
                return false;
            if (m_count == 0)
                return true;

            foreach (T item in other)
                if (Contains(item))
                    return true;

            return false;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (other == this)
                return false;
            if (m_count == 0)
                return false;

            foreach (T item in other)
                if (!Contains(item))
                    return false;

            return true;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (m_count > 0)
                foreach (T item in other)
                    if (Contains(item))
                        return true;

            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other is ICollection<T> col && m_count != col.Count)
                return false;

            int notfound = m_count;

            IEnumerator<T> me = GetEnumerator();
            IEnumerator<T> e1 = other.GetEnumerator();

        loop:
            if (me.MoveNext())
            {
                if (e1.MoveNext() && m_comparer.Equals(me.Current, e1.Current))
                    notfound--;

                goto loop;
            }
            else if (e1.MoveNext())
            {
                e1.Dispose();
                me.Dispose();

                return false;
            }

            e1.Dispose();
            me.Dispose();
            return notfound == 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            int b = 0;
            int maxb = m_count.Min(array.Length - arrayIndex);

            LinkedNode<T> node = m_head;
            while (b < maxb)
            {
                array[b++] = node.m_value;
                node = node.m_next;
            }
        }
        #endregion

        #region private
        private void IntersectWithCollection(ICollection<T> other)
        {
            for (int x = 0; x < m_lastIndex; x++)
                if (slots[x].hashCode >= 0)
                {
                    T item = slots[x].node.m_value;
                    if (!other.Contains(item))
                        Remove(item);
                }
        }

        private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
        {
            int ol = m_lastIndex;

            BitArray bit = new BitArray((ol >> 5) + 1);

            int x;
            foreach (T item in other)
            {
                int hc = item.GetHashCode() & bitmask;
                for (x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                    if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].node.m_value, item))
                    {
                        bit[x] = true;
                        break;
                    }
            }

            for (x = 0; x < ol; x++)
                if (slots[x].hashCode >= 0 && !bit[x])
                    Remove(slots[x].node.m_value);
        }
        #endregion

        #region enumerable
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
        private sealed class Slot
        {
            public int hashCode;
            public int next = -1;
            public LinkedNode<T> node = new LinkedNode<T>();

            public void Clear()
            {
                hashCode = 0;
                next = -1;
                node.Disconnect();
            }
        }
        #endregion

        #region structs
        protected struct Enumerator : IEnumerator<T>
        {
            private LinkedHashSet<T> inst;
            private LinkedNode<T> node;
            private T value;
            private uint version;

            public Enumerator(LinkedHashSet<T> inst)
            {
                this.inst = inst;
                node = inst.m_head;
                version = inst.m_version;
                value = default;
            }

            public T Current => value;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (version != inst.m_version)
                    throw new InvalidOperationException();

                if (node == null)
                    return false;

                value = node.m_value;
                node = node.m_next;
                if (node == inst.m_head)
                    node = null;

                return true;
            }

            public void Reset()
            {
                node = inst.m_head;
                value = default;
            }

            public void Dispose()
            {
                inst = null;
                node = null;
                version = 0;
            }
        }
        #endregion
    }
}
