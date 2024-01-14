// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;
using SuperComicLib.Runtime;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct _vector<T> : IRawList<T>, IReadOnlyRawContainer<T>, IDisposable
        where T : unmanaged
    {
        private T* m_Ptr;
        private T* m_Last;
        private T* m_End;

        #region constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in _vector<T> source) : this(source.cbegin(), source.cend())
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(long size) : this(size, default)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(long size, in T val)
        {
            m_Ptr = (T*)MemoryBlock.Memalloc(size * sizeof(T));
            m_Last = m_Ptr + size;
            m_End = m_Last;

            MemoryBlock.Fill(m_Ptr, (ulong)size, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(const_iterator<T> first, const_iterator<T> last) : this(first._ptr, last._ptr)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(T* first, T* last)
        {
            var blen = last - first;

            m_Ptr = (T*)MemoryBlock.Memalloc(blen);
            m_Last = (T*)((byte*)m_Ptr + blen);
            m_End = m_Last;

            MemoryBlock.Memmove(first, m_Ptr, (ulong)blen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in NativeSpan<T> source)
        {
            var blen = source.Length * sizeof(T);

            m_Ptr = (T*)MemoryBlock.Memalloc(blen);
            m_Last = (T*)((byte*)m_Ptr + blen);
            m_End = m_Last;

            MemoryBlock.Memmove(source.Source, m_Ptr, (ulong)blen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in NativeConstSpan<T> source)
        {
            var blen = source.Length * sizeof(T);

            m_Ptr = (T*)MemoryBlock.Memalloc(blen);
            m_Last = (T*)((byte*)m_Ptr + blen);
            m_End = m_Last;

            MemoryBlock.Memmove(source.DangerousGetPointer(), m_Ptr, (ulong)blen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in arrayref<T> source) : this(source.AsSpan()) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in arrayrefSegment<T> source) : this(source.AsSpan()) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(SafeArrayref<T> source) : this(source.AsSpan()) { }

        /// <summary>
        /// Fast object initialization
        /// </summary>
        /// <param name="hglobal_first">A pointer to the starting position allocated with the <see cref="Marshal.AllocHGlobal(IntPtr)"/>.</param>
        /// <param name="hglobal_end">
        /// End of this vector. 
        /// The last value is not included in the range. Similar to the role of 'Length', do not confuse with 'Index'.</param>
        /// <param name="hglobal_last">
        /// The position to start appending with push. Similar role to 'Count', so it must not be less than <paramref name="hglobal_first"/> 
        /// or equal to or greater than <paramref name="hglobal_end"/>.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, NoExcept]
        public _vector(T* hglobal_first, T* hglobal_last, T* hglobal_end)
        {
            m_Ptr = hglobal_first;
            m_Last = hglobal_last;
            m_End = hglobal_end;
        }
        #endregion

        #region indexer & property
        public ref T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(m_Ptr + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(long index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, size());
            return ref this[index];
        }

        public long Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => size();
        }

        public long Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => capacity();
        }
        #endregion

        #region impl interface -1-
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long size() => m_Last - m_Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long capacity() => m_End - m_Ptr;
        #endregion

        #region impl interface -2-
        ref readonly T IReadOnlyRawContainer<T>.this[long index] => ref this[index];

        ref readonly T IReadOnlyRawContainer<T>.at(long index) => ref at(index);
        #endregion

        #region methods 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void push_back(in T item)
        {
            reserve(size() + 1);

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

            MemoryBlock.Memmove(src, dst, (ulong)((byte*)m_Last - (byte*)dst));

            *src = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool removeAt(long index)
        {
            if (index >= size())
                return false;

            T* dst = m_Ptr + index;
            T* src = dst + 1;

            MemoryBlock.Memmove(src, dst, (ulong)((byte*)m_Last - (byte*)src));

            m_Last--;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_iterator<T> position)
        {
            if ((ulong)(position._ptr - m_Ptr) >= (ulong)size())
                throw new ArgumentOutOfRangeException(nameof(position));

            T* dst = position._ptr + 1;

            MemoryBlock.Memmove(dst, position._ptr, (ulong)((byte*)m_Last - (byte*)dst));

            m_Last--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void erase(_iterator<T> first, _iterator<T> last)
        {
            if (m_Ptr > first._ptr || m_Last < last._ptr || last._ptr < first._ptr)
                throw new ArgumentOutOfRangeException($"{nameof(first)} or {nameof(last)}");

            T* dst = last._ptr + 1;

            MemoryBlock.Memmove(dst, first._ptr, (ulong)((byte*)m_Last - (byte*)dst));

            m_Last -= last._ptr - first._ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void clear() => MemoryBlock.Clear((byte*)m_Ptr, (ulong)(size() * sizeof(T)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawContainerBuffer getRawContainerBuffer() => new RawContainerBuffer(m_Ptr, size());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _increaseCapacity(long min_size)
        {
            var old_cnt = size();
            var old_capa = capacity();

            var new_capa = (long)CMath.Max((ulong)old_capa << 1, 4u);
            new_capa = (long)CMath.Max((ulong)new_capa, (ulong)min_size);

            T* dst = (T*)MemoryBlock.Memalloc(new_capa * sizeof(T));
            T* last = dst + old_cnt;
            T* end = dst + new_capa;

            MemoryBlock.Memmove(m_Ptr, dst, (ulong)old_cnt);

            Marshal.FreeHGlobal((IntPtr)m_Ptr);

            m_Ptr = dst;
            m_Last = last;
            m_End = end;
        }
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
            var cnt = size();
            var n = capacity;

            var cpyCnt = (long)CMath.Min((ulong)cnt, (ulong)n);

            T* dst = (T*)MemoryBlock.Memalloc(n * (uint)sizeof(T));
            T* last = dst + cpyCnt;
            T* end = dst + n;

            if (n > cnt)
                MemoryBlock.Clear((byte*)last, (ulong)((n - cnt) * sizeof(T)));

            MemoryBlock.Memmove(m_Ptr, dst, (ulong)cpyCnt);

            Marshal.FreeHGlobal((IntPtr)m_Ptr);

            m_Ptr = dst;
            m_Last = last;
            m_End = end;
        }
        #endregion

        #region iterator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(m_Ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(m_Last);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(m_Last - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(m_Ptr - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => rend();
        #endregion

        #region collection control method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T pop_back() =>
            m_Ptr == m_Last
            ? throw new InvalidOperationException("empty collection")
            : *m_Last--;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T pop_back_unsafe() => *m_Last--;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void fast_clear() => m_Last = m_Ptr;
        #endregion

        #region common
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>(begin(), end());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Marshal.FreeHGlobal((IntPtr)m_Ptr);
        #endregion
    }
}
