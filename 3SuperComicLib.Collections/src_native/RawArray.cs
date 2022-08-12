using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class RawArray<T> : IDisposable, IRawContainer<T>, IReadOnlyRawContainer<T>
        where T : unmanaged
    {
        protected internal T* m_ptr;
        protected internal int m_length;

        public RawArray(int length, bool initDefault = true)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            m_length = length;
            m_ptr = (T*)Marshal.AllocHGlobal(length * sizeof(T));

            if (initDefault)
                RawContainerUtility.Internal_Clear(m_ptr, m_ptr + length);
        }

        public RawArray(T[] source) : this(source?.Length ?? 0, false)
        {
            T* ploc = m_ptr;

            for (int i = source.Length; --i >= 0;)
                ploc[i] = source[i];
        }

        public RawArray(IRawContainer<T> collection) : this(collection.size(), false) => 
            collection.CopyTo(this);

        public RawArray(RawMemory memory)
        {
            m_length = memory._size;
            m_ptr = (T*)memory._value;
        }

        public RawArray(IEnumerable<T> managed_collection) : this(managed_collection.ToArray())
        {
        }

        #region indexer
        /// <summary>
        /// indexing fastest. "UNSAFE METHOD"
        /// </summary>
        public ref T this[int idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref m_ptr[idx];
        }

        /// <summary>
        /// indexing after index range validate check
        /// </summary>
        public ref T at(int index)
        {
            if (index < 0 || index >= m_length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref m_ptr[index];
        }

        ref readonly T IReadOnlyRawContainer<T>.this[int index] => ref this[index];
        ref readonly T IReadOnlyRawContainer<T>.at(int index) => ref at(index);
        #endregion

        #region container impl
        public virtual RawIterator<T> begin() => new RawIterator<T>(m_ptr);
        public virtual RawIterator<T> end() => new RawIterator<T>(m_ptr + m_length);

        public virtual RawReverseIterator<T> rbegin() => new RawReverseIterator<T>(m_ptr + (m_length - 1));
        public virtual RawReverseIterator<T> rend() => new RawReverseIterator<T>(m_ptr - 1);

        public virtual RawConstIterator<T> cbegin() => new RawConstIterator<T>(m_ptr);
        public virtual RawConstIterator<T> cend() => new RawConstIterator<T>(m_ptr + m_length);

        public virtual RawConstReverseIterator<T> crbegin() => new RawConstReverseIterator<T>(m_ptr + (m_length - 1));
        public virtual RawConstReverseIterator<T> crend() => new RawConstReverseIterator<T>(m_ptr - 1);

        public virtual RawMemory getMemory() => new RawMemory(m_ptr, m_length);

        public virtual int size() => m_length;

        public virtual int capacity() => sizeof(T) * m_length;
        #endregion

        #region override
        public override bool Equals(object obj) => obj is IRawContainer<T> v0 && v0.SequenceEqual(this);

        public override int GetHashCode() => ((IntPtr)m_ptr).GetHashCode();

        public override string ToString() => $"Count: {size()}, Addrs: 0x{GetHashCode():X}";
        #endregion

        #region dispose
        ~RawArray()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Marshal.FreeHGlobal((IntPtr)m_ptr);

            if (disposing)
                m_ptr = null;
        }
        #endregion
    }
}
