using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawIterator<T> : IEquatable<RawIterator<T>>, IEquatable<IntPtr>
        where T : unmanaged
    {
        public readonly T* Value;

        internal RawIterator(T* _pvalue) => Value = _pvalue;

        #region override
        public override bool Equals(object obj) =>
            obj is IntPtr v0 && (IntPtr)this == v0 ||
            obj is IEquatable<RawIterator<T>> comp && comp.Equals(this);
        public override int GetHashCode() => ((IntPtr)Value).GetHashCode();
        public override string ToString() => ((IntPtr)Value).ToString();
        #endregion

        #region instance member
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(Value + index);
        }
        #endregion

        #region interface impl
        public bool Equals(RawIterator<T> other) => this == other;

        public bool Equals(IntPtr other) => Value == (void*)other;
        #endregion

        #region compare self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RawIterator<T> left, RawIterator<T> right) => left.Value == right.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RawIterator<T> left, RawIterator<T> right) => left.Value != right.Value;
        #endregion

        #region ++/--
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawIterator<T> operator +(RawIterator<T> left, int right) => new RawIterator<T>(unchecked(left.Value + right));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawIterator<T> operator -(RawIterator<T> left, int right) => new RawIterator<T>(unchecked(left.Value - right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawIterator<T> operator ++(RawIterator<T> left) => new RawIterator<T>(unchecked(left.Value + 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawIterator<T> operator --(RawIterator<T> left) => new RawIterator<T>(unchecked(left.Value - 1));
        #endregion

        #region explicit/implicit cast operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(RawIterator<T> self) => new IntPtr(self.Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(RawIterator<T> self) => *self.Value;
        #endregion
    }
}
