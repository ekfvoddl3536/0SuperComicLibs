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

        public NativeSpan(T* source, int length)
        {
            Source = source;
            Length = length;
        }

        #region property
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(Source + index);
        }
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

        public T[] ToArray()
        {
            int i = Length;

            T* ptr = Source + i;
            T[] res = new T[i];

            while (--i >= 0)
                res[i] = *--ptr;

            return res;
        }

        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((uint)Length <= (uint)dst.Length)
            {
                for (T* src_b = Source, end = src_b + Length, dst_b = dst.Source; src_b != end;)
                    *dst_b++ = *src_b++;

                return true;
            }

            return false;
        }

        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)Length <= (uint)dst.Length, $"'{nameof(dst)}'");

            for (T* src_b = Source, end = src_b + Length, dst_b = dst.Source; src_b != end;)
                *dst_b++ = *src_b++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(T value)
        {
            for (T* src = Source, end = src + Length; src != end;)
                *src++ = value;
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

        #region nested structs
        public readonly ref struct iterator
        {
            private readonly T* _ptr;

            internal iterator(T* source) => _ptr = source;

            public ref T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_ptr;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator +(iterator left, int right) => new iterator(left._ptr + right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator -(iterator left, int right) => new iterator(left._ptr - right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator ++(iterator left) => new iterator(left._ptr + 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator --(iterator left) => new iterator(left._ptr - 1);
        }

        public readonly ref struct reverse_iterator
        {
            private readonly T* _ptr;

            internal reverse_iterator(T* source) => _ptr = source;

            public ref T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_ptr;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator +(reverse_iterator left, int right) => new reverse_iterator(left._ptr - right);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator -(reverse_iterator left, int right) => new reverse_iterator(left._ptr + right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator ++(reverse_iterator left) => new reverse_iterator(left._ptr - 1);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static reverse_iterator operator --(reverse_iterator left) => new reverse_iterator(left._ptr + 1);
        }
#pragma warning restore IDE1006 // 명명 규칙
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
    }
}
