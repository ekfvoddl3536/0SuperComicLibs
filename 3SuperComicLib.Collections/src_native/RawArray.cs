using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class RawArray<TItem, TAlloc> : IDisposable, IRawContainer<TItem>
        where TItem : unmanaged
        where TAlloc : unmanaged, IRawAllocater
    {
        protected internal TItem* m_ptr;
        protected internal int m_length;

        public RawArray(int length, bool initDefault = true)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            m_length = length;
            m_ptr = (TItem*)CachedRawAllocater<TAlloc>.Alloc(sizeof(TItem) * length, initDefault);
        }

        public RawArray(TItem[] source) : this(source?.Length ?? 0, false)
        {
            TItem* ploc = m_ptr;

            for (int i = source.Length; --i >= 0;)
                ploc[i] = source[i];
        }

        public RawArray(IRawContainer<TItem> collection) : this(collection?.size() ?? 0, false) => 
            collection.CopyTo(this);

        public RawArray(RawMemory memory)
        {
            m_length = memory._size;
            m_ptr = (TItem*)memory._value;
        }

        public RawArray(IEnumerable<TItem> managed_collection) : this(managed_collection.ToArray())
        {
        }

        #region indexer
        /// <summary>
        /// indexing fastest. "UNSAFE METHOD"
        /// </summary>
        public ref TItem this[int idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref m_ptr[idx];
        }

        /// <summary>
        /// indexing after index range validate check
        /// </summary>
        public ref TItem at(int index)
        {
            if (index < 0 || index >= m_length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref m_ptr[index];
        }
        #endregion

        #region container impl
        public virtual RawIterator<TItem> begin() => new RawIterator<TItem>(m_ptr);
        public virtual RawIterator<TItem> end() => new RawIterator<TItem>(m_ptr + m_length);

        public virtual RawReverseIterator<TItem> rbegin() => new RawReverseIterator<TItem>(m_ptr + (m_length - 1));
        public virtual RawReverseIterator<TItem> rend() => new RawReverseIterator<TItem>(m_ptr - 1);

        public virtual RawMemory getMemory() => new RawMemory(m_ptr, m_length);

        public virtual int size() => m_length;

        public virtual int capacity() => sizeof(TItem) * m_length;
        #endregion

        #region override
        public override bool Equals(object obj) => obj is IRawContainer<TItem> v0 && v0.SequenceEqual(this);

        public override int GetHashCode() => ((IntPtr)m_ptr).GetHashCode();

        public override string ToString() => $"Count: {size()}, Addrs: 0x{GetHashCode():X}";
        #endregion

        #region dispose
        ~RawArray()
        {
            Dispose(false);
        }

        /// <exception cref="OutOfMemoryException">
        /// 메모리 누수를 감지한 경우 발생됩니다.
        /// <typeparamref name="TAlloc"/>이 <see cref="IRawPersistentAllocater"/>이고, Dispose()로 해제되지 않은 경우
        /// </exception>
        /// <summary>
        /// 메모리를 정리합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <exception cref="OutOfMemoryException">메모리 누수를 감지한 경우 발생됩니다.</exception>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing && CachedRawAllocater<TAlloc>.persistentAlloc)
            {
                if (m_ptr != null)
                    throw new OutOfMemoryException($"detect ---MEMORYLEAK---");
            }
            else if (m_ptr != null)
            {
                CachedRawAllocater<TAlloc>.Free((IntPtr)m_ptr);
                m_ptr = null;
            }
        }
        #endregion
    }
}
