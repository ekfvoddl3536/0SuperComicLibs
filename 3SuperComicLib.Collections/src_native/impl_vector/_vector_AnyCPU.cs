#if AnyCPU
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
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public _vector(size_t size) : this(size, default)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public _vector(size_t size, in T val)
        {
            m_Ptr = (T*)MemoryBlock.Memalloc(size, sizeof(T));
            m_Last = ptr_offset<T>.add(m_Ptr, size);
            m_End = m_Last;

            MemoryBlock.Memset(m_Ptr, size, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(const_iterator<T> first, const_iterator<T> last)
        {
            var len = last - first;

            if (IntPtr.Size == sizeof(int))
            {
                m_Ptr = (T*)Marshal.AllocHGlobal((int)len * sizeof(T));
                m_Last = m_Ptr + (int)len;
                m_End = m_Ptr + (int)len;
            }
            else
            {
                m_Ptr = (T*)Marshal.AllocHGlobal((IntPtr)((ulong)len * (uint)sizeof(T)));
                m_Last = m_Ptr + (long)len;
                m_End = m_Ptr + (long)len;
            }

            MemoryBlock.Memmove(m_Ptr, first._ptr, len, sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _vector(in _vector<T> source) : this(source.cbegin(), source.cend())
        {
        }
        #endregion

        #region indexer & property
        public ref T this[int index] => ref *(m_Ptr + index);

        [CodeContracts.X64Only]
        public ref T this[long index] => ref *(m_Ptr + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)size().value, nameof(index));
            return ref this[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public ref T at(long index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)size().value, nameof(index));
            return ref this[index];
        }

        [CodeContracts.X64LossOfLength]
        public int Count => (int)size().value;

        [CodeContracts.X64Only]
        public long LongCount => (long)size().value;

        [CodeContracts.X64LossOfLength]
        public int Capacity => (int)capacity().value;

        [CodeContracts.X64Only]
        public long LongCapacity => (long)capacity().value;
        #endregion

        #region set capacity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void reserve(size_t capacity)
        {
            T* tmp = ptr_offset<T>.add(m_Ptr, capacity);

            if (tmp > m_End)
                _increaseCapacity(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void resize(size_t capacity)
        {
            T* dst, last, end;

            if (IntPtr.Size == sizeof(int))
            {
                uint cnt = (uint)size().value;
                uint n = (uint)capacity.value;

                uint cpyCnt = CMath.Min(cnt, n);

                dst = (T*)Marshal.AllocHGlobal((int)(n * (uint)sizeof(T)));
                last = dst + cpyCnt;
                end = dst + n;

                if (n > cnt)
                    MemoryBlock.Clear32((byte*)last, (n - cnt) * (uint)sizeof(T));

                ulong sizeInBytes = cpyCnt * (uint)sizeof(T);
                Buffer.MemoryCopy(m_Ptr, dst, sizeInBytes, sizeInBytes);
            }
            else
            {
                ulong cnt = (ulong)size().value;
                ulong n = (ulong)capacity.value;

                ulong cpyCnt = CMath.Min(cnt, n);

                dst = (T*)Marshal.AllocHGlobal((IntPtr)(n * (uint)sizeof(T)));
                last = dst + cpyCnt;
                end = dst + n;

                if (n > cnt)
                    MemoryBlock.Clear64((byte*)last, (n - cnt) * (uint)sizeof(T));

                ulong sizeInBytes = cpyCnt * (uint)sizeof(T);
                Buffer.MemoryCopy(m_Ptr, dst, sizeInBytes, sizeInBytes);
            }

            Marshal.FreeHGlobal((IntPtr)m_Ptr);

            m_Ptr = dst;
            m_Last = last;
            m_End = end;
        }
        #endregion

        #region impl interface
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t size() => m_Last - m_Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t capacity() => m_End - m_Ptr;

        ref T IRawContainer<T>.this[size_t index] =>
            ref IntPtr.Size == sizeof(int)
            ? ref this[(int)index.value]
            : ref this[(long)index.value];

        ref T IRawContainer<T>.at(size_t index) =>
            ref IntPtr.Size == sizeof(int)
            ? ref at((int)index.value)
            : ref at((long)index.value);
        #endregion

        #region methods 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void push_back(in T item)
        {
            // operator order -> long (T* (T* + 1) - T*)
            reserve((m_Last + 1) - m_Ptr);

            *m_Last++ = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T pop_back() =>
            m_Ptr == m_Last
            ? throw new InvalidOperationException("empty collection")
            : *m_Last--;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void insert(size_t index, in T item)
        {
            if (index >= size())
                throw new ArgumentOutOfRangeException(nameof(index));

            reserve(size() + 1);

            T* dst =
                IntPtr.Size == sizeof(int)
                ? m_Ptr + (uint)index.value
                : m_Ptr + (ulong)index.value;

            var sizeInBytes = (ulong)((byte*)m_Last - (byte*)(dst + 1));

            Buffer.MemoryCopy(dst, dst + 1, sizeInBytes, sizeInBytes);

            *dst = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool removeAt(size_t index)
        {
            if (index >= size())
                return false;

            T* dst =
                IntPtr.Size == sizeof(int)
                ? m_Ptr + (uint)index.value
                : m_Ptr + (ulong)index.value;

            MemoryBlock.Memmove(dst + 1, dst, m_Last - (dst + 1), sizeof(T));
            m_Last--;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void earse(_iterator<T> position)
        {
            if ((size_t)(position - m_Ptr) >= size())
                throw new ArgumentOutOfRangeException(nameof(position));

            T* dst = position._ptr + 1;
            MemoryBlock.Memmove(dst, position._ptr, m_Last - dst, sizeof(T));

            m_Last--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void earse(_iterator<T> first, _iterator<T> last)
        {
            if (m_Ptr > first._ptr || m_Last < last._ptr || last._ptr < first._ptr)
                throw new ArgumentOutOfRangeException($"{nameof(first)} or {nameof(last)}");

            T* dst = last._ptr + 1;
            MemoryBlock.Memmove(dst, first._ptr, m_Last - dst, sizeof(T));

            if (IntPtr.Size == sizeof(T))
                m_Last -= (uint)(last._ptr - first._ptr);
            else
                m_Last -= (ulong)(last._ptr - first._ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64LossOfLength]
        public RawMemory getMemory() => new RawMemory(m_Ptr, (int)size().value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _increaseCapacity(size_t min_size)
        {
            T* dst, last, end;

            ulong sizeInBytes;

            if (IntPtr.Size == sizeof(int))
            {
                uint old_cnt = (uint)size().value;
                uint old_capa = (uint)capacity().value;

                uint new_capa = CMath.Max(old_capa << 1, 4u);
                if (new_capa < old_capa) // overflow
                    throw new OutOfMemoryException(nameof(_vector<T>));

                new_capa = CMath.Max(new_capa, (uint)min_size.value);

                sizeInBytes = old_cnt * (uint)sizeof(T);

                dst = (T*)Marshal.AllocHGlobal((int)new_capa * sizeof(T));
                last = dst + old_cnt;
                end = dst + new_capa;
            }
            else
            {
                ulong old_cnt = (ulong)size().value;
                ulong old_capa = (ulong)capacity().value;

                ulong new_capa = CMath.Max(old_capa << 1, 4u);
                if (new_capa < old_capa)
                    throw new OutOfMemoryException(nameof(_vector<T>));

                new_capa = CMath.Max(new_capa, (ulong)min_size.value);

                sizeInBytes = old_cnt * (uint)sizeof(T);

                dst = (T*)Marshal.AllocHGlobal((IntPtr)(new_capa * (uint)sizeof(T)));
                last = dst + old_cnt;
                end = dst + new_capa;
            }

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