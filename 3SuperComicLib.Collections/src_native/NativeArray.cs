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

using SuperComicLib.RuntimeMemoryMarshals;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct NativeArray<T> : IDisposable, IRawContainer<T>, IReadOnlyRawContainer<T>, IEquatable<NativeArray<T>>
        where T : unmanaged
    {
        public readonly T* Source;
        public readonly long Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(long length)
        {
            Source = (T*)MemoryBlock.Memalloc(length, sizeof(T));
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T* hGlobalAllocated_Ptr, long length)
        {
            Source = hGlobalAllocated_Ptr;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(T[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // don't MOVSXD r64/r32
            Length = (uint)source.Length;

            var sz = (ulong)Length * (uint)sizeof(T);
            Source = (T*)Marshal.AllocHGlobal((IntPtr)sz);

            if (source.Length > 0)
                fixed (T* psrc = &source[0])
                    MemoryBlock.Memmove(psrc, Source, sz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(in NativeSpan<T> source)
        {
            var blen = source.Length * sizeof(T);

            Length = source.Length;
            Source = (T*)Marshal.AllocHGlobal((IntPtr)blen);

            MemoryBlock.Memmove(source.Source, Source, (ulong)blen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(in NativeConstSpan<T> source)
        {
            var blen = source.Length * sizeof(T);

            Length = source.Length;
            Source = (T*)Marshal.AllocHGlobal((IntPtr)blen);

            MemoryBlock.Memmove(source.DangerousGetPointer(), Source, (ulong)blen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(in arrayref<T> source) : this(source.AsSpan()) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(in arrayrefSegment<T> source) : this(source.AsSpan()) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(SafeArrayref<T> source) : this(source.AsSpan()) { }
        #endregion

        #region indexer & property
        public ref T this[long index] => ref *(Source + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(long index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref this[index];
        }
        #endregion

        #region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> AsSpan() => new NativeSpan<T>(Source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear((byte*)Source, (ulong)Length * (uint)sizeof(T));
        #endregion

        #region explicit implement longerfaces
        ref readonly T IReadOnlyRawContainer<T>.this[long index] => ref this[index];
        ref readonly T IReadOnlyRawContainer<T>.at(long index) => ref at(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long capacity() => Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long size() => Length;
        #endregion

        #region implement -2-
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawContainerBuffer getRawContainerBuffer() => new RawContainerBuffer(Source, Length);

        bool IEquatable<NativeArray<T>>.Equals(NativeArray<T> other) => this == other;
        #endregion

        #region iterator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Source + Length - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Source - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => rend();
        #endregion

        #region override
        public override bool Equals(object obj) => obj is NativeArray<T> other && this == other;
        public override int GetHashCode() => ((IntPtr)Source).GetHashCode() ^ Length.GetHashCode();
        #endregion

        #region dispose
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Marshal.FreeHGlobal((IntPtr)Source);
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReferenceEquals(in NativeArray<T> left, in NativeArray<T> right) => left.Source == right.Source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeArray<T> left, in NativeArray<T> right) =>
            CMathi.CXOR(left.Source, right.Source, left.Length, right.Length) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeArray<T> left, in NativeArray<T> right) =>
            CMathi.CXOR(left.Source, right.Source, left.Length, right.Length) != 0;
        #endregion
    }
}
