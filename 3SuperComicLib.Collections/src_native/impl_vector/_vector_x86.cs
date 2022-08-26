﻿#if X86
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    unsafe partial struct _vector<T>
    {
        #region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(int size) : this(size, default)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(int size, in T val)
        {
            m_Ptr = (T*)Marshal.AllocHGlobal(size * sizeof(T));
            m_Last = m_Ptr + size;
            m_End = m_Last;

            MemoryBlock.Memset(m_Ptr, size, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(const_iterator<T> first, const_iterator<T> last)
        {
            var len = (int)(last - first);

            m_Ptr = (T*)Marshal.AllocHGlobal(len * sizeof(T));
            m_Last = m_Ptr + len;
            m_End = m_Ptr + len;

            MemoryBlock.Memmove(m_Ptr, first._ptr, len, sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in _vector<T> source) : this(source.cbegin(), source.cend())
        {
        }
        #endregion

        #region indexer & property
        public ref T this[int index] => ref *(m_Ptr + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)size(), nameof(index));
            return ref this[index];
        }

        public int Count => size();

        public int Capacity => capacity();
        #endregion

        #region set capacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void reserve(int capacity)
        {
            if (m_Ptr + capacity > m_End)
                _increaseCapacity(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void resize(int capacity)
        {
            uint cnt = (uint)size();
            uint n = (uint)capacity;

            uint cpyCnt = CMath.Min(cnt, n);

            T* dst = (T*)Marshal.AllocHGlobal((int)n * sizeof(T));
            T* last = dst + cpyCnt;
            T* end = dst + n;

            if (n > cnt)
                MemoryBlock.Clear32((byte*)last, (n - cnt) * (uint)sizeof(T));

            ulong sizeInBytes = cpyCnt * (uint)sizeof(T);
            Buffer.MemoryCopy(m_Ptr, dst, sizeInBytes, sizeInBytes);

            Marshal.FreeHGlobal((IntPtr)m_Ptr);

            m_Ptr = dst;
            m_Last = last;
            m_End = end;
        }
        #endregion

        #region impl interface
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int size() => (int)(m_Last - m_Ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int capacity() => (int)(m_End - m_Ptr);

        ref T IRawContainer<T>.this[int index] => ref this[index];
        ref T IRawContainer<T>.at(int index) => ref at(index);
        #endregion

        #region methods 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void push_back(in T item)
        {
            reserve((int)(m_Last - m_Ptr) + 1);

            *m_Last++ = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T pop_back() =>
            m_Ptr == m_Last
            ? throw new InvalidOperationException("empty collection")
            : *m_Last--;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void insert(int index, in T item)
        {
            if (index >= size())
                throw new ArgumentOutOfRangeException(nameof(index));

            reserve(size() + 1);

            T* dst = m_Ptr + index;

            var sizeInBytes = (ulong)((byte*)m_Last - (byte*)(dst + 1));

            Buffer.MemoryCopy(dst, dst + 1, sizeInBytes, sizeInBytes);

            *dst = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool removeAt(int index)
        {
            if (index >= size())
                return false;

            T* dst = m_Ptr + index;

            MemoryBlock.Memmove(dst + 1, dst, (int)(m_Last - (dst + 1)), sizeof(T));
            m_Last--;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void earse(_iterator<T> position)
        {
            if ((size_t)(position - m_Ptr) >= size())
                throw new ArgumentOutOfRangeException(nameof(position));

            T* dst = position._ptr + 1;
            MemoryBlock.Memmove(dst, position._ptr, (int)(m_Last - dst), sizeof(T));

            m_Last--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void earse(_iterator<T> first, _iterator<T> last)
        {
            if (m_Ptr > first._ptr || m_Last < last._ptr || last._ptr < first._ptr)
                throw new ArgumentOutOfRangeException($"{nameof(first)} or {nameof(last)}");

            T* dst = last._ptr + 1;
            MemoryBlock.Memmove(dst, first._ptr, (int)(m_Last - dst), sizeof(T));

            if (IntPtr.Size == sizeof(T))
                m_Last -= (uint)(last._ptr - first._ptr);
            else
                m_Last -= (ulong)(last._ptr - first._ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64LossOfLength]
        public RawMemory getMemory() => new RawMemory(m_Ptr, (int)size());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _increaseCapacity(int min_size)
        {
            uint old_cnt = (uint)size();
            uint old_capa = (uint)capacity();

            uint new_capa = CMath.Max(old_capa << 1, 4u);
            new_capa = CMath.Max(new_capa, (uint)min_size);

            T* dst = (T*)Marshal.AllocHGlobal((int)new_capa * sizeof(T));
            T* last = dst + old_cnt;
            T* end = dst + new_capa;

            ulong sizeInBytes = old_cnt * (uint)sizeof(T);
            Buffer.MemoryCopy(m_Ptr, dst, sizeInBytes, sizeInBytes);

            Marshal.FreeHGlobal((IntPtr)m_Ptr);

            m_Ptr = dst;
            m_Last = last;
            m_End = end;
        }
        #endregion
    }
}
#endif