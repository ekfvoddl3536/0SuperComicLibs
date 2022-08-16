#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref struct NativeConstSpan<T>
        where T : unmanaged
    {
        private readonly T* _ptr;
        public readonly int Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* source, int length)
        {
            _ptr = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* start, T* end)
        {
            _ptr = start;
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
        public ref readonly T this[int index] => ref *(_ptr + index);
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeConstSpan<T>(_ptr + startIndex, length);
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
                ulong copysize = (ulong)(uint)Length + (uint)sizeof(T);
                Buffer.MemoryCopy(_ptr, pdst, copysize, copysize);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((uint)Length <= (uint)dst.Length)
            {
                ulong copysize = (ulong)(uint)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(_ptr, dst.Source, copysize, copysize);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)Length <= (uint)dst.Length, $"'{nameof(dst)}'");

            ulong copysize = (ulong)(uint)Length + (uint)sizeof(T);
            Buffer.MemoryCopy(_ptr, dst.Source, copysize, copysize);
        }
        #endregion

        #region override
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString()
        {
            if (typeof(T) == typeof(char))
                return new string((char*)_ptr, 0, Length);

            return $"SuperComicLib.Collections.{nameof(NativeConstSpan<T>)}<{nameof(T)}>[{Length}]";
        }
        #endregion

        #region impl methods
        public ref readonly T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
            return ref *(_ptr + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> begin() => new const_iterator<T>(_ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> end() => new const_iterator<T>(_ptr + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rbegin() => new const_reverse_iterator<T>(_ptr + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rend() => new const_reverse_iterator<T>(_ptr - 1);
        #endregion

        #region static memeber
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left._ptr == right._ptr && 
            left.Length == right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeConstSpan<T> left, NativeConstSpan<T> right) => 
            left._ptr != right._ptr || 
            left.Length != right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NativeConstSpan<T>(NativeSpan<T> val) => new NativeConstSpan<T>(val.Source, val.Length);
        #endregion
    }
}
