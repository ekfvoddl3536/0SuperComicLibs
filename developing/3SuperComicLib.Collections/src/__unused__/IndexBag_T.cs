//using System;
//using System.Collections.Generic;

//namespace SuperComicLib.Collections
//{
//    public class IndexBag<T> : IIndexBag<T>
//    {
//        protected const int bitmask = 0x7FFF_FFFF;
//        protected const int maxlen = 0x7FEF_FFFF;
//        protected const int default_capacity = 4;

//        private readonly IEqualityComparer<T> comparer;
//        private int[] buckets;
//        private Slot[] slots;
//        private int m_count;
//        private int m_freelist;
//        private int m_lastidx;

//        public IndexBag(int capacity, IEqualityComparer<T> comparer)
//        {
//            if (capacity < 0)
//                throw new ArgumentOutOfRangeException(nameof(capacity));

//            this.comparer = comparer;
//            IncreaseCapacity(capacity);
//        }

//        public IndexBag() : this(default_capacity, EqualityComparer<T>.Default) { }

//        public virtual int Count => m_count;

//        public virtual int Capacity => buckets.Length;

//        #region 공개 속성 & 함수
//        public int this[T item] => Find(item);

//        public T[] ToArray()
//        {
//            int x = m_count;
//            T[] res = new T[x];
//            while (x > 0)
//            {
//                x--;
//                res[x] = slots[x].item;
//            }

//            return res;
//        }

//        public bool Contains(T item) => Find(item) >= 0;

//        public int Find(T item)
//        {
//            int hashcode = item.GetHashCode();
//            for (int x = buckets[(hashcode & bitmask) % buckets.Length] - 1; x >= 0; x = slots[x].next)
//                if (comparer.Equals(slots[x].item, item))
//                    return x;
//            return -1;
//        }

//        public int AddOrGet(T item)
//        {
//            int hashcode = item.GetHashCode();
//            int code = hashcode & bitmask;
//            int bucket = code % buckets.Length;
//            for (int x = buckets[bucket] - 1; x >= 0; x = slots[x].next)
//                if (comparer.Equals(slots[x].item, item))
//                    return x;

//            int idx;
//            if (m_freelist >= 0)
//            {
//                idx = m_freelist;
//                m_freelist = slots[idx].next;
//            }
//            else
//            {
//                if (m_count == buckets.Length)
//                {
//                    IncreaseCapacity(NextCapacity(buckets.Length));
//                    bucket = code % buckets.Length;
//                }
//                idx = m_lastidx;
//                m_lastidx++;
//            }

//            ref int target = ref buckets[bucket];

//            Slot slot = slots[idx];
//            slot.item = item;
//            slot.hashcode = hashcode;
//            slot.next = target - 1;

//            target = idx + 1;

//            m_count++;

//            return idx;
//        }

//        public bool TryAdd(T item, out int index)
//        {
//            int hashcode = item.GetHashCode();
//            int code = hashcode & bitmask;
//            int bucket = code % buckets.Length;
//            for (int x = buckets[bucket] - 1; x >= 0; x = slots[x].next)
//                if (comparer.Equals(slots[x].item, item))
//                {
//                    index = -1;
//                    return false;
//                }

//            if (m_freelist >= 0)
//            {
//                index = m_freelist;
//                m_freelist = slots[index].next;
//            }
//            else
//            {
//                if (m_count == buckets.Length)
//                {
//                    IncreaseCapacity(NextCapacity(buckets.Length));
//                    bucket = code % buckets.Length;
//                }
//                index = m_lastidx;
//                m_lastidx++;
//            }

//            ref int target = ref buckets[bucket];

//            Slot slot = slots[index];
//            slot.item = item;
//            slot.hashcode = hashcode;
//            slot.next = target - 1;

//            target = index + 1;

//            m_count++;
//            return true;
//        }

//        public bool Remove(T item)
//        {
//            int hashcode = item.GetHashCode();

//            int idx = (hashcode & bitmask) % buckets.Length;
//            int last = -1;
//            for (int x = buckets[idx] - 1; x >= 0; last = x, x = slots[x].next)
//                if (comparer.Equals(slots[x].item, item))
//                {
//                    if (last < 0)
//                        buckets[idx] = slots[x].next + 1;
//                    else
//                        slots[last].next = slots[x].next;

//                    Slot slot = slots[x];
//                    slot.item = item;
//                    slot.hashcode = hashcode;
//                    slot.next = m_freelist;

//                    m_count--;

//                    if (m_count == 0)
//                    {
//                        m_lastidx = 0;
//                        m_freelist = -1;
//                    }
//                    else
//                        m_freelist = x;

//                    return true;
//                }
//            return false;
//        }

//        public void Clear()
//        {
//            for (int x = 0; x < m_count; x++)
//            {
//                buckets[x] = 0;
//                slots[x].Clear();
//            }

//            m_count = 0;
//            m_lastidx = 0;

//            m_freelist = -1;
//        }
//        #endregion

//        #region capacity 관련 함수
//        protected virtual int NextCapacity(int current_size) => current_size + default_capacity;

//        private void IncreaseCapacity(int newsize)
//        {
//            if (newsize > maxlen)
//            {
//                newsize = maxlen;
//                if (newsize == m_count)
//                    throw new InvalidOperationException("MAX_SIZE");
//            }

//            ref Slot[] slots = ref this.slots;

//            Slot[] nslots = new Slot[newsize];
//            int[] nbks = new int[newsize];

//            int x = 0;
//            for (; x < m_lastidx; x++)
//            {
//                Slot slot = slots[x];
//                ref int val = ref nbks[slot.hashcode % newsize];
//                slot.next = val - 1;
//                nslots[x] = slot;
//                val = x + 1;
//            }
//            for (; x < newsize; x++)
//                nslots[x] = new Slot();

//            slots = nslots;
//            buckets = nbks;
//        }
//        #endregion

//        private sealed class Slot
//        {
//            public T item;
//            public int hashcode = -1;
//            public int next = -1;

//            public void Clear()
//            {
//                item = default;
//                hashcode = -1;
//                next = -1;
//            }
//        }
//    }
//}
