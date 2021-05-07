using System;
using System.Runtime.InteropServices;

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

        public LongHashedString(string data)
        {
            Low = High = 0;
            b_hash = Hashcode(data, out g_hash, out compare, length = data.Length);
        }

        public static implicit operator LongHashedString(string op) => op == null ? default : new LongHashedString(op);
        
        public override bool Equals(object obj) => obj != null && Equals(new LongHashedString(obj.ToString()));
        public override int GetHashCode() => CombineHashCode ^ compare ^ length;
        public unsafe override string ToString() =>
            $"{length:X} {compare:X8} {g_hash:X8} {b_hash:X8}";

        public int CombineHashCode => g_hash + b_hash * 1566083941;

        public HashedString ToHashedString() => new HashedString(CombineHashCode, length);

        public bool Equals(LongHashedString other) => Low == other.Low && High == other.High;
        public bool Equals(string other) => this == other;

        public bool HashcodeEquals(LongHashedString other) => b_hash == other.b_hash;
        public bool HashcodeEquals(int other) => b_hash == other;

        public static bool operator ==(LongHashedString a1, LongHashedString a2) => a1.Low == a2.Low && a1.High == a2.High;
        public static bool operator !=(LongHashedString a1, LongHashedString a2) => a1.Low != a2.Low || a1.High != a2.High;

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