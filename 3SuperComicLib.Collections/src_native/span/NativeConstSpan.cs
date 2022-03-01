using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref struct NativeConstSpan<T>
        where T : unmanaged
    {
        private readonly T* _ptr;
        public readonly int Length;

        public NativeConstSpan(T* source, int length)
        {
            _ptr = source;
            Length = length;
        }

        #region property
        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(_ptr + index);
        }
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex, int length)
        {
            Contract.Requires<ArgumentOutOfRangeException>(
                (uint)(startIndex + length) <= (uint)Length,
                $"'{nameof(startIndex)}' and '{nameof(length)}'");

            return new NativeConstSpan<T>(_ptr + startIndex, length);
        }

        public T[] ToArray()
        {
            int i = Length;

            T* ptr = _ptr + i;
            T[] res = new T[i];

            while (--i >= 0)
                res[i] = *--ptr;

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((uint)Length <= (uint)dst.Length)
            {
                for (T* src_b = _ptr, end = src_b + Length, dst_b = dst.Source; src_b != end;)
                    *dst_b++ = *src_b++;

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)Length <= (uint)dst.Length, $"'{nameof(dst)}'");

            for (T* src_b = _ptr, end = src_b + Length, dst_b = dst.Source; src_b != end;)
                *dst_b++ = *src_b++;
        }
        #endregion

        #region override
#pragma warning disable CS0809
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();
#pragma warning restore CS0809 // 사용되지 않는 멤버가 사용되는 멤버를 재정의합니다.

        public override string ToString()
        {
            if (typeof(T) == typeof(char))
                return new string((char*)_ptr, 0, Length);

            return $"SuperComicLib.Collections.{nameof(NativeConstSpan<T>)}<{nameof(T)}>[{Length}]";
        }
        #endregion

        #region impl methods
#pragma warning disable IDE1006
        public ref readonly T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
            return ref *(_ptr + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator begin() => new const_iterator(_ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator end() => new const_iterator(_ptr + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator rbegin() => new const_reverse_iterator(_ptr + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator rend() => new const_reverse_iterator(_ptr - 1);
        #endregion

        #region nested structs
        public readonly ref struct const_iterator
        {
            private readonly T* _ptr;

            internal const_iterator(T* source) => _ptr = source;

            public ref readonly T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_ptr;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator +(const_iterator left, int right) => new const_iterator(left._ptr + right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator -(const_iterator left, int right) => new const_iterator(left._ptr - right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator ++(const_iterator left) => new const_iterator(left._ptr + 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator --(const_iterator left) => new const_iterator(left._ptr - 1);
        }

        public readonly ref struct const_reverse_iterator
        {
            private readonly T* _ptr;

            internal const_reverse_iterator(T* source) => _ptr = source;

            public ref readonly T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_ptr;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator +(const_reverse_iterator left, int right) => new const_reverse_iterator(left._ptr - right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator -(const_reverse_iterator left, int right) => new const_reverse_iterator(left._ptr + right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator ++(const_reverse_iterator left) => new const_reverse_iterator(left._ptr - 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator --(const_reverse_iterator left) => new const_reverse_iterator(left._ptr + 1);
        }
#pragma warning restore IDE1006 // 명명 규칙
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
        #endregion
    }
}
