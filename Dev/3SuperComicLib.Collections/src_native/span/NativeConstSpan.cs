#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* source, int length)
        {
            _ptr = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(IReadOnlyRawContainer<T> iter)
        {
            _ptr = iter.cbegin().ptr;
            Length = iter.size();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(IRawContainer<T> iter)
        {
            _ptr = iter.begin().Value;
            Length = iter.size();
        }

        #region property
        public ref readonly T this[int index] => ref *(_ptr + index);
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

        #region nested structs
        public readonly ref struct const_iterator
        {
            private readonly T* _ptr;

            public const_iterator(T* source) => _ptr = source;

            public ref readonly T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *(_ptr + index);
            }

            public ref readonly T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_ptr;
            }

            #region override
            [Obsolete("NotSupport")]
            public override int GetHashCode() => throw new NotSupportedException();
            [Obsolete("NotSupport")]
            public override bool Equals(object obj) => throw new NotSupportedException();
            #endregion

            #region sum & sub
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator +(const_iterator left, int right) => new const_iterator(left._ptr + right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator -(const_iterator left, int right) => new const_iterator(left._ptr - right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator ++(const_iterator left) => new const_iterator(left._ptr + 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_iterator operator --(const_iterator left) => new const_iterator(left._ptr - 1);
            #endregion

            #region compare & equals
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(const_iterator left, const_iterator right) => left._ptr == right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(const_iterator left, const_iterator right) => left._ptr != right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <=(const_iterator left, const_iterator right) => left._ptr <= right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >=(const_iterator left, const_iterator right) => left._ptr >= right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <(const_iterator left, const_iterator right) => left._ptr < right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >(const_iterator left, const_iterator right) => left._ptr > right._ptr;
            #endregion

            #region cast
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator const_iterator(T* ptr) => new const_iterator(ptr);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator T*(const_iterator ptr) => ptr._ptr;
            #endregion
        }

        public readonly ref struct const_reverse_iterator
        {
            private readonly T* _ptr;

            public const_reverse_iterator(T* source) => _ptr = source;

            public ref readonly T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *(_ptr - index);
            }

            public ref readonly T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_ptr;
            }

            #region override
            [Obsolete("NotSupport")]
            public override int GetHashCode() => throw new NotSupportedException();
            [Obsolete("NotSupport")]
            public override bool Equals(object obj) => throw new NotSupportedException();
            #endregion

            #region sum & sub
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator +(const_reverse_iterator left, int right) => new const_reverse_iterator(left._ptr - right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator -(const_reverse_iterator left, int right) => new const_reverse_iterator(left._ptr + right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator ++(const_reverse_iterator left) => new const_reverse_iterator(left._ptr - 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static const_reverse_iterator operator --(const_reverse_iterator left) => new const_reverse_iterator(left._ptr + 1);
            #endregion

            #region compare & equals
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(const_reverse_iterator left, const_reverse_iterator right) => left._ptr == right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(const_reverse_iterator left, const_reverse_iterator right) => left._ptr != right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <=(const_reverse_iterator left, const_reverse_iterator right) => left._ptr <= right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >=(const_reverse_iterator left, const_reverse_iterator right) => left._ptr >= right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <(const_reverse_iterator left, const_reverse_iterator right) => left._ptr < right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >(const_reverse_iterator left, const_reverse_iterator right) => left._ptr > right._ptr;
            #endregion

            #region cast
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator const_reverse_iterator(T* ptr) => new const_reverse_iterator(ptr);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator T*(const_reverse_iterator ptr) => ptr._ptr;
            #endregion
        }
#pragma warning restore IDE1006 // 명명 규칙
        #endregion
    }
}
