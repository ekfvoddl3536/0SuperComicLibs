// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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
    public readonly struct LongHashedString : IEquatable<LongHashedString>
    {
        [FieldOffset(0)]
        public readonly ulong Low;
        [FieldOffset(8)]
        public readonly ulong High;
        #region private
        [FieldOffset(0)]
        private readonly int b_hash;
        [FieldOffset(4)]
        private readonly int g_hash;
        [FieldOffset(8)]
        private readonly int compare;
        [FieldOffset(12)]
        private readonly int length;
        #endregion

        public LongHashedString([DisallowNull] string data)
        {
            Low = High = 0;
            b_hash = Hashcode(data, out g_hash, out compare, length = data.Length);
        }

        public override bool Equals(object obj) => obj != null && Equals(new LongHashedString(obj.ToString()));
        public override int GetHashCode() => CombineHashCode ^ compare ^ length;
        public unsafe override string ToString() =>
            $"(Gh: 0x{g_hash:X8} // Bh: 0x{b_hash:X8} // C: 0x{compare:X8} // Length: {length})";

        public int CombineHashCode => g_hash + b_hash * 1566083941;

        public HashedString ToHashedString() => new HashedString(CombineHashCode, length);

        public bool Equals(LongHashedString other) => Low == other.Low && High == other.High;
        public bool Equals(string other) => this == new LongHashedString(other);

        public bool HashcodeEquals(LongHashedString other) => b_hash == other.b_hash;
        public bool HashcodeEquals(int other) => b_hash == other;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LongHashedString(string op) => op == null ? default : new LongHashedString(op);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(LongHashedString a1, LongHashedString a2) => a1.Low == a2.Low && a1.High == a2.High;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(LongHashedString a1, LongHashedString a2) => a1.Low != a2.Low || a1.High != a2.High;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int Hashcode(string data, out int hash1, out int comp, int len)
        {
            comp = 0;
            
            hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;
            
            fixed (char* src = data)
            {
                int* pint = (int*)src;
                
                while (len > 2)
                {
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];

                    comp = (hash2 - hash1) ^ comp;
                    
                    pint += 2;
                    len -= 4;
                }
                
                if (len > 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ *pint;
            }
            
            return hash2;
        }
    }
}