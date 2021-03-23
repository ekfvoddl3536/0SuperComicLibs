using System;
using SuperComicLib.Core;

namespace SuperComicLib.Arithmetic
{
    public readonly struct UInt1024 : IEquatable<UInt1024>, IComparable<UInt1024>, ICustomInteger
    {
        #region constant | static field
        public const int Bits = 1024;
        public const int Size32 = Bits >> 5;
        public const int Size64 = Bits >> 6;
        public const bool Signed = false;

        public static readonly UInt1024 MinValue =
            new UInt1024(
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, long.MinValue);
        public static readonly UInt1024 MaxValue =
            new UInt1024(
                ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
        #endregion

        internal readonly ulong k_00;
        internal readonly ulong k_01;
        internal readonly ulong k_02;
        internal readonly ulong k_03;
        internal readonly ulong k_04;
        internal readonly ulong k_05;
        internal readonly ulong k_06;
        internal readonly ulong k_07;
        internal readonly ulong k_08;
        internal readonly ulong k_09;
        internal readonly ulong k_10;
        internal readonly ulong k_11;
        internal readonly ulong k_12;
        internal readonly ulong k_13;
        internal readonly ulong k_14;
        internal readonly ulong flag;

        #region constructor
        public UInt1024(
            long k_00, long k_01, long k_02, long k_03,
            long k_04, long k_05, long k_06, long k_07,
            long k_08, long k_09, long k_10, long k_11,
            long k_12, long k_13, long k_14, long flag) :
            this((ulong)k_00, (ulong)k_01, (ulong)k_02, (ulong)k_03,
                (ulong)k_04, (ulong)k_05, (ulong)k_06, (ulong)k_07,
                (ulong)k_08, (ulong)k_09, (ulong)k_10, (ulong)k_11,
                (ulong)k_12, (ulong)k_13, (ulong)k_14, (ulong)flag)
        {
        }

        public UInt1024(
            ulong k_00, ulong k_01, ulong k_02, ulong k_03,
            ulong k_04, ulong k_05, ulong k_06, ulong k_07,
            ulong k_08, ulong k_09, ulong k_10, ulong k_11,
            ulong k_12, ulong k_13, ulong k_14, ulong flag)
        {
            this.k_00 = k_00;
            this.k_01 = k_01;
            this.k_02 = k_02;
            this.k_03 = k_03;
            this.k_04 = k_04;
            this.k_05 = k_05;
            this.k_06 = k_06;
            this.k_07 = k_07;
            this.k_08 = k_08;
            this.k_09 = k_09;
            this.k_10 = k_10;
            this.k_11 = k_11;
            this.k_12 = k_12;
            this.k_13 = k_13;
            this.k_14 = k_14;
            this.flag = flag;
        }
        #endregion

        #region method
        int ICustomInteger.Length32 => Size32;

        int ICustomInteger.Length64 => Size64;

        bool ICustomInteger.IsUnsigned => !Signed;

        public bool Equals(UInt1024 other) => this == other;

        public unsafe int CompareTo(UInt1024 other)
        {
            fixed (ulong* ptr = &k_00)
                return BigIntArithmetic.CompareTo_Un(ptr, &other.k_00, Size64);
        }
        #endregion

        #region override
        public override bool Equals(object obj) =>
            obj is UInt1024 v && Equals(v);

        public unsafe override int GetHashCode()
        {
            int result = k_00.GetHashCode();

            fixed (ulong* ptr = &k_01)
                for (int x = Size64 - 2; --x >= 0;)
                    result = IntHash.Combine(result, ptr[x].GetHashCode());

            return IntHash.Combine(result, flag.GetHashCode());
        }

        public unsafe override string ToString()
        {
            fixed (ulong* ptr = &k_00)
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

            fixed (ulong* ptr = &k_00)
                return
                    f == 'h'
                    ? BigIntArithmetic.ToHexString((uint*)ptr, Size32, count)
                    : BigIntArithmetic.ToExpToString((uint*)ptr, Size32, count, Signed);
        }
        #endregion

        #region + - * / %
        public unsafe static UInt1024 operator +(UInt1024 left, UInt1024 right)
        {
            BigIntArithmetic.Add((uint*)&left.k_00, (uint*)&right.k_00, Size32);
            return left;
        }
        public unsafe static UInt1024 operator -(UInt1024 left, UInt1024 right)
        {
            BigIntArithmetic.Sub((uint*)&left.k_00, (uint*)&right.k_00, Size32);
            return left;
        }
        public unsafe static UInt1024 operator *(UInt1024 left, UInt1024 right)
        {
            UInt1024 result = default;
            BigIntArithmetic.Mul((uint*)&left.k_00, (uint*)&right.k_00, (uint*)&result.k_00, Size32);
            return result;
        }
        public unsafe static UInt1024 operator /(UInt1024 left, UInt1024 right)
        {
            UInt1024 result = default;
            BigIntArithmetic.Div(&left.k_00, &right.k_00, &result.k_00, Size64);
            return result;
        }
        public unsafe static UInt1024 operator %(UInt1024 left, UInt1024 right)
        {
            BigIntArithmetic.Mod(&left.k_00, &right.k_00, Size64);
            return left;
        }
        #endregion

        #region bit
        public unsafe static UInt1024 operator |(UInt1024 left, UInt1024 right)
        {
            BigIntArithmetic.OR(&left.k_00, &right.k_00, Size64);
            return left;
        }
        public unsafe static UInt1024 operator ^(UInt1024 left, UInt1024 right)
        {
            BigIntArithmetic.XOR(&left.k_00, &right.k_00, Size64);
            return left;
        }
        public unsafe static UInt1024 operator &(UInt1024 left, UInt1024 right)
        {
            BigIntArithmetic.AND(&left.k_00, &right.k_00, Size64);
            return left;
        }
        public unsafe static UInt1024 operator <<(UInt1024 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.LSHIFT(&left.k_00, shift, Size64);
            return left;
        }
        public unsafe static UInt1024 operator >>(UInt1024 left, int shift)
        {
            if (shift <= 0)
                return left;
            else if (shift >= Bits)
                return default;

            BigIntArithmetic.RSHIFT(&left.k_00, shift, Size64);
            return left;
        }
        public unsafe static UInt1024 operator ~(UInt1024 value)
        {
            BigIntArithmetic.NOT(&value.k_00, Size64);
            return value;
        }
        #endregion

        #region x -> current
        public static unsafe implicit operator UInt1024(float v)
        {
            UInt1024 result = default;
            BigIntArithmetic.FormatIEEE754(v, &result.k_00, Size64);
            return result;
        }

        public static implicit operator UInt1024(decimal v)
        {
            int[] vs = decimal.GetBits(v);
            return new UInt1024(vs[0], vs[1], vs[2], 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, vs[3] & int.MinValue);
        }

        public static implicit operator UInt1024(uint v) => new UInt1024(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static implicit operator UInt1024(ulong v) => new UInt1024(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static implicit operator UInt1024(int v) =>
            v < 0
            ? new UInt1024(v, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1)
            : new UInt1024(v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static implicit operator UInt1024(long v) =>
            v < 0
            ? new UInt1024(v, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1)
            : new UInt1024((ulong)v, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);


        public static implicit operator UInt1024(in Int128 v) =>
            new UInt1024(v.low, (ulong)v.high, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static implicit operator UInt1024(in Int256 v) =>
            new UInt1024(v.low, v.mid, v.high, (ulong)v.flag, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static implicit operator UInt1024(in UInt128 v) =>
            new UInt1024(v.low, v.high, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static implicit operator UInt1024(in UInt256 v) =>
            new UInt1024(v.low, v.mid, v.high, v.flag, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        #endregion

        #region current -> x
        public static unsafe explicit operator int(UInt1024 v) => (int)v.k_00;
        public static unsafe explicit operator uint(UInt1024 v) => (uint)v.k_00;
        public static unsafe explicit operator long(UInt1024 v) => (long)v.k_00;
        public static unsafe explicit operator ulong(UInt1024 v) => v.k_00;
        public static unsafe explicit operator decimal(UInt1024 v) => new decimal((int)v.k_00, (int)(v.k_00 >> 32), (int)v.flag, v.flag < 0, 0);

        public static unsafe explicit operator float(UInt1024 v)
        {
            float result = BigIntArithmetic.ToIEEE754(&v.k_00);
            return result;
        }
        #endregion

        #region signed <-> unsigned
        public static unsafe implicit operator UInt1024(in Int1024 v)
        {
            fixed (ulong* ptr = &v.k_00)
                return *(UInt1024*)&ptr;
        }
        public static unsafe implicit operator Int1024(in UInt1024 v)
        {
            fixed (ulong* ptr = &v.k_00)
                return *(Int1024*)&ptr;
        }
        #endregion

        #region compare
        public static unsafe bool operator ==(UInt1024 left, UInt1024 right) =>
            BigIntArithmetic.Equals(&left.k_00, &right.k_00, Size64);
        public static unsafe bool operator !=(UInt1024 left, UInt1024 right) =>
            BigIntArithmetic.Inequals(&left.k_00, &right.k_00, Size64);
        public static bool operator <(UInt1024 left, UInt1024 right) => left.CompareTo(right) < 0;
        public static bool operator >(UInt1024 left, UInt1024 right) => left.CompareTo(right) > 0;
        public static bool operator <=(UInt1024 left, UInt1024 right) => left.CompareTo(right) <= 0;
        public static bool operator >=(UInt1024 left, UInt1024 right) => left.CompareTo(right) >= 0;
        #endregion
    }
}