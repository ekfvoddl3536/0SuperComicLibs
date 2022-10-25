// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#if X86
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

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

            MemoryBlock.Memset32(m_Ptr, (uint)size, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(const_iterator<T> first, const_iterator<T> last)
        {
            var len = (int)((byte*)last._ptr - (byte*)first._ptr);

            m_Ptr = (T*)Marshal.AllocHGlobal(len);
            m_Last = (T*)((byte*)m_Ptr + len);
            m_End = m_Last;

            ulong copysize = (uint)len;
            Buffer.MemoryCopy(first._ptr, m_Ptr, copysize, copysize);
        }
#endregion

#region indexer & property
        public ref T this[int index] => ref *(m_Ptr + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)size(), nameof(index));
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
#endregion

#region interface impl (readonly)
        ref readonly T IReadOnlyRawContainer<T>.this[int index] => ref this[index];

        ref readonly T IReadOnlyRawContainer<T>.at(int index) => ref at(index);
#endregion

#region methods 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void push_back(in T item)
        {
            reserve((int)(m_Last - m_Ptr) + 1);

            *m_Last++ = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void insert(int index, in T item)
        {
            if (index >= size())
                throw new ArgumentOutOfRangeException(nameof(index));

            reserve(size() + 1);

            T* src = m_Ptr + index;
            T* dst = src + 1;

            var copysize = (ulong)((byte*)m_Last - (byte*)dst);
            Buffer.MemoryCopy(src, dst, copysize, copysize);

            *src = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool removeAt(int index)
        {
            if (index >= size())
                return false;

            T* dst = m_Ptr + index;
            T* src = dst + 1;

            ulong copysize = (uint)((byte*)m_Last - (byte*)src);
            Buffer.MemoryCopy(src, dst, copysize, copysize);

            m_Last--;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_iterator<T> position)
        {
            if ((int)(position._ptr - m_Ptr) >= size())
                throw new ArgumentOutOfRangeException(nameof(position));

            T* dst = position._ptr + 1;

            ulong copysize = (uint)((byte*)m_Last - (byte*)dst);
            Buffer.MemoryCopy(dst, position._ptr, copysize, copysize);

            m_Last--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_iterator<T> first, _iterator<T> last)
        {
            if (m_Ptr > first._ptr || m_Last < last._ptr || last._ptr < first._ptr)
                throw new ArgumentOutOfRangeException($"{nameof(first)} or {nameof(last)}");

            T* dst = last._ptr + 1;

            ulong copysize = (uint)((byte*)m_Last - (byte*)dst);
            Buffer.MemoryCopy(dst, first._ptr, copysize, copysize);

            m_Last -= (uint)(last._ptr - first._ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void clear() => MemoryBlock.Clear32((byte*)m_Ptr, (uint)size() * (uint)sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawMemory getMemory() => new RawMemory(m_Ptr, size());

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