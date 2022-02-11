using System;

namespace SuperComicLib.Collections
{
    public readonly unsafe struct RawVoidPtr : IEquatable<RawVoidPtr>
    {
        internal readonly void* ptr;

        internal RawVoidPtr(void* value) => ptr = value;

        public bool Equals(RawVoidPtr other) => ptr == other.ptr;

        public override bool Equals(object obj) => obj is RawVoidPtr other && this == other;
        public override int GetHashCode() => ((IntPtr)ptr).GetHashCode();
        public override string ToString() => ((IntPtr)ptr).ToString();

        public static bool operator ==(RawVoidPtr left, RawVoidPtr right) => left.ptr == right.ptr;
        public static bool operator !=(RawVoidPtr left, RawVoidPtr right) => left.ptr != right.ptr;

        public static explicit operator RawVoidPtr(IntPtr other) => new RawVoidPtr(other.ToPointer());
        public static explicit operator RawVoidPtr(UIntPtr other) => new RawVoidPtr(other.ToPointer());
    }
}
