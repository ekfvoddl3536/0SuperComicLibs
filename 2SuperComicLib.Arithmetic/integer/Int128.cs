using System;
using SuperComicLib.Core;

namespace SuperComicLib.Arithmetic
{
    public readonly struct Int128 : IEquatable<Int128>, IComparable<Int128>, ICustomInteger
    {
        #region constant | static field
        public const int Size32 = 4;
        public const int Size64 = 2;
        public const int Bits = 128;
        public const bool Signed = true;

        public static readonly Int128 MinValue = new Int128(0, long.MinValue);
        public static readonly Int128 MaxValue = new Int128(ulong.MaxValue, long.MaxValue);
        #endregion

        internal readonly ulong low;
        internal readonly long high;

        #region constructor
        public Int128(int low, int mid, int high, int flag) :
            this((uint)low, (uint)mid, (uint)high, flag)
        {
        }

        public Int128(uint low, uint mid, uint high, int flag) :
            this(((ulong)mid << 32) | low, ((long)flag << 32) | high)
        {
        }

        public Int128(ulong low, long high)
        {
            this.low = low;
            this.high = high;
        }
        #endregion

        #region method
        int ICustomInteger.Length32 => Size32;

        int ICustomInteger.Length64 => Size64;

        bool ICustomInteger.IsUnsigned => !Signed;

        public bool Equals(Int128 other) => this == other;

        public int CompareTo(Int128 other) =>
            low == other.low
            ? high.CompareTo(other.high)
            : low.CompareTo(other.low);
        #endregion

        #region override
        public override bool Equals(object obj) =>
            obj is Int128 v && Equals(v);

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
        public unsafe static Int128 operator +(Int128 left, Int128 right)
        {
            BigIntArithmetic.Add((uint*)&left.low, (uint*)&right.low, Size32);
            return left;
        }
        public unsafe static Int128 operator -(Int128 left, Int128 right)
        {
            BigIntArithmetic.Sub((uint*)&left.low, (uint*)&right.low, Size32);
            return left;
        }
        public unsafe static Int128 operator *(Int128 left, Int128 right)
        {
            Int128 result = default;
            BigIntArithmetic.Mul((uint*)&left.low, (uint*)&right.low, (uint*)&result.low, Size32);
            return result;
        }
        public unsafe static Int128 operator /(Int128 left, Int128 right)
        {
            Int128 result = default;
            BigIntArithmetic.Div(&left.low, &right.low, &result.low, Size64);
            return result;
        }
        public unsafe static Int128 operator %(Int128 left, Int128 right)
        {
            BigIntArithmetic.Mod(&left.low, &right.low, Size64);
            return left;
        }
        #endregion

        #region neg or pos
        public unsafe static Int128 operator -(Int128 value)
        {
            BigIntArithmetic.NEG((uint*)&value.low, (uint*)&value.low, Size32);
            return value;
        }
        public unsafe static Int128 operator +(Int128 value) => value;
        #endregion

        #region bit
        public unsafe static Int128 operator |(Int128 left, Int128 right)
        {
            BigIntArithmetic.OR(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static Int128 operator ^(Int128 left, Int128 right)
        {
            BigIntArithmetic.XOR(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static Int128 operator &(Int128 left, Int128 right)
        {
            BigIntArithmetic.AND(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static Int128 operator <<(Int128 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.LSHIFT(&left.low, shift, Size64);
            return left;
        }
        public unsafe static Int128 operator >>(Int128 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.RSHIFT(&left.low, shift, Size64);
            return left;
        }
        public unsafe static Int128 operator ~(Int128 value)
        {
            BigIntArithmetic.NOT(&value.low, Size64);
            return value;
        }
        #endregion

        #region x -> current
        public static unsafe explicit operator Int128(float v)
        {
            Int128 result = default;
            if (BigIntArithmetic.FormatIEEE754(v, &result.low, Size64))
                BigIntArithmetic.NEG((uint*)&result.low, (uint*)&result.low, Size32);

            return result;
        }

        public static implicit operator Int128(decimal v)
        {
            int[] vs = decimal.GetBits(v);
            return new Int128(vs[0], vs[1], vs[2], vs[3] & int.MinValue);
        }

        public static implicit operator Int128(uint v) => new Int128(v, 0, 0, 0);
        public static implicit operator Int128(ulong v) => new Int128(v, 0);
        public static implicit operator Int128(int v) => 
            v < 0
            ? new Int128(v, -1, -1, -1)
            : new Int128(v, 0, 0, 0);
        public static implicit operator Int128(long v) => 
            v < 0 
            ? new Int128((ulong)v, -1) 
            : new Int128((ulong)v, 0);
        #endregion

        #region current -> x
        public static unsafe explicit operator int(Int128 v) => (int)v.low;
        public static unsafe explicit operator uint(Int128 v) => (uint)v.low;
        public static unsafe explicit operator long(Int128 v) => (long)v.low;
        public static unsafe explicit operator ulong(Int128 v) => v.low;
        public static unsafe explicit operator decimal(Int128 v) => new decimal((int)v.low, (int)(v.low >> 32), (int)v.high, v.high < 0, 0);

        public static unsafe explicit operator float(Int128 v)
        {
            bool neg;
            if (v.high < 0) // neg
            {
                neg = true;
                BigIntArithmetic.NEG((uint*)&v.low, (uint*)&v.low, Size32);
            }
            else
                neg = false;

            float result = BigIntArithmetic.ToIEEE754(&v.low);
            return
                neg
                ? -result
                : result;
        }
        #endregion

        #region compare
        public static bool operator ==(Int128 left, Int128 right) =>
            left.low == right.low &&
            left.high == right.high;
        public static bool operator !=(Int128 left, Int128 right) =>
            left.low != right.low ||
            left.high != right.high;
        public static bool operator <(Int128 left, Int128 right) =>
            left.high < right.high ||
            left.low < right.low;
        public static bool operator >(Int128 left, Int128 right) =>
            left.high > right.high ||
            left.low > right.low;
        public static bool operator <=(Int128 left, Int128 right) =>
           left.high <= right.high ||
           left.low <= right.low;
        public static bool operator >=(Int128 left, Int128 right) =>
           left.high >= right.high ||
           left.low >= right.low;
        #endregion
    }
}
