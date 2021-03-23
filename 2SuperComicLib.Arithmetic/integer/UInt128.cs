using System;
using SuperComicLib.Core;

namespace SuperComicLib.Arithmetic
{
    public readonly struct UInt128 : IEquatable<UInt128>, IComparable<UInt128>, ICustomInteger
    {
        #region constant | static field
        public const int Size32 = 4;
        public const int Size64 = 2;
        public const int Bits = 128;
        public const bool Signed = false;

        public static readonly UInt128 MinValue = default;
        public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);
        #endregion

        internal readonly ulong low;
        internal readonly ulong high;

        #region constructor
        public UInt128(int low, int mid, int high, int flag) :
            this((uint)low, (uint)mid, (uint)high, (uint)flag)
        {
        }

        public UInt128(uint low, uint mid, uint high, uint flag) :
            this(((ulong)mid << 32) | low, ((ulong)flag << 32) | high)
        {
        }

        public UInt128(ulong low, ulong high)
        {
            this.low = low;
            this.high = high;
        }
        #endregion

        #region method
        int ICustomInteger.Length32 => Size32;

        int ICustomInteger.Length64 => Size64;

        bool ICustomInteger.IsUnsigned => !Signed;

        public bool Equals(UInt128 other) => this == other;

        public int CompareTo(UInt128 other) =>
            low == other.low
            ? high.CompareTo(other.high)
            : low.CompareTo(other.low);
        #endregion

        #region override
        public override bool Equals(object obj) =>
            obj is UInt128 v && Equals(v);

        public override int GetHashCode()
        {
            int result = low.GetHashCode();
            return IntHash.Combine(result, high.GetHashCode());
        }

        public unsafe override string ToString()
        {
            fixed (ulong* ptr = &low)
                return BigIntArithmetic.ToString((uint*)ptr, Size32, Signed);
        }
        #endregion

        #region format toString
        public unsafe string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                return ToString();

            char f = char.ToLower(format[0]);
            int count = ToInteger.Positive(format, 1);

            fixed (ulong* ptr = &low)
                return
                    f == 'h'
                    ? BigIntArithmetic.ToHexString((uint*)ptr, Size32, count)
                    : BigIntArithmetic.ToExpToString((uint*)ptr, Size32, count, Signed);
        }
        #endregion

        #region + - * / %
        public unsafe static UInt128 operator +(UInt128 left, UInt128 right)
        {
            BigIntArithmetic.Add((uint*)&left.low, (uint*)&right.low, Size32);
            return left;
        }
        public unsafe static UInt128 operator -(UInt128 left, UInt128 right)
        {
            BigIntArithmetic.Sub((uint*)&left.low, (uint*)&right.low, Size32);
            return left;
        }
        public unsafe static UInt128 operator *(UInt128 left, UInt128 right)
        {
            UInt128 result = default;
            BigIntArithmetic.Mul((uint*)&left.low, (uint*)&right.low, (uint*)&result.low, Size32);
            return result;
        }
        public unsafe static UInt128 operator /(UInt128 left, UInt128 right)
        {
            UInt128 result = default;
            BigIntArithmetic.Div(&left.low, &right.low, &result.low, Size64);
            return result;
        }
        public unsafe static UInt128 operator %(UInt128 left, UInt128 right)
        {
            BigIntArithmetic.Mod(&left.low, &right.low, Size64);
            return left;
        }
        #endregion

        #region bit
        public unsafe static UInt128 operator |(UInt128 left, UInt128 right)
        {
            BigIntArithmetic.OR(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static UInt128 operator ^(UInt128 left, UInt128 right)
        {
            BigIntArithmetic.XOR(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static UInt128 operator &(UInt128 left, UInt128 right)
        {
            BigIntArithmetic.AND(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static UInt128 operator <<(UInt128 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.LSHIFT(&left.low, shift, Size64);
            return left;
        }
        public unsafe static UInt128 operator >>(UInt128 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.RSHIFT(&left.low, shift, Size64);
            return left;
        }
        public unsafe static UInt128 operator ~(UInt128 value)
        {
            BigIntArithmetic.NOT(&value.low, Size64);
            return value;
        }
        #endregion

        #region x -> current
        public static unsafe explicit operator UInt128(float v)
        {
            UInt128 result = default;
            BigIntArithmetic.FormatIEEE754(v, &result.low, Size64);
            return result;
        }

        public static implicit operator UInt128(decimal v)
        {
            int[] vs = decimal.GetBits(v);
            return new UInt128(vs[0], vs[1], vs[2], 0);
        }

        public static implicit operator UInt128(uint v) => new UInt128(v, 0, 0, 0);
        public static implicit operator UInt128(ulong v) => new UInt128(v, 0);
        public static implicit operator UInt128(int v) => new UInt128((uint)v, 0, 0, 0);
        public static implicit operator UInt128(long v) => new UInt128((ulong)v, 0);
        #endregion

        #region current -> x
        public static unsafe explicit operator int(UInt128 v) => (int)v.low;
        public static unsafe explicit operator uint(UInt128 v) => (uint)v.low;
        public static unsafe explicit operator long(UInt128 v) => (long)v.low;
        public static unsafe explicit operator ulong(UInt128 v) => v.low;
        public static unsafe explicit operator decimal(UInt128 v) => new decimal((int)v.low, (int)(v.low >> 32), (int)v.high, false, 0);

        public static unsafe explicit operator float(UInt128 v) => BigIntArithmetic.ToIEEE754(&v.low);
        #endregion

        #region signed <-> unsigned
        public static unsafe explicit operator Int128(in UInt128 v)
        {
            fixed (ulong* ptr = &v.low)
                return *(Int128*)&ptr;
        }
        public static unsafe explicit operator UInt128(in Int128 v)
        {
            fixed (ulong* ptr = &v.low)
                return *(UInt128*)&ptr;
        }
        #endregion

        #region compare
        public static bool operator ==(UInt128 left, UInt128 right) =>
            left.low == right.low &&
            left.high == right.high;
        public static bool operator !=(UInt128 left, UInt128 right) =>
            left.low != right.low ||
            left.high != right.high;
        public static bool operator <(UInt128 left, UInt128 right) =>
            left.high < right.high ||
            left.low < right.low;
        public static bool operator >(UInt128 left, UInt128 right) =>
            left.high > right.high ||
            left.low > right.low;
        public static bool operator <=(UInt128 left, UInt128 right) =>
           left.high <= right.high ||
           left.low <= right.low;
        public static bool operator >=(UInt128 left, UInt128 right) =>
           left.high >= right.high ||
           left.low >= right.low;
        #endregion
    }
}
