﻿#if X86
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct NativeSpan<T>
    {
        public readonly T* Source;
        public readonly int Length;

#region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* source, int length)
        {
            Source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* start, T* end)
        {
            Source = start;
            Length = (int)(end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, int length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, _iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
#endregion

#region property
        public ref T this[int index] => ref *(Source + index);
#endregion

#region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeSpan<T>(Source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            int len = Length;

            if (len <= 0)
                return Array.Empty<T>();

            T[] res = new T[len];

            fixed (T* pdst = &res[0])
            {
                ulong copysize = (uint)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, pdst, copysize, copysize);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((uint)Length <= (uint)dst.Length)
            {
                ulong copysize = (uint)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, dst.Source, copysize, copysize);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)Length <= (uint)dst.Length, $"'{nameof(dst)}'");

            ulong copysize = (uint)Length * (uint)sizeof(T);
            Buffer.MemoryCopy(Source, dst.Source, copysize, copysize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value) => MemoryBlock.Memset32(Source, (uint)Length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear32((byte*)Source, (uint)Length * (uint)sizeof(T));
#endregion

#region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Source + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Source - 1);
#endregion

#region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source == right.Source && left.Length == right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source != right.Source || left.Length != right.Length;
#endregion
    }
}
#endif