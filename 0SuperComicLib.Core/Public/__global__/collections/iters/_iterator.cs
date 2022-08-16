#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public readonly unsafe ref struct _iterator<T>
        where T : unmanaged
    {
        internal readonly T* _ptr;

        public _iterator(T* source) => _ptr = source;

        public ref T this[int index] => ref *(_ptr + index);

        public ref T value => ref *_ptr;

        public void* UnsafePointerValue => _ptr;

        #region override
        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();
        #endregion

        #region sum & sub
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static _iterator<T> operator +(_iterator<T> left, int right) => new _iterator<T>(left._ptr + right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static _iterator<T> operator -(_iterator<T> left, int right) => new _iterator<T>(left._ptr - right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static _iterator<T> operator ++(_iterator<T> left) => new _iterator<T>(left._ptr + 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static _iterator<T> operator --(_iterator<T> left) => new _iterator<T>(left._ptr - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* operator -(_iterator<T> left, _iterator<T> right) => (void*)(left._ptr - right._ptr);
        #endregion

        #region compare & equals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(_iterator<T> left, _iterator<T> right) => left._ptr == right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(_iterator<T> left, _iterator<T> right) => left._ptr != right._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(_iterator<T> left, _iterator<T> right) => left._ptr <= right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(_iterator<T> left, _iterator<T> right) => left._ptr >= right._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(_iterator<T> left, _iterator<T> right) => left._ptr < right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(_iterator<T> left, _iterator<T> right) => left._ptr > right._ptr;
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator _iterator<T>(T* ptr) => new _iterator<T>(ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T*(_iterator<T> ptr) => ptr._ptr;
        #endregion
    }
}
