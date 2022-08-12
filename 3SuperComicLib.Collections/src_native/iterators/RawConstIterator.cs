using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawConstIterator<T> : IEquatable<RawConstIterator<T>>, IEquatable<IntPtr>
        where T : unmanaged
    {
        internal readonly T* ptr;

        public RawConstIterator(T* source) => ptr = source;

        #region override
        public override bool Equals(object obj) =>
            obj is IntPtr v0 && (IntPtr)this == v0 ||
            obj is IEquatable<RawConstIterator<T>> comp && comp.Equals(this);
        public override int GetHashCode() => ((IntPtr)ptr).GetHashCode();
        public override string ToString() => ((IntPtr)ptr).ToString();
        #endregion

        #region instance property
        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(ptr + index);
        }
        #endregion

        #region interface impl
        public bool Equals(RawConstIterator<T> other) => this == other;

        public bool Equals(IntPtr other) => ptr == (void*)other;
        #endregion

        #region compare self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RawConstIterator<T> left, RawConstIterator<T> right) => left.ptr == right.ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RawConstIterator<T> left, RawConstIterator<T> right) => left.ptr != right.ptr;
        #endregion

        #region ++/--
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstIterator<T> operator +(RawConstIterator<T> left, int right) => new RawConstIterator<T>(unchecked(left.ptr + right));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstIterator<T> operator -(RawConstIterator<T> left, int right) => new RawConstIterator<T>(unchecked(left.ptr - right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstIterator<T> operator ++(RawConstIterator<T> left) => new RawConstIterator<T>(unchecked(left.ptr + 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstIterator<T> operator --(RawConstIterator<T> left) => new RawConstIterator<T>(unchecked(left.ptr - 1));
        #endregion

        #region explicit/implicit cast operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(RawConstIterator<T> self) => new IntPtr(self.ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(RawConstIterator<T> self) => *self.ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RawConstIterator<T>(RawIterator<T> iter) => new RawConstIterator<T>(iter.Value);
        #endregion
    }
}
