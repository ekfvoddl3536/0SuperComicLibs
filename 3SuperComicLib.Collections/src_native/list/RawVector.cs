using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed unsafe class RawVector<TItem, TAlloc> : RawArray<TItem, TAlloc>, IRawList<TItem>
        where TItem : unmanaged
        where TAlloc : unmanaged, IRawAllocater
    {
        private int m_last;

        #region constructor
        public RawVector() : this(4)
        {
        }

        public RawVector(int init_length) : base(init_length, true)
        {
        }

        public RawVector(TItem[] source) : base(source)
        {
        }

        public RawVector(IRawContainer<TItem> collection) : base(collection)
        {
        }

        public RawVector(RawMemory memory) : base(memory)
        {
        }

        public RawVector(IEnumerable<TItem> managed_collection) : base(managed_collection)
        {
        }
        #endregion

        public override RawIterator<TItem> end() => new RawIterator<TItem>(m_ptr + m_last);

        public override RawReverseIterator<TItem> rbegin() => new RawReverseIterator<TItem>(m_ptr + (m_last - 1));

        public override int size() => m_last;

        public void push_back(TItem item)
        {
            EnsureCapacity(m_last);
            m_ptr[m_last++] = item;
        }

        public TItem pop_back()
        {
            if (m_last <= 0)
                throw new InvalidOperationException("empty list");

            ref TItem ref_item = ref m_ptr[--m_last];
            TItem backup = ref_item;
            ref_item = default;

            return backup;
        }

        public void insert(int index, TItem item)
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

        public void earse(RawIterator<TItem> position)
        {
            RawContainerUtility.CheckVaildateAddress(position.Value, position.Value, m_ptr, m_ptr + m_last);
            RawContainerUtility.Internal_Earse_Single(position, end());

            m_last--;
        }

        public void earse(RawIterator<TItem> first, RawIterator<TItem> last)
        {
            if (last.Value < first.Value)
            {
                RawIterator<TItem> backup = first;
                first = last;
                last = backup;
            }

            RawContainerUtility.CheckVaildateAddress(first.Value, last.Value, m_ptr, m_ptr + m_last);

            m_last -= RawContainerUtility.Internal_Earse(first, last, end());
        }

        public RawArray<TItem, TAlloc> ToArray() => new RawArray<TItem, TAlloc>(this);

        public RawMemory ToDirectArray() => new RawMemory(m_ptr, m_last);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int size)
        {
            if (size == m_length)
                RawContainerUtility.Internal_IncreaseCapacity<TItem, TAlloc>(ref m_ptr, size, size + 12);
        }
    }
}
