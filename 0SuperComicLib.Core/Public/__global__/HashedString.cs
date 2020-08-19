using System;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HashedString : IEquatable<HashedString>, IEquatable<string>
    {
        public static readonly HashedString Empty;

        [FieldOffset(0)]
        public readonly long Value;
        [FieldOffset(0)]
        private readonly int hash;
        [FieldOffset(4)]
        private readonly int length;

        public HashedString(string data)
        {
            Value = 0;
            hash = data.GetFixedHashcode();
            length = data.Length;
        }

        public HashedString(long value)
        {
            hash = 0;
            length = 0;
            Value = value;
        }

        public static implicit operator HashedString(string op) => op == null ? Empty : new HashedString(op);
        public static implicit operator long(HashedString str) => str.Value;

        public override bool Equals(object obj) => obj == null ? false : Equals(new HashedString(obj.ToString()));
        public override int GetHashCode() => hash;
        public override string ToString() => Value.ToString("X");
        public bool Equals(HashedString other) => Value == other.Value;
        public bool Equals(string other) => this == other;

        public bool HashcodeEquals(HashedString other) => hash == other.hash;
        public bool HashcodeEquals(int other) => hash == other;

        public static bool operator ==(HashedString a1, HashedString a2) => a1.Value == a2.Value;
        public static bool operator !=(HashedString a1, HashedString a2) => a1.Value != a2.Value;
    }
}