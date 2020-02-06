using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HashedString : IEquatable<HashedString>, IComparer<HashedString>, IComparable<HashedString>, IComparable
    {
        public static readonly HashedString NULL;

        [FieldOffset(0)]
        public readonly long Value;
        [FieldOffset(0)]
        private readonly int hash;
        [FieldOffset(4)]
        private readonly int length;

        internal HashedString(string data)
        {
            Value = 0;
            hash = data.GetHashCode();
            length = data.Length;
        }

        public HashedString(long value)
        {
            hash = 0;
            length = 0;
            Value = value;
        }

        public static implicit operator HashedString(string op) => op == null ? NULL : new HashedString(op);

        public override bool Equals(object obj) => obj == null ? false : Equals(new HashedString(obj.ToString()));
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString("X");
        public bool Equals(HashedString other) => Value == other.Value;

        public int Compare(HashedString x, HashedString y) => x.Value.CompareTo(y.Value);
        public int CompareTo(HashedString other) => Value.CompareTo(other.Value);
        public int CompareTo(object obj) => Value.CompareTo(obj);

        public static bool operator ==(HashedString a1, HashedString a2) => a1.Equals(a2);
        public static bool operator !=(HashedString a1, HashedString a2) => !a1.Equals(a2);

        public static bool operator <(HashedString left, HashedString right) => left.CompareTo(right) < 0;
        public static bool operator >(HashedString left, HashedString right) => left.CompareTo(right) > 0;

        public static bool operator <=(HashedString left, HashedString right) => left.CompareTo(right) <= 0;
        public static bool operator >=(HashedString left, HashedString right) => left.CompareTo(right) >= 0;
    }
}
