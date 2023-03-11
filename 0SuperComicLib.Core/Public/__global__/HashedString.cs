// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct HashedString : IEquatable<HashedString>, IEquatable<string>
    {
        [FieldOffset(0)]
        public readonly long Value;
        [FieldOffset(0)]
        private readonly int hash;
        [FieldOffset(4)]
        private readonly int length;

        public HashedString([DisallowNull] string data)
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

        public HashedString(int hash1, int hash2)
        {
            Value = 0;
            
            hash = hash1;
            length = hash2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HashedString(string op) => op == null ? default : new HashedString(op);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(HashedString str) => str.Value;

        public override bool Equals(object obj) => obj != null && Equals(new HashedString(obj.ToString()));
        public override int GetHashCode() => hash ^ length;
        public override string ToString() => $"(H: 0x{hash:X8} // Length: {length})";

        public bool Equals(HashedString other) => Value == other.Value;
        public bool Equals(string other) => this == other;

        public bool HashcodeEquals(HashedString other) => hash == other.hash;
        public bool HashcodeEquals(int other) => hash == other;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(HashedString a1, HashedString a2) => a1.Value == a2.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(HashedString a1, HashedString a2) => a1.Value != a2.Value;
    }
}