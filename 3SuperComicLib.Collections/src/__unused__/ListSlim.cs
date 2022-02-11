// using System;
// using System.Collections.Generic;
// using System.Linq;
// 
// 
// namespace SuperComicLib.Collections
// {
//     public sealed class ListSlim<T> : IDisposable, IEnumerable<T>, IEnumerableSlim<T>, IList<T>
//     {
//        private T[] m_items;
//        private int m_pos;
//        private uint m_version;

//        #region constructor
//        public ListSlim() => m_items = Array.Empty<T>();

//        public ListSlim(List<T> source)
//        {
//            if (source == null)
//            {
//                m_items = Array.Empty<T>();
//                return;
//            }

//            m_pos = source.Count;
//            m_items = source.ToArray();
//        }

//        public ListSlim(ICollection<T> collection)
//        {
//            if (collection == null)
//            {
//                m_items = Array.Empty<T>();
//                return;
//            }

//            int temp = collection.Count;
//            m_items = new T[temp];
//            m_pos = temp;

//            collection.CopyTo(m_items, 0);
//        }

//        public ListSlim(IEnumerable<T> collection)
//        {
//            if (collection == null)
//            {
//                m_items = Array.Empty<T>();
//                return;
//            }

//            m_items = collection.ToArray();
//            m_pos = m_items.Length;
//        }
//        #endregion

//        #region property
//        public bool IsReadOnly => false;

//        public int Count => m_pos;

//        public int Capacity => m_items.Length;

//        public T this[int index]
//        {
//            get => m_items[index];
//            set
//            {
//                m_items[index] = value;
//                m_version++;
//            }
//        }
//        #endregion

//        #region instance method
//        #region search
//        public bool Contains(T value) => IndexOf(value) >= 0;

//        public int IndexOf(T value)
//        {
//            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
//            for (int x = 0; x < m_pos; x++)
//                if (comparer.Equals(m_items[x], value))
//                    return x;

//            return -1;
//        }
//        #endregion

//        #region add++
//        public void Add(T value)
//        {
//            int newpos = m_pos + 1;
//            EnsureCapacity(newpos);

//            m_items[m_pos] = value;

//            m_version++;
//            m_pos = newpos;
//        }

//        public void Insert(int index, T value)
//        {
//            if (index < 0)
//                return;

//            if (index == m_pos)
//                Add(value);
//            else if (index < m_pos)
//            {
//                int newpos = m_pos + 1;
//                EnsureCapacity(newpos);

//                if (index >= m_items.Length) // out of range
//                    return;

//                Array.Copy(m_items, index, m_items, index + 1, newpos - index);

//                m_pos = newpos;
//                m_version++;
//            }
//        }
//        #endregion

//        #region remove--
//        public bool Remove(T value)
//        {
//            int temp = IndexOf(value);
//            if (temp >= 0)
//            {
//                RemoveAt(temp);
//                return true;
//            }
//            return false;
//        }

//        public void RemoveAt(int index)
//        {
//            if (index >= m_pos)
//                return;

//            m_pos--;
//            if (index < m_pos)
//                Array.Copy(m_items, index + 1, m_items, index, m_pos - index);

//            m_items[m_pos] = default;
//            m_version++;
//        }

//        public void RemoveRange(int count) => RemoveRange(m_pos - count, count);

//        public void RemoveRange(int index, int count)
//        {
//            int tmp = index + count;
//            if (tmp < 0 || tmp > m_pos)
//                return;

//            for (int x = 0; x < count; x++)
//                RemoveAt(index);
//            // m_pos -= count;
//            // if (index < m_pos)
//            //     Array.Copy(m_items, tmp, m_items, index, m_pos - index);
//            // else
//            //     Array.Clear(m_items, index, count);
//            // 
//            // m_version++;
//        }

//        public void Clear()
//        {
//            Array.Clear(m_items, 0, m_pos);

//            m_pos = 0;
//            m_version = 0;
//        }
//        #endregion

//        #region copy or toarray
//        public void CopyTo(T[] arr, int arridx)
//        {
//            if (arr == null || arridx >= arr.Length || arridx < 0)
//                return;

//            Array.Copy(m_items, 0, arr, arridx, Math.Min(arr.Length - arridx, m_items.Length));
//        }

//        public T[] ToArray()
//        {
//            int len = m_pos;
//            T[] result = new T[len];

//            Array.Copy(m_items, 0, result, 0, len);

//            return result;
//        }
//        #endregion

//        private void EnsureCapacity(int newCapacity)
//        {
//            if (newCapacity > m_items.Length)
//            {
//                int len = m_items.Length;
//                T[] newarr = new T[len + 4];

//                Array.Copy(m_items, 0, newarr, 0, len);

//                m_items = newarr;
//            }
//        }
//        #endregion

//        #region enumerable
//        public IForwardIterator<T> Begin() => new Iterator(this);

//        public IForwardIterator<T> RBegin() => new ReverseIterator(this);

//        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//        #endregion

//        #region IDisposable
//        public void Dispose()
//        {
//            m_items = null;
//            m_pos = 0;
//            m_version = 0;
//        }
//        #endregion

//        #region structs
//        private struct Enumerator : IEnumerator<T>
//        {
//            private ListSlim<T> inst;
//            private int idx;
//            private uint version;

//            public Enumerator(ListSlim<T> inst)
//            {
//                this.inst = inst;
//                version = inst.m_version;
//                idx = 0;
//                Current = default;
//            }

//            public T Current { get; private set; }
//            object IEnumerator.Current => Current;

//            public void Dispose()
//            {
//                inst = null;
//                idx = 0;
//                version = 0;
//                Current = default;
//            }

//            public bool MoveNext()
//            {
//                if (version != inst.m_version)
//                    throw new InvalidOperationException();

//                if (idx < inst.m_pos)
//                {
//                    Current = inst.m_items[idx];
//                    idx++;

//                    return true;
//                }
//                else
//                {
//                    Reset();
//                    return false;
//                }
//            }

//            public void Reset()
//            {
//                idx = 0;
//                Current = default;
//            }
//        }

//#pragma warning disable IDE0044
//        private struct Iterator : IForwardIterator<T>
//        {
//            private ListSlim<T> inst;
//            private int idx;

//            public Iterator(ListSlim<T> inst)
//            {
//                this.inst = inst;
//                idx = 0;
//            }

//            public bool IsAlive => idx < inst.m_pos;
//            public T Value
//            {
//                get => inst.m_items[idx];
//                set => inst.m_items[idx] = value;
//            }

//            public void Add() => idx++;

//            public void Reset() => idx = 0;
//        }

//        private struct ReverseIterator : IForwardIterator<T>
//        {
//            private ListSlim<T> inst;
//            private int idx;

//            public ReverseIterator(ListSlim<T> inst)
//            {
//                this.inst = inst;
//                idx = inst.m_pos - 1;
//            }

//            public bool IsAlive => idx >= 0 && idx < inst.m_pos;
//            public T Value
//            {
//                get => inst.m_items[idx];
//                set => inst.m_items[idx] = value;
//            }

//            public void Add() => idx--;

//            public void Reset() => idx = inst.m_pos - 1;
//        }
//#pragma warning restore
//        #endregion
//    }
//}
