using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed unsafe class RawVector<T> : RawArray<T>, IRawList<T>
        where T : unmanaged
    {
        private int m_last;

        #region constructor
        public RawVector() : this(4)
        {
        }

        public RawVector(int init_length) : base(init_length, true)
        {
        }

        public RawVector(T[] source) : base(source)
        {
        }

        public RawVector(IRawContainer<T> collection) : base(collection)
        {
        }

        public RawVector(RawMemory memory) : base(memory)
        {
        }

        public RawVector(IEnumerable<T> managed_collection) : base(managed_collection)
        {
        }
        #endregion

        public override RawIterator<T> end() => new RawIterator<T>(m_ptr + m_last);

        public override RawReverseIterator<T> rbegin() => new RawReverseIterator<T>(m_ptr + (m_last - 1));

        public override int size() => m_last;

        public void push_back(T item)
        {
            EnsureCapacity(m_last);
            m_ptr[m_last++] = item;
        }

        public T pop_back()
        {
            if (m_last <= 0)
                throw new InvalidOperationException("empty list");

            ref T ref_item = ref m_ptr[--m_last];
            T backup = ref_item;
            ref_item = default;

            return backup;
        }

        public void insert(int index, T item)
        {
            if (index < 0 || index >= m_last)
            {
                push_back(item);
                return;
            }

            EnsureCapacity(m_last);

            this.Internal_CopySelf(index, index + 1, m_last - index);
            
            m_ptr[index] = item;
            m_last++;
        }

        public bool removeAt(int index)
        {
            if (index < 0 || index >= m_last)
                return false;

            int n_idx = index + 1;
            this.Internal_CopySelf(n_idx, index, m_last - n_idx);
            m_ptr[--m_last] = default;
            return true;
        }

        public void earse(RawIterator<T> position)
        {
            RawContainerUtility.CheckVaildateAddress(position.Value, position.Value, m_ptr, m_ptr + m_last);
            RawContainerUtility.Internal_Earse_Single(position, end());

            m_last--;
        }

        public void earse(RawIterator<T> first, RawIterator<T> last)
        {
            if (last.Value < first.Value)
            {
                RawIterator<T> backup = first;
                first = last;
                last = backup;
            }

            RawContainerUtility.CheckVaildateAddress(first.Value, last.Value, m_ptr, m_ptr + m_last);

            m_last -= RawContainerUtility.Internal_Earse(first, last, end());
        }

        public RawArray<T> ToArray() => new RawArray<T>(this);

        public RawMemory ToDirectArray() => new RawMemory(m_ptr, m_last);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int size)
        {
            if (size == m_length)
                RawContainerUtility.Internal_IncreaseCapacity(ref m_ptr, size, size + 12);
        }
    }
}
