#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref struct NativeSpan<T>
        where T : unmanaged
    {
        public readonly T* Source;
        public readonly int Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* source, int length)
        {
            Source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(IRawContainer<T> iter)
        {
            Source = iter.begin().Value;
            Length = iter.size();
        }

        #region property
        public ref T this[int index] => ref *(Source + index);
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex, int length)
        {
            Contract.Requires<ArgumentOutOfRangeException>(
                (uint)(startIndex + length) <= (uint)Length,
                $"'{nameof(startIndex)}' and '{nameof(length)}'");

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
                ulong copysize = (ulong)(uint)Length + (uint)sizeof(T);
                Buffer.MemoryCopy(Source, pdst, copysize, copysize);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((uint)Length <= (uint)dst.Length)
            {
                ulong copysize = (ulong)(uint)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(Source, dst.Source, copysize, copysize);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)Length <= (uint)dst.Length, $"'{nameof(dst)}'");

            ulong copysize = (ulong)(uint)Length * (uint)sizeof(T);
            Buffer.MemoryCopy(Source, dst.Source, copysize, copysize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(T value)
        {
            for (T* src = Source, end = src + Length; src != end;)
                *src++ = value;
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
                return new string((char*)Source, 0, Length);

            return $"SuperComicLib.Collections.{nameof(NativeSpan<T>)}<{nameof(T)}>[{Length}]";
        }
        #endregion

        #region impl methods
#pragma warning disable IDE1006
        public ref T at(int index) 
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public iterator begin() => new iterator(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public iterator end() => new iterator(Source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator rbegin() => new reverse_iterator(Source + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator rend() => new reverse_iterator(Source - 1);
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeSpan<T> left, NativeSpan<T> right) => 
            left.Source == right.Source &&
            left.Length == right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source != right.Source || 
            left.Length != right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NativeConstSpan<T>(NativeSpan<T> val) => new NativeConstSpan<T>(val.Source, val.Length);
        #endregion

        #region nested structs
        public readonly ref struct iterator
        {
            private readonly T* _ptr;

            public iterator(T* source) => _ptr = source;

            public ref T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *(_ptr + index);
            }

            public ref T Value
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
            public static iterator operator +(iterator left, int right) => new iterator(left._ptr + right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator -(iterator left, int right) => new iterator(left._ptr - right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator ++(iterator left) => new iterator(left._ptr + 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator --(iterator left) => new iterator(left._ptr - 1);
            #endregion

            #region compare & equals
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(iterator left, iterator right) => left._ptr == right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(iterator left, iterator right) => left._ptr != right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <=(iterator left, iterator right) => left._ptr <= right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >=(iterator left, iterator right) => left._ptr >= right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <(iterator left, iterator right) => left._ptr < right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >(iterator left, iterator right) => left._ptr > right._ptr;
            #endregion

            #region cast
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator iterator(T* ptr) => new iterator(ptr);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator T*(iterator ptr) => ptr._ptr;
            #endregion
        }

        public readonly ref struct reverse_iterator
        {
            private readonly T* _ptr;

            public reverse_iterator(T* source) => _ptr = source;

            public ref T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *(_ptr - index);
            }

            public ref T Value
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
            public static reverse_iterator operator +(reverse_iterator left, int right) => new reverse_iterator(left._ptr - right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator -(reverse_iterator left, int right) => new reverse_iterator(left._ptr + right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator ++(reverse_iterator left) => new reverse_iterator(left._ptr - 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator --(reverse_iterator left) => new reverse_iterator(left._ptr + 1);
            #endregion

            #region compare & equals
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(reverse_iterator left, reverse_iterator right) => left._ptr == right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(reverse_iterator left, reverse_iterator right) => left._ptr != right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <=(reverse_iterator left, reverse_iterator right) => left._ptr <= right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >=(reverse_iterator left, reverse_iterator right) => left._ptr >= right._ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator <(reverse_iterator left, reverse_iterator right) => left._ptr < right._ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator >(reverse_iterator left, reverse_iterator right) => left._ptr > right._ptr;
            #endregion

            #region cast
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator reverse_iterator(T* ptr) => new reverse_iterator(ptr);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static explicit operator T*(reverse_iterator ptr) => ptr._ptr;
            #endregion
        }
#pragma warning restore IDE1006 // 명명 규칙
        #endregion
    }
}
