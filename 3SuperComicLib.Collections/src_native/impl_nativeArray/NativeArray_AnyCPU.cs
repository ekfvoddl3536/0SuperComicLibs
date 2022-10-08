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

#if AnyCPU
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    unsafe partial struct NativeArray<T>
    {
        public readonly T* Ptr;
        private readonly void* _length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Ptr = (T*)Marshal.AllocHGlobal(source.Length * sizeof(T));
            _length = (void*)source.Length;

            if (source.Length > 0)
                fixed (T* psrc = &source[0])
                {
                    ulong cb = (ulong)(uint)source.Length * (uint)sizeof(T);
                    Buffer.MemoryCopy(psrc, Ptr, cb, cb);
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(int length)
        {
            Ptr = (T*)Marshal.AllocHGlobal(length * sizeof(T));
            _length = (void*)length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeArray(long length)
        {
            Ptr = (T*)Marshal.AllocHGlobal((IntPtr)((ulong)length * (uint)sizeof(T)));
            _length = (void*)length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T* hGlobalAllocated_Ptr, int length)
        {
            Ptr = hGlobalAllocated_Ptr;
            _length = (void*)length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeArray(T* hGlobalAllocated_Ptr, long length)
        {
            Ptr = hGlobalAllocated_Ptr;
            _length = (void*)length;
        }
        #endregion

        #region indexer & property
        public ref T this[int index] => ref Ptr[index];

        [CodeContracts.X64Only]
        public ref T this[long index] => ref Ptr[index];

        public int Length => (int)_length;

        [CodeContracts.X64Only]
        public long LongLength => (long)_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref this[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public ref T at(long index)
        {
            if ((ulong)index >= (ulong)LongLength)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref this[index];
        }
        #endregion

        #region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() =>
            IntPtr.Size == sizeof(int)
            ? new NativeSpan<T>(Ptr, Length)
            : new NativeSpan<T>(Ptr, LongLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear(Ptr, _length, sizeof(T));
        #endregion

        #region explicit implement interfaces
        ref T IRawContainer<T>.this[size_t index] =>
            ref IntPtr.Size == sizeof(int)
            ? ref this[(int)index.value]
            : ref this[(long)index.value];
        ref T IRawContainer<T>.at(size_t index) =>
            ref IntPtr.Size == sizeof(int)
            ? ref at((int)index.value)
            : ref at((long)index.value);
        ref readonly T IReadOnlyRawContainer<T>.this[size_t index] =>
            ref IntPtr.Size == sizeof(int)
            ? ref this[(int)index.value]
            : ref this[(long)index.value];
        ref readonly T IReadOnlyRawContainer<T>.at(size_t index) =>
            ref IntPtr.Size == sizeof(int)
            ? ref at((int)index.value)
            : ref at((long)index.value);

        size_t IRawContainer.capacity() =>
            IntPtr.Size == sizeof(int)
            ? Length
            : LongLength;
        size_t IRawContainer.size() =>
            IntPtr.Size == sizeof(int)
            ? Length
            : LongLength;
        #endregion

        #region implement interfaces
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64LossOfLength]
        public RawMemory getMemory() => new RawMemory(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() =>
            IntPtr.Size == sizeof(int)
            ? new _iterator<T>(Ptr + Length)
            : new _iterator<T>(Ptr + LongLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() =>
            IntPtr.Size == sizeof(int)
            ? new reverse_iterator<T>(Ptr + (Length - 1))
            : new reverse_iterator<T>(Ptr + (LongLength - 1));
        #endregion

        #region explicit impl interface
        bool IEquatable<NativeArray<T>>.Equals(NativeArray<T> other) => this == other;
        #endregion

        #region override
        public override bool Equals(object obj) => obj is NativeArray<T> other && this == other;
        public override int GetHashCode() => ((IntPtr)Ptr).GetHashCode() ^ Length;
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReferenceEquals(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr == right.Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeArray<T> left, in NativeArray<T> right) =>
            left.Ptr == right.Ptr && left._length == right._length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeArray<T> left, in NativeArray<T> right) =>
            left.Ptr != right.Ptr || left._length != right._length;
        #endregion
    }
}
#endif