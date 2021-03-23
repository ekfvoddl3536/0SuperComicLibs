using System;
using System.Collections;
using System.Collections.Generic;
using SuperComicLib.Numerics;

namespace SuperComicLib.Collections
{
#if DEBUG
    [System.Diagnostics.DebuggerTypeProxy(typeof(CHashSetView<>))]
    [System.Diagnostics.DebuggerDisplay("Count = {m_count}")]
#endif
    public class CHashSet<T> : ISet<T>, IEnumerable<T>, IDisposable
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

        #region constructor
        public CHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            m_freeList = -1;
            m_comparer = comparer;

            if (collection is ICollection<T> item)
                IncreaseCapacity(item.Count.Max(default_capacity));
            else
                IncreaseCapacity(default_capacity);

            IEnumerator<T> e1 = collection.GetEnumerator();
            while (e1.MoveNext())
                Add(e1.Current);

            e1.Dispose();
        }

        public CHashSet(IEnumerable<T> collection) : this(collection, EqualityComparer<T>.Default) { }

        public CHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            m_freeList = -1;
            m_comparer = comparer;

            IncreaseCapacity(capacity.Max(default_capacity));
        }

        public CHashSet(int capacity) : this(capacity, EqualityComparer<T>.Default) { }

        public CHashSet() : this(default_capacity, EqualityComparer<T>.Default) { }
        #endregion

        #region property
        public int Count => m_count;

        public int Capacity => buckets.Length;

        public bool IsReadOnly => false;
        #endregion

        #region methods
        public IHashCodeUpdater<T> Update(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].value, item))
                    return new SlotUpdater(slots[x]);

            return null;
        }

        public bool Contains(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            for (int x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].value, item))
                    return true;

            return false;
        }

        public bool Add(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            int bucket = hc % buckets.Length;
            for (int x = buckets[bucket] - 1; x >= 0; x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].value, item))
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
            slot.value = item;

            temp = index;

            m_count++;

            return true;
        }

        public bool Remove(T item)
        {
            int hc = item.GetHashCode() & bitmask;
            int bucket = hc % buckets.Length;
            int last = -1;
            for (int x = buckets[bucket] - 1; x >= 0; last = x, x = slots[x].next)
                if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].value, item))
                {
                    Slot s = slots[x];
                    if (last < 0)
                        buckets[bucket] = s.next + 1;
                    else
                        slots[last].next = s.next;

                    s.Clear();

                    m_count--;
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
            for (int x = 0; x < buckets.Length; x++)
            {
                buckets[x] = 0;
                slots[x].Clear();
            }

            m_count = 0;
            m_lastIndex = 0;

            m_freeList = -1;
        }

        public T[] ToArray()
        {
            int x = m_lastIndex;
            if (x <= 0)
                return Array.Empty<T>();

            T[] vs = new T[x];
            Slot[] slots = this.slots;
            while (--x >= 0)
                vs[x] = slots[x].value;

            return vs;
        }
        #endregion

        #region iset interface
        void ICollection<T>.Add(T item) => Add(item);

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            IEnumerator<T> e1 = other.GetEnumerator();
            while (e1.MoveNext())
                Add(e1.Current);

            e1.Dispose();
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
                IEnumerator<T> e1 = other.GetEnumerator();
                while (e1.MoveNext())
                    if (!Remove(e1.Current))
                        Add(e1.Current);

                e1.Dispose();
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other == this)
                Clear();
            else if (m_count > 0)
            {
                IEnumerator<T> e1 = other.GetEnumerator();
                while (e1.MoveNext())
                    Remove(e1.Current);

                e1.Dispose();
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (m_count == 0)
                return true;

            int found = m_count;

            IEnumerator<T> e1 = other.GetEnumerator();
            while (e1.MoveNext())
                if (found == 0)
                {
                    e1.Dispose();
                    return true;
                }
                else if (Contains(e1.Current))
                    found--;

            e1.Dispose();
            return false;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (m_count == 0)
                return false;

            IEnumerator<T> e1 = other.GetEnumerator();
            while (e1.MoveNext())
                if (!Contains(e1.Current))
                {
                    e1.Dispose();
                    return false;
                }

            e1.Dispose();
            return true;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) => other != this && IsSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => other != this && IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (m_count > 0)
            {
                IEnumerator<T> e1 = other.GetEnumerator();
                while (e1.MoveNext())
                    if (Contains(e1.Current))
                    {
                        e1.Dispose();
                        return true;
                    }

                e1.Dispose();
            }

            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            IEnumerator<T> e1 = other.GetEnumerator();
            if (other is ICollection<T> col)
            {
                if (col.Count != m_count)
                    return false;
            }

            int notfound = m_count;

            while (e1.MoveNext())
                if (Contains(e1.Current))
                    notfound--;

            e1.Dispose();
            return notfound == 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            int x = 0;
            int maxb = m_lastIndex.Min(array.Length - arrayIndex);
            Slot[] slots = this.slots;
            for (; x < maxb; x++)
                array[arrayIndex + x] = slots[x].value;
        }
        #endregion

        #region private
        private void IntersectWithCollection(ICollection<T> other)
        {
            for (int x = 0; x < m_lastIndex; x++)
                if (slots[x].hashCode >= 0)
                {
                    T item = slots[x].value;
                    if (!other.Contains(item))
                        Remove(item);
                }
        }

        private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
        {
            int ol = m_lastIndex;
            int arrlen = BitsFactory.ToLengthX86(ol);

            Bits bit = new X86Bits(new uint[arrlen]);

            int x;
            IEnumerator<T> e1 = other.GetEnumerator();
            while (e1.MoveNext())
            {
                T item = e1.Current;

                int hc = item.GetHashCode() & bitmask;
                for (x = buckets[hc % buckets.Length] - 1; x >= 0; x = slots[x].next)
                    if (slots[x].hashCode == hc && m_comparer.Equals(slots[x].value, item))
                    {
                        bit.MarkBit(x);
                        break;
                    }
            }
            e1.Dispose();

            for (x = 0; x < ol; x++)
                if (slots[x].hashCode >= 0 && !bit[x])
                    Remove(slots[x].value);

            bit.Dispose();
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

        #region disposable
        protected virtual void Dispose(bool disposing)
        {
            buckets = null;
            slots = null;

            m_count = 0;
            m_freeList = 0;
            m_lastIndex = 0;

            m_comparer = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        }

        private sealed class SlotUpdater : IHashCodeUpdater<T>
        {
            private Slot target;
            private int old_hashcode;

            public SlotUpdater(Slot target)
            {
                this.target = target;
                old_hashcode = target.hashCode;
            }

            public T Item => target.value;

            public void Apply()
            {
                int temp = target.value.GetHashCode() & bitmask;

                target.hashCode = temp;
                old_hashcode = temp; 
            }

            public int GetNewHashCode() => target.value.GetHashCode() & bitmask;

            public int GetOldHashCode() => old_hashcode;
        }
        #endregion

        #region struct
        protected struct Enumerator : IEnumerator<T>
        {
            private CHashSet<T> set;
            private int idx;
            private T current;

            public Enumerator(CHashSet<T> set)
            {
                this.set = set;
                idx = 0;
                current = default;
            }

            public T Current => current;
            object IEnumerator.Current => current;

            public bool MoveNext()
            {
                int last = set.m_lastIndex;
                while (idx < last)
                    if (set.slots[idx].hashCode >= 0)
                    {
                        current = set.slots[idx++].value;
                        return true;
                    }
                    else
                        idx++;

                current = default;
                return false;
            }

            public void Reset()
            {
                idx = 0;
                current = default;
            }

            public void Dispose()
            {
                set = null;
                idx = 0;
                current = default;
            }
        }
        #endregion
    }
}
