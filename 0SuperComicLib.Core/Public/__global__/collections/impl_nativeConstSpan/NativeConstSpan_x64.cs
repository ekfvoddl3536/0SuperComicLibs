#if X64
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct NativeConstSpan<T>
    {
        public readonly T* Source;
        public readonly long Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* source, long length)
        {
            Source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* start, T* end)
        {
            Source = start;
            Length = end - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, long length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, const_iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref readonly T this[long index] => ref *(Source + index);
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(long startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(long startIndex, long length)
        {
            if ((ulong)(startIndex + length) > (ulong)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeConstSpan<T>(Source + startIndex, length);
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
                ulong sizeInBytes = (ulong)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, pdst, sizeInBytes, sizeInBytes);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((ulong)Length <= (ulong)dst.Length)
            {
                ulong sizeInBytes = (ulong)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, dst.Source, sizeInBytes, sizeInBytes);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)Length <= (ulong)dst.Length, $"'{nameof(dst)}'");

            ulong sizeInBytes = (ulong)Length * (uint)sizeof(T);
            Buffer.MemoryCopy(Source, dst.Source, sizeInBytes, sizeInBytes);
        }
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T at(long index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)Length, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> begin() => new const_iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> end() => new const_iterator<T>(Source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rbegin() => new const_reverse_iterator<T>(Source + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rend() => new const_reverse_iterator<T>(Source - 1);
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left.Source == right.Source && left.Length == right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left.Source != right.Source || left.Length != right.Length;
        #endregion
    }
}
#endif