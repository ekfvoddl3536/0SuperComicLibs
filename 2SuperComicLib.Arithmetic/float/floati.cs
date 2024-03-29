﻿// MIT License
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

//namespace SuperComicLib.Arithmetic
//{
//    [StructLayout(LayoutKind.Sequential, Size = 4)]
//    public readonly unsafe struct floati : IComparable, IEquatable<floati>, IComparable<floati>, IConvertible, IFormattable
//    {
//        public const int MaxValue = int.MaxValue;
//        public const int MinValue = int.MinValue + 1;

//        private readonly int _value;

//        #region implement interfaces
//        int IComparable.CompareTo(object obj) =>
//            obj is floati other
//            ? CompareTo(other)
//            : throw new ArgumentException($"invalid type");

//        public int CompareTo(floati other) => _value - *(int*)&other;

//        public bool Equals(floati other) => _value == *(int*)&other;
//        #endregion

//        #region implement IConvertible
//        TypeCode IConvertible.GetTypeCode() => TypeCode.Int32;

//        bool IConvertible.ToBoolean(IFormatProvider provider) => _value > 0;
//        byte IConvertible.ToByte(IFormatProvider provider) => (byte)_value;
//        char IConvertible.ToChar(IFormatProvider provider) => (char)_value;
//        DateTime IConvertible.ToDateTime(IFormatProvider provider) => new DateTime(_value * TimeSpan.TicksPerMillisecond);
//        decimal IConvertible.ToDecimal(IFormatProvider provider) => _value;
//        double IConvertible.ToDouble(IFormatProvider provider) => this;
//        short IConvertible.ToInt16(IFormatProvider provider) => (short)_value;
//        int IConvertible.ToInt32(IFormatProvider provider) => _value;
//        long IConvertible.ToInt64(IFormatProvider provider) => _value;
//        sbyte IConvertible.ToSByte(IFormatProvider provider) => (sbyte)_value;
//        float IConvertible.ToSingle(IFormatProvider provider) => (float)this;
//        string IConvertible.ToString(IFormatProvider provider) => ToString();
//        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => this;
//        ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort)_value;
//        uint IConvertible.ToUInt32(IFormatProvider provider) => (uint)_value;
//        ulong IConvertible.ToUInt64(IFormatProvider provider) => (uint)_value;
//        #endregion

//        #region override methods (instance)
//        public override bool Equals(object obj) => obj is floati other && this == other;
//        public override int GetHashCode() => _value;
//        public override string ToString() => ((double)this).ToString();

//        public string ToString(string format, IFormatProvider provider) => ((double)this).ToString(format, provider);
//        public string ToString(string format) => ((double)this).ToString(format);
//        #endregion

//        #region cast
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static implicit operator int(floati val) => val._value;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static implicit operator double(floati val) => CMath.Max(int.MinValue + 1, val._value) / (double)int.MaxValue;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static implicit operator floati(int val) => *(floati*)&val;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static implicit operator floati(double val)
//        {
//            int temp = (int)(Math.Min(Math.Max(val, -1.0d), 1.0d) * int.MaxValue);
//            return *(floati*)&temp;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static explicit operator float(floati val) => CMath.Max(int.MinValue + 1, val._value) / (float)int.MaxValue;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static explicit operator floati(float val)
//        {
//            int temp = (int)(Math.Min(Math.Max(val, -1.0f), 1.0f) * (double)int.MaxValue);
//            return *(floati*)&temp;
//        }
//        #endregion

//        #region comapre & equals (operator override)
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool operator ==(floati left, floati right) => *(int*)&left == *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool operator !=(floati left, floati right) => *(int*)&left != *(int*)&right;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool operator <(floati left, floati right) => *(int*)&left < *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool operator >(floati left, floati right) => *(int*)&left > *(int*)&right;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool operator <=(floati left, floati right) => *(int*)&left <= *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool operator >=(floati left, floati right) => *(int*)&left >= *(int*)&right;
//        #endregion

//        #region [+ - * / %] operator (override)
//        #region right int
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator +(floati left, int right) => left._value + right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator -(floati left, int right) => left._value - right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator *(floati left, int right) => (int)((left._value * (long)(right + (int)((uint)~right >> 31))) >> 31);
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator /(floati left, int right) => left._value * (int.MaxValue / right);
//        #endregion

//        #region self
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator +(floati left, floati right) => *(int*)&left + *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator -(floati left, floati right) => *(int*)&left - *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator *(floati left, floati right) => left * *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator /(floati left, floati right) => left * (int.MaxValue / *(int*)&right);
//        #endregion
//        #endregion

//        #region bit operator (override)
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator &(floati left, floati right) => *(int*)&left & *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator |(floati left, floati right) => *(int*)&left | *(int*)&right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator ^(floati left, floati right) => *(int*)&left ^ *(int*)&right;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator <<(floati left, int right)  => *(int*)&left << right;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator >>(floati left, int right)  => *(int*)&left >> right;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator -(floati left) => -*(int*)&left;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator +(floati left) => +*(int*)&left;
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static floati operator ~(floati left) => ~*(int*)&left;
//        #endregion
//    }
//}