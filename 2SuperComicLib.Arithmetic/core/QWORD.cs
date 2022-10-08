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
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(long))]
    public readonly struct QWORD : IEquatable<QWORD>, IEquatable<long>, IEquatable<ulong>, IEquatable<double>, IComparable<QWORD>, IComparable<long>, IComparable<ulong>, IComparable<double>, IBitset, IBitCalculable<QWORD>
    {
        [FieldOffset(0)]
        public readonly long signed;
        [FieldOffset(0)]
        public readonly ulong unsigned;
        [FieldOffset(0)]
        public readonly double float64;

        public QWORD(long _v)
        {
            float64 = 0;
            unsigned = 0;
            signed = _v;
        }

        public QWORD(ulong _v)
        {
            float64 = 0;
            signed = 0;
            unsigned = _v;
        }

        public QWORD(double _v)
        {
            signed = 0;
            unsigned = 0;
            float64 = _v;
        }

        public int CompareTo(double other) => float64.CompareTo(other);
        public int CompareTo(ulong other) => unsigned.CompareTo(other);
        public int CompareTo(long other) => signed.CompareTo(other);
        public int CompareTo(QWORD other) => unsigned.CompareTo(other.unsigned);
        public bool Equals(double other) => float64 == other;
        public bool Equals(ulong other) => unsigned == other;
        public bool Equals(long other) => signed == other;
        public bool Equals(QWORD other) => unsigned == other.unsigned;

        public override string ToString() => unsigned.ToString("X");
        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) =>
            obj is QWORD r1
            ? Equals(r1)
            : obj is long r2
            ? Equals(r2)
            : obj is ulong r3
            ? Equals(r3)
            : obj is double r4 && Equals(r4);

        public static implicit operator long(QWORD value) => value.signed;
        public static implicit operator ulong(QWORD value) => value.unsigned;
        public static implicit operator double(QWORD value) => value.float64;
        public static implicit operator QWORD(int value) => new QWORD(value);
        public static implicit operator QWORD(uint value) => new QWORD(value);
        public static implicit operator QWORD(long value) => new QWORD(value);
        public static implicit operator QWORD(ulong value) => new QWORD(value);
        public static implicit operator QWORD(double value) => new QWORD(value);


        #region 인터페이스 구현
        public int Count()
        {
            ulong c = unsigned - ((unsigned >> 1) & 0x5555_5555_5555_5555ul);
            c = (c & 0x3333_3333_3333_3333ul) + ((c >> 2) & 0x3333_3333_3333_3333ul);
            return (int)unchecked((((c + (c >> 4)) & 0x0F0F_0F0F_0F0F_0F0Ful) * 0x0101_0101_0101_0101ul) >> 56);
        }

        public QWORD OR(QWORD other) => this | other;
        public QWORD AND(QWORD other) => this & other;
        public QWORD XOR(QWORD other) => this ^ other;
        public QWORD NOT() => ~this;
        public QWORD LSHIFT(int shift) => this << shift;
        public QWORD RSHIFT(int shift) => this >> shift;
        public QWORD ADD(QWORD other) => this + other;
        public QWORD SUB(QWORD other) => this - other;
        public QWORD MUL(QWORD other) => this * other;
        public QWORD DIV(QWORD other) => this / other;
        public QWORD MOD(QWORD other) => this % other;
        #endregion

        #region 닫기
        public static bool operator ==(QWORD left, QWORD right) => left.unsigned == right.unsigned;
        public static bool operator !=(QWORD left, QWORD right) => left.unsigned != right.unsigned;
        public static bool operator <(QWORD left, QWORD right) => left.unsigned < right.unsigned;
        public static bool operator <=(QWORD left, QWORD right) => left.unsigned <= right.unsigned;
        public static bool operator >(QWORD left, QWORD right) => left.unsigned > right.unsigned;
        public static bool operator >=(QWORD left, QWORD right) => left.unsigned >= right.unsigned;

        public static bool operator ==(QWORD left, double right) => left.float64 == right;
        public static bool operator !=(QWORD left, double right) => left.float64 != right;
        public static bool operator <(QWORD left, double right) => left.float64 < right;
        public static bool operator <=(QWORD left, double right) => left.float64 <= right;
        public static bool operator >(QWORD left, double right) => left.float64 > right;
        public static bool operator >=(QWORD left, double right) => left.float64 >= right;

        public static bool operator ==(double left, QWORD right) => left == right.float64;
        public static bool operator !=(double left, QWORD right) => left != right.float64;
        public static bool operator <(double left, QWORD right) => left < right.float64;
        public static bool operator <=(double left, QWORD right) => left <= right.float64;
        public static bool operator >(double left, QWORD right) => left > right.float64;
        public static bool operator >=(double left, QWORD right) => left >= right.float64;

        public static bool operator ==(QWORD left, long right) => left.signed == right;
        public static bool operator !=(QWORD left, long right) => left.signed != right;
        public static bool operator <(QWORD left, long right) => left.signed < right;
        public static bool operator <=(QWORD left, long right) => left.signed <= right;
        public static bool operator >(QWORD left, long right) => left.signed > right;
        public static bool operator >=(QWORD left, long right) => left.signed >= right;

        public static bool operator ==(long left, QWORD right) => left == right.signed;
        public static bool operator !=(long left, QWORD right) => left != right.signed;
        public static bool operator <(long left, QWORD right) => left < right.signed;
        public static bool operator <=(long left, QWORD right) => left <= right.signed;
        public static bool operator >(long left, QWORD right) => left > right.signed;
        public static bool operator >=(long left, QWORD right) => left >= right.signed;

        public static bool operator ==(QWORD left, ulong right) => left.unsigned == right;
        public static bool operator !=(QWORD left, ulong right) => left.unsigned != right;
        public static bool operator <(QWORD left, ulong right) => left.unsigned < right;
        public static bool operator <=(QWORD left, ulong right) => left.unsigned <= right;
        public static bool operator >(QWORD left, ulong right) => left.unsigned > right;
        public static bool operator >=(QWORD left, ulong right) => left.unsigned >= right;

        public static bool operator ==(ulong left, QWORD right) => left == right.unsigned;
        public static bool operator !=(ulong left, QWORD right) => left != right.unsigned;
        public static bool operator <(ulong left, QWORD right) => left < right.unsigned;
        public static bool operator <=(ulong left, QWORD right) => left <= right.unsigned;
        public static bool operator >(ulong left, QWORD right) => left > right.unsigned;
        public static bool operator >=(ulong left, QWORD right) => left >= right.unsigned;
        #endregion

        #region 산술
        public static QWORD operator +(QWORD left, QWORD right) => new QWORD(left.unsigned + right.unsigned);
        public static QWORD operator -(QWORD left, QWORD right) => new QWORD(left.unsigned - right.unsigned);
        public static QWORD operator *(QWORD left, QWORD right) => new QWORD(left.unsigned * right.unsigned);
        public static QWORD operator /(QWORD left, QWORD right) => new QWORD(left.unsigned / right.unsigned);
        public static QWORD operator %(QWORD left, QWORD right) => new QWORD(left.unsigned % right.unsigned);

        public static QWORD operator +(QWORD left, ulong right) => new QWORD(left.unsigned + right);
        public static QWORD operator -(QWORD left, ulong right) => new QWORD(left.unsigned - right);
        public static QWORD operator *(QWORD left, ulong right) => new QWORD(left.unsigned * right);
        public static QWORD operator /(QWORD left, ulong right) => new QWORD(left.unsigned / right);
        public static QWORD operator %(QWORD left, ulong right) => new QWORD(left.unsigned % right);

        public static QWORD operator +(ulong left, QWORD right) => new QWORD(left + right.unsigned);
        public static QWORD operator -(ulong left, QWORD right) => new QWORD(left - right.unsigned);
        public static QWORD operator *(ulong left, QWORD right) => new QWORD(left * right.unsigned);
        public static QWORD operator /(ulong left, QWORD right) => new QWORD(left / right.unsigned);
        public static QWORD operator %(ulong left, QWORD right) => new QWORD(left % right.unsigned);

        public static QWORD operator +(QWORD left, long right) => new QWORD(left.signed + right);
        public static QWORD operator -(QWORD left, long right) => new QWORD(left.signed - right);
        public static QWORD operator *(QWORD left, long right) => new QWORD(left.signed * right);
        public static QWORD operator /(QWORD left, long right) => new QWORD(left.signed / right);
        public static QWORD operator %(QWORD left, long right) => new QWORD(left.signed % right);

        public static QWORD operator +(long left, QWORD right) => new QWORD(left + right.signed);
        public static QWORD operator -(long left, QWORD right) => new QWORD(left - right.signed);
        public static QWORD operator *(long left, QWORD right) => new QWORD(left * right.signed);
        public static QWORD operator /(long left, QWORD right) => new QWORD(left / right.signed);
        public static QWORD operator %(long left, QWORD right) => new QWORD(left % right.signed);

        public static QWORD operator +(QWORD left, double right) => new QWORD(left.float64 + right);
        public static QWORD operator -(QWORD left, double right) => new QWORD(left.float64 - right);
        public static QWORD operator *(QWORD left, double right) => new QWORD(left.float64 * right);
        public static QWORD operator /(QWORD left, double right) => new QWORD(left.float64 / right);
        public static QWORD operator %(QWORD left, double right) => new QWORD(left.float64 % right);

        public static QWORD operator +(double left, QWORD right) => new QWORD(left + right.float64);
        public static QWORD operator -(double left, QWORD right) => new QWORD(left - right.float64);
        public static QWORD operator *(double left, QWORD right) => new QWORD(left * right.float64);
        public static QWORD operator /(double left, QWORD right) => new QWORD(left / right.float64);
        public static QWORD operator %(double left, QWORD right) => new QWORD(left % right.float64);
        #endregion

        #region 비트
        public static QWORD operator <<(QWORD left, int right) => new QWORD(left.unsigned << right);
        public static QWORD operator >>(QWORD left, int right) => new QWORD(left.unsigned >> right);
        public static QWORD operator &(QWORD left, QWORD right) => new QWORD(left.unsigned & right.unsigned);
        public static QWORD operator |(QWORD left, QWORD right) => new QWORD(left.unsigned | right.unsigned);
        public static QWORD operator ^(QWORD left, QWORD right) => new QWORD(left.unsigned ^ right.unsigned);
        public static QWORD operator ~(QWORD left) => new QWORD(~left.unsigned);
        #endregion

        #region 기본 자료형
        public static bool operator ==(QWORD left, int right) => (int)left.signed == right;
        public static bool operator !=(QWORD left, int right) => (int)left.signed != right;
        public static bool operator <(QWORD left, int right) => (int)left.signed < right;
        public static bool operator <=(QWORD left, int right) => (int)left.signed <= right;
        public static bool operator >(QWORD left, int right) => (int)left.signed > right;
        public static bool operator >=(QWORD left, int right) => (int)left.signed >= right;

        public static bool operator ==(int left, QWORD right) => left == (int)right.signed;
        public static bool operator !=(int left, QWORD right) => left != (int)right.signed;
        public static bool operator <(int left, QWORD right) => left < (int)right.signed;
        public static bool operator <=(int left, QWORD right) => left <= (int)right.signed;
        public static bool operator >(int left, QWORD right) => left > (int)right.signed;
        public static bool operator >=(int left, QWORD right) => left >= (int)right.signed;

        public static QWORD operator +(QWORD left, int right) => new QWORD(left.signed + right);
        public static QWORD operator -(QWORD left, int right) => new QWORD(left.signed - right);
        public static QWORD operator *(QWORD left, int right) => new QWORD(left.signed * right);
        public static QWORD operator /(QWORD left, int right) => new QWORD(left.signed / right);
        public static QWORD operator %(QWORD left, int right) => new QWORD(left.signed % right);

        public static QWORD operator +(int left, QWORD right) => new QWORD(left + right.signed);
        public static QWORD operator -(int left, QWORD right) => new QWORD(left - right.signed);
        public static QWORD operator *(int left, QWORD right) => new QWORD(left * right.signed);
        public static QWORD operator /(int left, QWORD right) => new QWORD(left / right.signed);
        public static QWORD operator %(int left, QWORD right) => new QWORD(left % right.signed);
        #endregion

        #region 모호함 제거
        public static QWORD operator ++(QWORD left) => new QWORD(left.unsigned + 1);
        public static QWORD operator --(QWORD left) => new QWORD(left.unsigned - 1);
        #endregion
    }
}