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

namespace SuperComicLib.Arithmetic
{
    public readonly struct UInt128 : IEquatable<UInt128>, IComparable<UInt128>, ICustomInteger
    {
        #region constant | static field
        public const int Size32 = 4;
        public const int Size64 = Size32 >> 1;
        public const int Bits = Size32 << 5;
        public const bool Signed = false;

        public static readonly UInt128 MinValue = new UInt128(0, 0);
        public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);
        #endregion

        public readonly ulong low;
        public readonly ulong high;

        #region constructor
        public UInt128(int sign, int high, int mid, int low) :
            this((uint)sign, (uint)high, (uint)mid, (uint)low)
        {
        }

        public UInt128(uint head, uint high, uint mid, uint low) :
            this(((ulong)head << 32) | high, ((ulong)mid << 32) | low)
        {
        }

        public UInt128(ulong high, ulong low)
        {
            this.high = high;
            this.low = low;
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

        public unsafe override string ToString() => ToString(null);

        public unsafe string ToString(string format)
        {
            fixed (ulong* ptr = &low)
                return BigIntArithmetic.FormatString((uint*)ptr, Size32, Signed, format);
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

        #region neg or pos
        public unsafe static UInt128 operator -(UInt128 value)
        {
            BigIntArithmetic.NEG((uint*)&value.low, (uint*)&value.low, Size32);
            return value;
        }
        public unsafe static UInt128 operator +(UInt128 value) => value;
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
            if (BigIntArithmetic.FormatIEEE754(v, &result.low, Size64))
                BigIntArithmetic.NEG((uint*)&result.low, (uint*)&result.low, Size32);

            return result;
        }

        public static implicit operator UInt128(decimal v)
        {
            int[] vs = decimal.GetBits(v);
            return new UInt128((uint)(vs[3] & int.MinValue), (uint)vs[2], (uint)vs[1], (uint)vs[0]);
        }

        public static implicit operator UInt128(uint v) => new UInt128(0, 0, 0, v);
        public static implicit operator UInt128(ulong v) => new UInt128(0, v);
        public static implicit operator UInt128(int v) =>
            v < 0
            ? new UInt128(uint.MaxValue, uint.MaxValue, uint.MaxValue, (uint)v)
            : new UInt128(0, 0, 0, (uint)v);
        public static implicit operator UInt128(long v) =>
            v < 0
            ? new UInt128(ulong.MaxValue, (ulong)v)
            : new UInt128(0L, (ulong)v);
        #endregion

        #region current -> x
        public static unsafe explicit operator int(UInt128 v) => (int)v.low;
        public static unsafe explicit operator uint(UInt128 v) => (uint)v.low;
        public static unsafe explicit operator long(UInt128 v) => (long)v.low;
        public static unsafe explicit operator ulong(UInt128 v) => v.low;
        public static unsafe explicit operator decimal(UInt128 v) => new decimal((int)v.low, (int)(v.low >> 32), (int)v.high, v.high < 0, 0);

        public static unsafe explicit operator float(UInt128 v)
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
