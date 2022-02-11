using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct RawIterator<T> : IEquatable<RawIterator<T>>, IEquatable<RawVoidPtr>, IConvertible<RawVoidPtr>
        where T : unmanaged
    {
        public readonly T* Value;

        internal RawIterator(T* _pvalue) => Value = _pvalue;

        #region override
        public override bool Equals(object obj) =>
            obj is IntPtr v0 && this == v0 ||
            obj is RawIterator<T> p1 && this == p1 ||
            obj is IEquatable<RawIterator<T>> comp && comp.Equals(this);
        public override int GetHashCode() => ((IntPtr)Value).GetHashCode();
        public override string ToString() => ((IntPtr)Value).ToString();
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

        #region compare self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RawIterator<T> left, RawIterator<T> right) => left.Value == right.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RawIterator<T> left, RawIterator<T> right) => left.Value != right.Value;
        #endregion

        #region compare another pointers
        public static bool operator ==(RawIterator<T> left, IntPtr right) => left.Value == right.ToPointer();
        public static bool operator !=(RawIterator<T> left, IntPtr right) => left.Value != right.ToPointer();

        public static bool operator ==(IntPtr left, RawIterator<T> right) => left.ToPointer() == right.Value;
        public static bool operator !=(IntPtr left, RawIterator<T> right) => left.ToPointer() != right.Value;

        public static bool operator ==(RawIterator<T> left, void* right) => left.Value == right;
        public static bool operator !=(RawIterator<T> left, void* right) => left.Value != right;

        public static bool operator ==(void* left, RawIterator<T> right) => left == right.Value;
        public static bool operator !=(void* left, RawIterator<T> right) => left != right.Value;
        #endregion

        #region unary operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator +(RawIterator<T> self) => *self.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator -(RawIterator<T> self) => *self.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator !(RawIterator<T> self) => *self.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator ~(RawIterator<T> self) => *self.Value;
        #endregion

        #region explicit/implicit cast operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(RawIterator<T> self) => new IntPtr(self.Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UIntPtr(RawIterator<T> self) => new UIntPtr(self.Value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T(RawIterator<T> self) => *self.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RawVoidPtr(RawIterator<T> self) => new RawVoidPtr(self.Value);
        #endregion

        #region method
        public ref T Item
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddressInt32() => (int)Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long AddressInt64() => (long)Value;
        #endregion

        #region interface impl
        public bool Equals(RawIterator<T> other) => this == other;

        public bool Equals(RawVoidPtr other) => Value == other.ptr;

        public RawVoidPtr ConvertTo() => this;
        #endregion
    }
}
