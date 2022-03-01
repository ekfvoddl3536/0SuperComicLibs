using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct NativeArray<TItem, TAlloc> : IDisposable
        where TItem : unmanaged
        where TAlloc : unmanaged, IRawAllocater
    {
        internal readonly TItem* m_ptr;
        internal readonly int m_length;

        public NativeArray(int length, bool initDefault = true)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            m_length = length;
            m_ptr = (TItem*)CachedRawAllocater<TAlloc>.Alloc(sizeof(TItem) * length, initDefault);
        }

        public NativeArray(TItem[] source) : this(source?.Length ?? 0, false)
        {
            TItem* ploc = m_ptr;

            for (int i = source.Length; --i >= 0;)
                ploc[i] = source[i];
        }

        public ref TItem this[int index] 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref m_ptr[index];
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_length;
        }

        public RawMemory GetMemory() => new RawMemory(m_ptr, m_length);

        public void Dispose()
        {
            if (m_ptr != null)
                CachedRawAllocater<TAlloc>.Free((IntPtr)m_ptr);
        }
    }
}
