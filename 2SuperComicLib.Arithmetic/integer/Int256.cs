using System;
using SuperComicLib.Core;

namespace SuperComicLib.Arithmetic
{
    public readonly struct Int256 : IEquatable<Int256>, IComparable<Int256>, ICustomInteger
    {
        #region constant | static field
        public const int Size32 = 8;
        public const int Size64 = 4;
        public const int Bits = 256;
        public const bool Signed = true;

        public static readonly Int256 MinValue = new Int256(0, 0, 0, long.MinValue);
        public static readonly Int256 MaxValue = new Int256(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, long.MaxValue);
        #endregion

        internal readonly ulong low;
        internal readonly ulong mid;
        internal readonly ulong high;
        internal readonly long flag;

        #region constructor
        public Int256(long low, long mid, long high, long flag) :
            this((ulong)low, (ulong)mid, (ulong)high, flag)
        {
        }

        public Int256(ulong low, ulong mid, ulong high, long flag)
        {
            this.low = low;
            this.mid = mid;
            this.high = high;
            this.flag = flag;
        }
        #endregion

        #region method
        int ICustomInteger.Length32 => Size32;

        int ICustomInteger.Length64 => Size64;

        bool ICustomInteger.IsUnsigned => !Signed;

        public bool Equals(Int256 other) => this == other;

        public unsafe int CompareTo(Int256 other)
        {
            fixed (ulong* ptr = &low)
                return BigIntArithmetic.CompareTo(ptr, &other.low, Size64);
        }
        #endregion

        #region override
        public override bool Equals(object obj) =>
            obj is Int256 v && Equals(v);

        public unsafe override int GetHashCode()
        {
            int result = low.GetHashCode();

            fixed (ulong* ptr = &mid)
                for (int x = Size64 - 2; --x >= 0;)
                    result = IntHash.Combine(result, ptr[x].GetHashCode());

            return IntHash.Combine(result, flag.GetHashCode());
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
        public unsafe static Int256 operator +(Int256 left, Int256 right)
        {
            BigIntArithmetic.Add((uint*)&left.low, (uint*)&right.low, Size32);
            return left;
        }
        public unsafe static Int256 operator -(Int256 left, Int256 right)
        {
            BigIntArithmetic.Sub((uint*)&left.low, (uint*)&right.low, Size32);
            return left;
        }
        public unsafe static Int256 operator *(Int256 left, Int256 right)
        {
            Int256 result = default;
            BigIntArithmetic.Mul((uint*)&left.low, (uint*)&right.low, (uint*)&result.low, Size32);
            return result;
        }
        public unsafe static Int256 operator /(Int256 left, Int256 right)
        {
            Int256 result = default;
            BigIntArithmetic.Div(&left.low, &right.low, &result.low, Size64);
            return result;
        }
        public unsafe static Int256 operator %(Int256 left, Int256 right)
        {
            BigIntArithmetic.Mod(&left.low, &right.low, Size64);
            return left;
        }
        #endregion

        #region neg or pos
        public unsafe static Int256 operator -(Int256 value)
        {
            BigIntArithmetic.NEG((uint*)&value.low, (uint*)&value.low, Size32);
            return value;
        }
        public unsafe static Int256 operator +(Int256 value) => value;
        #endregion

        #region bit
        public unsafe static Int256 operator |(Int256 left, Int256 right)
        {
            BigIntArithmetic.OR(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static Int256 operator ^(Int256 left, Int256 right)
        {
            BigIntArithmetic.XOR(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static Int256 operator &(Int256 left, Int256 right)
        {
            BigIntArithmetic.AND(&left.low, &right.low, Size64);
            return left;
        }
        public unsafe static Int256 operator <<(Int256 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.LSHIFT(&left.low, shift, Size64);
            return left;
        }
        public unsafe static Int256 operator >>(Int256 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.RSHIFT(&left.low, shift, Size64);
            return left;
        }
        public unsafe static Int256 operator ~(Int256 value)
        {
            BigIntArithmetic.NOT(&value.low, Size64);
            return value;
        }
        #endregion

        #region x -> current
        public static unsafe implicit operator Int256(float v)
        {
            Int256 result = default;
            if (BigIntArithmetic.FormatIEEE754(v, &result.low, Size64))
                BigIntArithmetic.NEG((uint*)&result.low, (uint*)&result.low, Size32);

            return result;
        }

        public static implicit operator Int256(decimal v)
        {
            int[] vs = decimal.GetBits(v);
            return new Int256(vs[0], vs[1], vs[2], vs[3] & int.MinValue);
        }

        public static implicit operator Int256(uint v) => new Int256(v, 0, 0, 0);
        public static implicit operator Int256(ulong v) => new Int256(v, 0, 0, 0);
        public static implicit operator Int256(int v) =>
            v < 0
            ? new Int256(v, -1, -1, -1)
            : new Int256(v, 0, 0, 0);
        public static implicit operator Int256(long v) =>
            v < 0
            ? new Int256(v, -1, -1, -1)
            : new Int256((ulong)v, 0, 0, 0);

        // from low bits
        public static implicit operator Int256(in Int128 value) =>
            value.high < 0
            ? new Int256((long)value.low, value.high, -1, -1)
            : new Int256(value.low, (ulong)value.high, 0, 0);

        public static implicit operator Int256(in UInt128 value) =>
            new Int256(value.low, value.high, 0, 0);
        #endregion

        #region current -> x
        public static unsafe explicit operator int(Int256 v) => (int)v.low;
        public static unsafe explicit operator uint(Int256 v) => (uint)v.low;
        public static unsafe explicit operator long(Int256 v) => (long)v.low;
        public static unsafe explicit operator ulong(Int256 v) => v.low;
        public static unsafe explicit operator decimal(Int256 v) => new decimal((int)v.low, (int)(v.low >> 32), (int)v.flag, v.flag < 0, 0);

        public static unsafe explicit operator float(Int256 v)
        {
            bool neg;
            if (v.flag < 0) // neg
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
        public static unsafe bool operator ==(Int256 left, Int256 right) =>
            BigIntArithmetic.Equals(&left.low, &right.low, Size64);
        public static unsafe bool operator !=(Int256 left, Int256 right) =>
            BigIntArithmetic.Inequals(&left.low, &right.low, Size64);
        public static bool operator <(Int256 left, Int256 right) => left.CompareTo(right) < 0;
        public static bool operator >(Int256 left, Int256 right) => left.CompareTo(right) > 0;
        public static bool operator <=(Int256 left, Int256 right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Int256 left, Int256 right) => left.CompareTo(right) >= 0;
        #endregion
    }
}