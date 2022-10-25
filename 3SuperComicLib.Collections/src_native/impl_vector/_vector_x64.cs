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

#if X64
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
        public _vector(long size) : this(size, default)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(long size, in T val)
        {
            m_Ptr = (T*)Marshal.AllocHGlobal((IntPtr)((ulong)size * (uint)sizeof(T)));
            m_Last = m_Ptr + size;
            m_End = m_Last;

            MemoryBlock.Memset64(m_Ptr, (ulong)size, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(const_iterator<T> first, const_iterator<T> last)
        {
            var len = (ulong)((byte*)last._ptr - (byte*)first._ptr);

            m_Ptr = (T*)Marshal.AllocHGlobal((IntPtr)len);
            m_Last = (T*)((byte*)m_Ptr + len);
            m_End = m_Last;

            Buffer.MemoryCopy(first._ptr, m_Ptr, len, len);
        }
#endregion

#region indexer & property
        public ref T this[long index] => ref *(m_Ptr + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(long index)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)size(), nameof(index));
            return ref this[index];
        }

        public long Count => size();

        public long Capacity => capacity();
#endregion

#region set capacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void reserve(long capacity)
        {
            if (m_Ptr + capacity > m_End)
                _increaseCapacity(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void resize(long capacity)
        {
            ulong cnt = (ulong)size();
            ulong n = (ulong)capacity;

            ulong cpyCnt = CMath.Min(cnt, n);

            T* dst = (T*)Marshal.AllocHGlobal((IntPtr)(n * (uint)sizeof(T)));
            T* last = dst + cpyCnt;
            T* end = dst + n;

            if (n > cnt)
                MemoryBlock.Clear64((byte*)last, (n - cnt) * (uint)sizeof(T));

            ulong sizeInBytes = cpyCnt * (uint)sizeof(T);
            Buffer.MemoryCopy(m_Ptr, dst, sizeInBytes, sizeInBytes);

            Marshal.FreeHGlobal((IntPtr)m_Ptr);

            m_Ptr = dst;
            m_Last = last;
            m_End = end;
        }
#endregion

#region impl longerface
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long size() => m_Last - m_Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long capacity() => m_End - m_Ptr;
#endregion

#region interface impl (readonly)
        ref readonly T IReadOnlyRawContainer<T>.this[long index] => ref this[index];

        ref readonly T IReadOnlyRawContainer<T>.at(long index) => ref at(index);
#endregion

#region methods 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void push_back(in T item)
        {
            reserve(m_Last - m_Ptr + 1);

            *m_Last++ = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void insert(long index, in T item)
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
        public bool removeAt(long index)
        {
            if (index >= size())
                return false;

            T* dst = m_Ptr + index;
            T* src = dst + 1;

            var copysize = (ulong)((byte*)m_Last - (byte*)src);
            Buffer.MemoryCopy(src, dst, copysize, copysize);

            m_Last--;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_iterator<T> position)
        {
            if (position._ptr - m_Ptr >= size())
                throw new ArgumentOutOfRangeException(nameof(position));

            T* dst = position._ptr + 1;

            var copysize = (ulong)((byte*)m_Last - (byte*)dst);
            Buffer.MemoryCopy(dst, position._ptr, copysize, copysize);

            m_Last--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_iterator<T> first, _iterator<T> last)
        {
            if (m_Ptr > first._ptr || m_Last < last._ptr || last._ptr < first._ptr)
                throw new ArgumentOutOfRangeException($"{nameof(first)} or {nameof(last)}");

            T* dst = last._ptr + 1;

            var copysize = (ulong)((byte*)m_Last - (byte*)dst);
            Buffer.MemoryCopy(dst, first._ptr, copysize, copysize);

            m_Last -= (ulong)(last._ptr - first._ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void clear() => MemoryBlock.Clear64((byte*)m_Ptr, (ulong)size() * (uint)sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64LossOfLength]
        public RawMemory getMemory() => new RawMemory(m_Ptr, (int)size());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _increaseCapacity(long min_size)
        {
            ulong old_cnt = (ulong)size();
            ulong old_capa = (ulong)capacity();

            ulong new_capa = CMath.Max(old_capa << 1, 4u);
            new_capa = CMath.Max(new_capa, (ulong)min_size);

            T* dst = (T*)Marshal.AllocHGlobal((IntPtr)(new_capa * (uint)sizeof(T)));
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