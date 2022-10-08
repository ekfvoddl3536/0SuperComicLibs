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

#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Arithmetic
{
    [StructLayout(LayoutKind.Sequential, Size = 2)]
    public readonly unsafe struct halfi : IComparable, IEquatable<halfi>, IComparable<halfi>, IFormattable, IConvertible
    {
        public const short MaxValue = short.MaxValue;
        public const short MinValue = short.MinValue + 1;

        private readonly short _value;

        #region implement interfaces
        int IComparable.CompareTo(object obj) =>
            obj is halfi other
            ? CompareTo(other)
            : throw new ArgumentException($"invalid type");

        public int CompareTo(halfi other) => _value - other._value;

        public bool Equals(halfi other) => _value == other._value;
        #endregion

        #region implement IConvertible
        TypeCode IConvertible.GetTypeCode() => TypeCode.Int16;

        bool IConvertible.ToBoolean(IFormatProvider provider) => _value > 0;
        byte IConvertible.ToByte(IFormatProvider provider) => (byte)_value;
        char IConvertible.ToChar(IFormatProvider provider) => (char)_value;
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => new DateTime(_value * TimeSpan.TicksPerMillisecond);
        decimal IConvertible.ToDecimal(IFormatProvider provider) => _value;
        double IConvertible.ToDouble(IFormatProvider provider) => (float)this;
        short IConvertible.ToInt16(IFormatProvider provider) => _value;
        int IConvertible.ToInt32(IFormatProvider provider) => _value;
        long IConvertible.ToInt64(IFormatProvider provider) => _value;
        sbyte IConvertible.ToSByte(IFormatProvider provider) => (sbyte)_value;
        float IConvertible.ToSingle(IFormatProvider provider) => (float)this;
        string IConvertible.ToString(IFormatProvider provider) => ToString();
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => this;
        ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort)_value;
        uint IConvertible.ToUInt32(IFormatProvider provider) => (uint)_value;
        ulong IConvertible.ToUInt64(IFormatProvider provider) => (uint)_value;
        #endregion

        #region override methods (instance)
        public override bool Equals(object obj) => obj is halfi other && this == other;
        public override int GetHashCode() => _value;
        public override string ToString() => ((float)this).ToString();

        public string ToString(string format, IFormatProvider provider) => ((float)this).ToString(format, provider);
        public string ToString(string format) => ((float)this).ToString(format);
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(halfi val) => val._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(halfi val) => CMath.Max(short.MinValue + 1, val._value) / (float)short.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator halfi(int val) => *(halfi*)&val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator halfi(float val)
        {
            int temp = (int)(Math.Min(Math.Max(val, -1.0f), 1.0f) * short.MaxValue);
            return *(halfi*)&temp;
        }
        #endregion

        #region comapre & equals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(halfi left, halfi right) => left._value == right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(halfi left, halfi right) => left._value != right._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(halfi left, halfi right) => left._value < right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(halfi left, halfi right) => left._value > right._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(halfi left, halfi right) => left._value <= right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(halfi left, halfi right) => left._value >= right._value;
        #endregion

        #region [+ - * / %] operator
        #region right int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator +(halfi left, int right) => left._value + right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator -(halfi left, int right) => left._value - right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator *(halfi left, int right) => (left._value * (right + (int)((uint)~right >> 31))) >> 15;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator /(halfi left, int right) => left._value * (short.MaxValue / right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator %(halfi left, int right) => left._value % right;
        #endregion

        #region self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator +(halfi left, halfi right) => left + (int)right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator -(halfi left, halfi right) => left - (int)right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator *(halfi left, halfi right) => left * (int)right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator /(halfi left, halfi right) => left / (int)right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator %(halfi left, halfi right) => left % (int)right._value;
        #endregion
        #endregion

        #region bit opreator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator &(halfi left, halfi right) => left._value & right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator |(halfi left, halfi right) => left._value | right._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator ^(halfi left, halfi right) => left._value ^ right._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator <<(halfi left, int right) => left._value << right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator >>(halfi left, int right) => left._value >> right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator -(halfi left) => -left._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator +(halfi left) => +left._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static halfi operator ~(halfi left) => ~left._value;
        #endregion
    }
}