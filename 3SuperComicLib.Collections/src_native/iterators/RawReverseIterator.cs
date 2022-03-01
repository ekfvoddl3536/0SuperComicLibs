using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawReverseIterator<T> : IEquatable<RawReverseIterator<T>>, IEquatable<IntPtr>
       where T : unmanaged
    {
        public readonly T* Value;

        internal RawReverseIterator(T* _pvalue) => Value = _pvalue;

        #region override
        public override bool Equals(object obj) =>
            obj is IntPtr v0 && (IntPtr)this == v0 ||
            obj is IEquatable<RawReverseIterator<T>> comp && comp.Equals(this);
        public override int GetHashCode() => ((IntPtr)Value).GetHashCode();
        public override string ToString() => ((IntPtr)Value).ToString();
        #endregion

        #region instance member
        public ref T Item
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawIterator<T> Base() => new RawIterator<T>(Value + 1);
        #endregion

        #region interface impl
        public bool Equals(RawReverseIterator<T> other) => this == other;

        public bool Equals(IntPtr other) => Value == (void*)other;
        #endregion

        #region compare self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RawReverseIterator<T> left, RawReverseIterator<T> right) => left.Value == right.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RawReverseIterator<T> left, RawReverseIterator<T> right) => left.Value != right.Value;
        #endregion

        #region ++/--
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawReverseIterator<T> operator +(RawReverseIterator<T> left, int right) => new RawReverseIterator<T>(unchecked(left.Value - right));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawReverseIterator<T> operator -(RawReverseIterator<T> left, int right) => new RawReverseIterator<T>(unchecked(left.Value + right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawReverseIterator<T> operator ++(RawReverseIterator<T> left) => new RawReverseIterator<T>(unchecked(left.Value - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawReverseIterator<T> operator --(RawReverseIterator<T> left) => new RawReverseIterator<T>(unchecked(left.Value + 1));
        #endregion

        #region explicit/implicit cast operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(RawReverseIterator<T> self) => new IntPtr(self.Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(RawReverseIterator<T> self) => *self.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RawReverseIterator<T>(RawIterator<T> iter) => new RawReverseIterator<T>(iter.Value - 1);
        #endregion
    }
}
