#if X86
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct NativeConstSpan<T>
    {
        public readonly T* Source;
        public readonly int Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* source, int length)
        {
            Source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* start, T* end)
        {
            Source = start;
            Length = (int)(end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, int length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, const_iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref readonly T this[int index] => ref *(Source + index);
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeConstSpan<T>(Source + startIndex, length);
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
                ulong sizeInBytes = (uint)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, pdst, sizeInBytes, sizeInBytes);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((uint)Length <= (uint)dst.Length)
            {
                ulong sizeInBytes = (uint)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, dst.Source, sizeInBytes, sizeInBytes);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)Length <= (ulong)dst.Length, $"'{nameof(dst)}'");

            ulong sizeInBytes = (uint)Length * (uint)sizeof(T);
            Buffer.MemoryCopy(Source, dst.Source, sizeInBytes, sizeInBytes);
        }
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
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