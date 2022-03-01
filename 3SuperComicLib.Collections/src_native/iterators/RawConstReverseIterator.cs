using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawConstReverseIterator<T> : IEquatable<RawConstReverseIterator<T>>, IEquatable<IntPtr>
       where T : unmanaged
    {
        internal readonly T* ptr;

        public RawConstReverseIterator(T* source) => ptr = source;

        #region override
        public override bool Equals(object obj) =>
            obj is IntPtr v0 && (IntPtr)this == v0 ||
            obj is IEquatable<RawConstReverseIterator<T>> comp && comp.Equals(this);
        public override int GetHashCode() => ((IntPtr)ptr).GetHashCode();
        public override string ToString() => ((IntPtr)ptr).ToString();
        #endregion

        #region instance member
        public ref readonly T Item
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawConstIterator<T> Base() => new RawConstIterator<T>(ptr + 1);
        #endregion

        #region interface impl
        public bool Equals(RawConstReverseIterator<T> other) => this == other;

        public bool Equals(IntPtr other) => ptr == (void*)other;
        #endregion

        #region compare self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RawConstReverseIterator<T> left, RawConstReverseIterator<T> right) => left.ptr == right.ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RawConstReverseIterator<T> left, RawConstReverseIterator<T> right) => left.ptr != right.ptr;
        #endregion

        #region ++/--
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstReverseIterator<T> operator +(RawConstReverseIterator<T> left, int right) => new RawConstReverseIterator<T>(unchecked(left.ptr - right));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstReverseIterator<T> operator -(RawConstReverseIterator<T> left, int right) => new RawConstReverseIterator<T>(unchecked(left.ptr + right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstReverseIterator<T> operator ++(RawConstReverseIterator<T> left) => new RawConstReverseIterator<T>(unchecked(left.ptr - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawConstReverseIterator<T> operator --(RawConstReverseIterator<T> left) => new RawConstReverseIterator<T>(unchecked(left.ptr + 1));
        #endregion

        #region explicit/implicit cast operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(RawConstReverseIterator<T> self) => new IntPtr(self.ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(RawConstReverseIterator<T> self) => *self.ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RawConstReverseIterator<T>(RawIterator<T> iter) => new RawConstReverseIterator<T>(iter.Value - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RawConstReverseIterator<T>(RawConstIterator<T> iter) => new RawConstReverseIterator<T>(iter.ptr - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RawConstReverseIterator<T>(RawReverseIterator<T> iter) => new RawConstReverseIterator<T>(iter.Value);
        #endregion
    }
}
