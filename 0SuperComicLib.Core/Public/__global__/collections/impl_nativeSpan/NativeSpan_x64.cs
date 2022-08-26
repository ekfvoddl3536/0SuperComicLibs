#if X64
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct NativeSpan<T>
    {
        public readonly T* Source;
        public readonly long Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* source, long length)
        {
            Source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* start, T* end)
        {
            Source = start;
            Length = end - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, long length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, _iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref T this[long index] => ref *(Source + index);
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(long startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(long startIndex, long length)
        {
            if ((ulong)(startIndex + length) > (ulong)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeSpan<T>(Source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            long len = Length;

            if (len <= 0)
                return Array.Empty<T>();

            T[] res = new T[len];

            fixed (T* pdst = &res[0])
            {
                ulong copysize = (ulong)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, pdst, copysize, copysize);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((ulong)Length <= (ulong)dst.Length)
            {
                ulong copysize = (ulong)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, dst.Source, copysize, copysize);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)Length <= (ulong)dst.Length, $"'{nameof(dst)}'");

            ulong copysize = (ulong)Length * (uint)sizeof(T);
            Buffer.MemoryCopy(Source, dst.Source, copysize, copysize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value) => MemoryBlock.Memset64(Source, (ulong)Length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear64((byte*)Source, (ulong)Length * (uint)sizeof(T));
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(long index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)Length, $"index: {index} / length: {Length}");
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