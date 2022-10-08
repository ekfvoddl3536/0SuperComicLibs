﻿// MIT License
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
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    unsafe partial struct NativeArray<T>
    {
        public readonly T* Ptr;
        public readonly long Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(long length)
        {
            Ptr = (T*)Marshal.AllocHGlobal((IntPtr)((ulong)length * (uint)sizeof(T)));
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T* hGlobalAllocated_Ptr, long length)
        {
            Ptr = hGlobalAllocated_Ptr;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Length = source.LongLength;
            Ptr = (T*)Marshal.AllocHGlobal((IntPtr)((ulong)source.LongLength * (uint)sizeof(T)));

            if (source.Length > 0)
                fixed (T* psrc = &source[0])
                {
                    ulong cb = (ulong)source.Length * (uint)sizeof(T);
                    Buffer.MemoryCopy(psrc, Ptr, cb, cb);
                }
        }
#endregion

#region indexer & property
        public ref T this[long index] => ref Ptr[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(long index)
        {
            if ((ulong)index >= (ulong)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref this[index];
        }
#endregion

#region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear64((byte*)Ptr, (ulong)Length * (uint)sizeof(T));
#endregion

#region explicit implement longerfaces
        ref readonly T IReadOnlyRawContainer<T>.this[long index] => ref this[index];
        ref readonly T IReadOnlyRawContainer<T>.at(long index) => ref at(index);

        long IRawContainer.capacity() => Length;
        long IRawContainer.size() => Length;
#endregion

#region implement longerfaces
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64LossOfLength]
        public RawMemory getMemory() => new RawMemory(Ptr, (int)Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Ptr + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Ptr + (Length - 1));
#endregion

#region explicit impl longerface
        bool IEquatable<NativeArray<T>>.Equals(NativeArray<T> other) => this == other;
#endregion

#region override
        public override bool Equals(object obj) => obj is NativeArray<T> other && this == other;
        public override int GetHashCode() => ((IntPtr)Ptr).GetHashCode() ^ Length.GetHashCode();
#endregion

#region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReferenceEquals(in NativeArray<T> left, in NativeArray<T> right) => left.Ptr == right.Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeArray<T> left, in NativeArray<T> right) =>
            left.Ptr == right.Ptr && left.Length == right.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeArray<T> left, in NativeArray<T> right) =>
            left.Ptr != right.Ptr || left.Length != right.Length;
#endregion
    }
}
#endif