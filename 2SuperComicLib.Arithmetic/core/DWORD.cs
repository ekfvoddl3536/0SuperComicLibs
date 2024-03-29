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

using System;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(int))]
    public readonly struct DWORD : IEquatable<DWORD>, IEquatable<int>, IEquatable<uint>, IEquatable<float>, IComparable<DWORD>, IComparable<int>, IComparable<uint>, IComparable<float>, IBitset, IBitCalculable<DWORD>
    {
        [FieldOffset(0)]
        public readonly int signed;
        [FieldOffset(0)]
        public readonly uint unsigned;
        [FieldOffset(0)]
        public readonly float float32;

        public DWORD(int _v)
        {
            float32 = 0;
            unsigned = 0;
            signed = _v;
        }

        public DWORD(uint _v)
        {
            float32 = 0;
            signed = 0;
            unsigned = _v;
        }

        public DWORD(float _v)
        {
            signed = 0;
            unsigned = 0;
            float32 = _v;
        }

        public int CompareTo(float other) => float32.CompareTo(other);
        public int CompareTo(uint other) => unsigned.CompareTo(other);
        public int CompareTo(int other) => signed.CompareTo(other);
        public int CompareTo(DWORD other) => unsigned.CompareTo(other.unsigned);
        public bool Equals(float other) => float32 == other;
        public bool Equals(uint other) => unsigned == other;
        public bool Equals(int other) => signed == other;
        public bool Equals(DWORD other) => unsigned == other.unsigned;

        public override string ToString() => unsigned.ToString("X");
        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) =>
            obj is DWORD r1
            ? Equals(r1)
            : obj is int r2
            ? Equals(r2)
            : obj is uint r3
            ? Equals(r3)
            : obj is float r4 && Equals(r4);

        public int Count() => BitMath.Popcnt(unsigned);

        public DWORD OR(DWORD other) => this | other;
        public DWORD AND(DWORD other) => this & other;
        public DWORD XOR(DWORD other) => this ^ other;
        public DWORD NOT() => ~this;
        public DWORD LSHIFT(int shift) => this << shift;
        public DWORD RSHIFT(int shift) => this >> shift;
        public DWORD ADD(DWORD other) => this + other;
        public DWORD SUB(DWORD other) => this - other;
        public DWORD MUL(DWORD other) => this * other;
        public DWORD DIV(DWORD other) => this / other;
        public DWORD MOD(DWORD other) => this % other;

        public static implicit operator int(DWORD value) => value.signed;
        public static implicit operator uint(DWORD value) => value.unsigned;
        public static implicit operator float(DWORD value) => value.float32;
        public static implicit operator DWORD(int value) => new DWORD(value);
        public static implicit operator DWORD(uint value) => new DWORD(value);
        public static implicit operator DWORD(float value) => new DWORD(value);

        #region 닫기
        public static bool operator ==(DWORD left, DWORD right) => left.unsigned == right.unsigned;
        public static bool operator !=(DWORD left, DWORD right) => left.unsigned != right.unsigned;
        public static bool operator <(DWORD left, DWORD right) => left.unsigned < right.unsigned;
        public static bool operator <=(DWORD left, DWORD right) => left.unsigned <= right.unsigned;
        public static bool operator >(DWORD left, DWORD right) => left.unsigned > right.unsigned;
        public static bool operator >=(DWORD left, DWORD right) => left.unsigned >= right.unsigned;

        public static bool operator ==(DWORD left, float right) => left.float32 == right;
        public static bool operator !=(DWORD left, float right) => left.float32 != right;
        public static bool operator <(DWORD left, float right) => left.float32 < right;
        public static bool operator <=(DWORD left, float right) => left.float32 <= right;
        public static bool operator >(DWORD left, float right) => left.float32 > right;
        public static bool operator >=(DWORD left, float right) => left.float32 >= right;

        public static bool operator ==(float left, DWORD right) => left == right.float32;
        public static bool operator !=(float left, DWORD right) => left != right.float32;
        public static bool operator <(float left, DWORD right) => left < right.float32;
        public static bool operator <=(float left, DWORD right) => left <= right.float32;
        public static bool operator >(float left, DWORD right) => left > right.float32;
        public static bool operator >=(float left, DWORD right) => left >= right.float32;

        public static bool operator ==(DWORD left, int right) => left.signed == right;
        public static bool operator !=(DWORD left, int right) => left.signed != right;
        public static bool operator <(DWORD left, int right) => left.signed < right;
        public static bool operator <=(DWORD left, int right) => left.signed <= right;
        public static bool operator >(DWORD left, int right) => left.signed > right;
        public static bool operator >=(DWORD left, int right) => left.signed >= right;

        public static bool operator ==(int left, DWORD right) => left == right.signed;
        public static bool operator !=(int left, DWORD right) => left != right.signed;
        public static bool operator <(int left, DWORD right) => left < right.signed;
        public static bool operator <=(int left, DWORD right) => left <= right.signed;
        public static bool operator >(int left, DWORD right) => left > right.signed;
        public static bool operator >=(int left, DWORD right) => left >= right.signed;

        public static bool operator ==(DWORD left, uint right) => left.unsigned == right;
        public static bool operator !=(DWORD left, uint right) => left.unsigned != right;
        public static bool operator <(DWORD left, uint right) => left.unsigned < right;
        public static bool operator <=(DWORD left, uint right) => left.unsigned <= right;
        public static bool operator >(DWORD left, uint right) => left.unsigned > right;
        public static bool operator >=(DWORD left, uint right) => left.unsigned >= right;

        public static bool operator ==(uint left, DWORD right) => left == right.unsigned;
        public static bool operator !=(uint left, DWORD right) => left != right.unsigned;
        public static bool operator <(uint left, DWORD right) => left < right.unsigned;
        public static bool operator <=(uint left, DWORD right) => left <= right.unsigned;
        public static bool operator >(uint left, DWORD right) => left > right.unsigned;
        public static bool operator >=(uint left, DWORD right) => left >= right.unsigned;
        #endregion

        #region 산술
        public static DWORD operator +(DWORD left, DWORD right) => new DWORD(left.unsigned + right.unsigned);
        public static DWORD operator -(DWORD left, DWORD right) => new DWORD(left.unsigned - right.unsigned);
        public static DWORD operator *(DWORD left, DWORD right) => new DWORD(left.unsigned * right.unsigned);
        public static DWORD operator /(DWORD left, DWORD right) => new DWORD(left.unsigned / right.unsigned);
        public static DWORD operator %(DWORD left, DWORD right) => new DWORD(left.unsigned % right.unsigned);

        public static DWORD operator +(DWORD left, uint right) => new DWORD(left.unsigned + right);
        public static DWORD operator -(DWORD left, uint right) => new DWORD(left.unsigned - right);
        public static DWORD operator *(DWORD left, uint right) => new DWORD(left.unsigned * right);
        public static DWORD operator /(DWORD left, uint right) => new DWORD(left.unsigned / right);
        public static DWORD operator %(DWORD left, uint right) => new DWORD(left.unsigned % right);

        public static DWORD operator +(uint left, DWORD right) => new DWORD(left + right.unsigned);
        public static DWORD operator -(uint left, DWORD right) => new DWORD(left - right.unsigned);
        public static DWORD operator *(uint left, DWORD right) => new DWORD(left * right.unsigned);
        public static DWORD operator /(uint left, DWORD right) => new DWORD(left / right.unsigned);
        public static DWORD operator %(uint left, DWORD right) => new DWORD(left % right.unsigned);

        public static DWORD operator +(DWORD left, int right) => new DWORD(left.signed + right);
        public static DWORD operator -(DWORD left, int right) => new DWORD(left.signed - right);
        public static DWORD operator *(DWORD left, int right) => new DWORD(left.signed * right);
        public static DWORD operator /(DWORD left, int right) => new DWORD(left.signed / right);
        public static DWORD operator %(DWORD left, int right) => new DWORD(left.signed % right);

        public static DWORD operator +(int left, DWORD right) => new DWORD(left + right.signed);
        public static DWORD operator -(int left, DWORD right) => new DWORD(left - right.signed);
        public static DWORD operator *(int left, DWORD right) => new DWORD(left * right.signed);
        public static DWORD operator /(int left, DWORD right) => new DWORD(left / right.signed);
        public static DWORD operator %(int left, DWORD right) => new DWORD(left % right.signed);

        public static DWORD operator +(DWORD left, float right) => new DWORD(left.float32 + right);
        public static DWORD operator -(DWORD left, float right) => new DWORD(left.float32 - right);
        public static DWORD operator *(DWORD left, float right) => new DWORD(left.float32 * right);
        public static DWORD operator /(DWORD left, float right) => new DWORD(left.float32 / right);
        public static DWORD operator %(DWORD left, float right) => new DWORD(left.float32 % right);

        public static DWORD operator +(float left, DWORD right) => new DWORD(left + right.float32);
        public static DWORD operator -(float left, DWORD right) => new DWORD(left - right.float32);
        public static DWORD operator *(float left, DWORD right) => new DWORD(left * right.float32);
        public static DWORD operator /(float left, DWORD right) => new DWORD(left / right.float32);
        public static DWORD operator %(float left, DWORD right) => new DWORD(left % right.float32);
        #endregion

        #region 비트
        public static DWORD operator <<(DWORD left, int right) => new DWORD(left.unsigned << right);
        public static DWORD operator >>(DWORD left, int right) => new DWORD(left.unsigned >> right);
        public static DWORD operator &(DWORD left, DWORD right) => new DWORD(left.unsigned & right.unsigned);
        public static DWORD operator |(DWORD left, DWORD right) => new DWORD(left.unsigned | right.unsigned);
        public static DWORD operator ^(DWORD left, DWORD right) => new DWORD(left.unsigned ^ right.unsigned);
        public static DWORD operator ~(DWORD left) => new DWORD(~left.unsigned);
        #endregion

        #region 모호함 제거
        public static DWORD operator ++(DWORD left) => new DWORD(left.unsigned + 1);
        public static DWORD operator --(DWORD left) => new DWORD(left.unsigned - 1);
        #endregion
    }
}
