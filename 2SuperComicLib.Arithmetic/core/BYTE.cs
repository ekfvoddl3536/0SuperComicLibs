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
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(bool))]
    public readonly struct BYTE : IEquatable<BYTE>, IEquatable<sbyte>, IEquatable<byte>, IComparable<BYTE>, IComparable<sbyte>, IComparable<byte>, IBitset, IBitCalculable<BYTE>
    {
        [FieldOffset(0)]
        public readonly sbyte signed;
        [FieldOffset(0)]
        public readonly byte unsigned;
        [FieldOffset(0)]
        public readonly bool boolean;

        public BYTE(sbyte _v)
        {
            boolean = false;
            unsigned = 0;
            signed = _v;
        }

        public BYTE(byte _v)
        {
            boolean = false;
            signed = 0;
            unsigned = _v;
        }

        public int CompareTo(BYTE other) => unsigned.CompareTo(other.unsigned);
        public int CompareTo(byte other) => unsigned.CompareTo(other);
        public int CompareTo(sbyte other) => signed.CompareTo(other);
        public bool Equals(BYTE other) => unsigned == other.unsigned;
        public bool Equals(byte other) => unsigned == other;
        public bool Equals(sbyte other) => signed == other;

        public override bool Equals(object obj) =>
            obj is BYTE r1
            ? Equals(r1)
            : obj is byte r2
            ? Equals(r2)
            : obj is sbyte r3 && Equals(r3);

        public override string ToString() => unsigned.ToString("X");
        public override int GetHashCode() => base.GetHashCode();
        public int Count() => BitMath.Popcnt((uint)unsigned);

        public BYTE OR(BYTE other) => this | other;
        public BYTE AND(BYTE other) => this & other;
        public BYTE XOR(BYTE other) => this ^ other;
        public BYTE NOT() => ~this;
        public BYTE LSHIFT(int shift) => this << shift;
        public BYTE RSHIFT(int shift) => this >> shift;
        public BYTE ADD(BYTE other) => this + other;
        public BYTE SUB(BYTE other) => this - other;
        public BYTE MUL(BYTE other) => this * other;
        public BYTE DIV(BYTE other) => this / other;
        public BYTE MOD(BYTE other) => this % other;

        public static implicit operator bool(BYTE value) => value.boolean;
        public static implicit operator byte(BYTE value) => value.unsigned;
        public static implicit operator sbyte(BYTE value) => value.signed;
        public static implicit operator BYTE(int value) => new BYTE((byte)value);
        public static implicit operator BYTE(byte value) => new BYTE(value);
        public static implicit operator BYTE(sbyte value) => new BYTE(value);

        #region 닫기
        public static bool operator ==(BYTE left, BYTE right) => left.unsigned == right.unsigned;
        public static bool operator !=(BYTE left, BYTE right) => left.unsigned != right.unsigned;
        public static bool operator <(BYTE left, BYTE right) => left.unsigned < right.unsigned;
        public static bool operator <=(BYTE left, BYTE right) => left.unsigned <= right.unsigned;
        public static bool operator >(BYTE left, BYTE right) => left.unsigned > right.unsigned;
        public static bool operator >=(BYTE left, BYTE right) => left.unsigned >= right.unsigned;

        public static bool operator ==(BYTE left, byte right) => left.unsigned == right;
        public static bool operator !=(BYTE left, byte right) => left.unsigned != right;
        public static bool operator <(BYTE left, byte right) => left.unsigned < right;
        public static bool operator <=(BYTE left, byte right) => left.unsigned <= right;
        public static bool operator >(BYTE left, byte right) => left.unsigned > right;
        public static bool operator >=(BYTE left, byte right) => left.unsigned >= right;

        public static bool operator ==(byte left, BYTE right) => left == right.unsigned;
        public static bool operator !=(byte left, BYTE right) => left != right.unsigned;
        public static bool operator <(byte left, BYTE right) => left < right.unsigned;
        public static bool operator <=(byte left, BYTE right) => left <= right.unsigned;
        public static bool operator >(byte left, BYTE right) => left > right.unsigned;
        public static bool operator >=(byte left, BYTE right) => left >= right.unsigned;

        public static bool operator ==(BYTE left, sbyte right) => left.signed == right;
        public static bool operator !=(BYTE left, sbyte right) => left.signed != right;
        public static bool operator <(BYTE left, sbyte right) => left.signed < right;
        public static bool operator <=(BYTE left, sbyte right) => left.signed <= right;
        public static bool operator >(BYTE left, sbyte right) => left.signed > right;
        public static bool operator >=(BYTE left, sbyte right) => left.signed >= right;

        public static bool operator ==(sbyte left, BYTE right) => left == right.signed;
        public static bool operator !=(sbyte left, BYTE right) => left != right.signed;
        public static bool operator <(sbyte left, BYTE right) => left < right.signed;
        public static bool operator <=(sbyte left, BYTE right) => left <= right.signed;
        public static bool operator >(sbyte left, BYTE right) => left > right.signed;
        public static bool operator >=(sbyte left, BYTE right) => left >= right.signed;
        #endregion

        #region 산술
        public static BYTE operator +(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned + right.unsigned));
        public static BYTE operator -(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned - right.unsigned));
        public static BYTE operator *(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned * right.unsigned));
        public static BYTE operator /(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned / right.unsigned));
        public static BYTE operator %(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned % right.unsigned));

        public static BYTE operator +(BYTE left, byte right) => new BYTE((byte)(left.unsigned + right));
        public static BYTE operator -(BYTE left, byte right) => new BYTE((byte)(left.unsigned - right));
        public static BYTE operator *(BYTE left, byte right) => new BYTE((byte)(left.unsigned * right));
        public static BYTE operator /(BYTE left, byte right) => new BYTE((byte)(left.unsigned / right));
        public static BYTE operator %(BYTE left, byte right) => new BYTE((byte)(left.unsigned % right));

        public static BYTE operator +(byte left, BYTE right) => new BYTE((byte)(left + right.unsigned));
        public static BYTE operator -(byte left, BYTE right) => new BYTE((byte)(left - right.unsigned));
        public static BYTE operator *(byte left, BYTE right) => new BYTE((byte)(left * right.unsigned));
        public static BYTE operator /(byte left, BYTE right) => new BYTE((byte)(left / right.unsigned));
        public static BYTE operator %(byte left, BYTE right) => new BYTE((byte)(left % right.unsigned));

        public static BYTE operator +(BYTE left, sbyte right) => new BYTE((sbyte)(left.signed + right));
        public static BYTE operator -(BYTE left, sbyte right) => new BYTE((sbyte)(left.signed - right));
        public static BYTE operator *(BYTE left, sbyte right) => new BYTE((sbyte)(left.signed * right));
        public static BYTE operator /(BYTE left, sbyte right) => new BYTE((sbyte)(left.signed / right));
        public static BYTE operator %(BYTE left, sbyte right) => new BYTE((sbyte)(left.signed % right));

        public static BYTE operator +(sbyte left, BYTE right) => new BYTE((sbyte)(left + right.signed));
        public static BYTE operator -(sbyte left, BYTE right) => new BYTE((sbyte)(left - right.signed));
        public static BYTE operator *(sbyte left, BYTE right) => new BYTE((sbyte)(left * right.signed));
        public static BYTE operator /(sbyte left, BYTE right) => new BYTE((sbyte)(left / right.signed));
        public static BYTE operator %(sbyte left, BYTE right) => new BYTE((sbyte)(left % right.signed));
        #endregion

        #region 비트
        public static BYTE operator <<(BYTE left, int right) => new BYTE((byte)(left.unsigned << right));
        public static BYTE operator >>(BYTE left, int right) => new BYTE((byte)(left.unsigned >> right));
        public static BYTE operator &(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned & right.unsigned));
        public static BYTE operator |(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned | right.unsigned));
        public static BYTE operator ^(BYTE left, BYTE right) => new BYTE((byte)(left.unsigned ^ right.unsigned));
        public static BYTE operator ~(BYTE left) => new BYTE((byte)~left.unsigned);
        #endregion

        #region 기본 자료형
        public static bool operator ==(BYTE left, int right) => left.unsigned == right;
        public static bool operator !=(BYTE left, int right) => left.unsigned != right;
        public static bool operator <(BYTE left, int right) => left.unsigned < right;
        public static bool operator <=(BYTE left, int right) => left.unsigned <= right;
        public static bool operator >(BYTE left, int right) => left.unsigned > right;
        public static bool operator >=(BYTE left, int right) => left.unsigned >= right;

        public static bool operator ==(int left, BYTE right) => left == right.unsigned;
        public static bool operator !=(int left, BYTE right) => left != right.unsigned;
        public static bool operator <(int left, BYTE right) => left < right.unsigned;
        public static bool operator <=(int left, BYTE right) => left <= right.unsigned;
        public static bool operator >(int left, BYTE right) => left > right.unsigned;
        public static bool operator >=(int left, BYTE right) => left >= right.unsigned;

        public static BYTE operator +(BYTE left, int right) => new BYTE((byte)(left.unsigned + right));
        public static BYTE operator -(BYTE left, int right) => new BYTE((byte)(left.unsigned - right));
        public static BYTE operator *(BYTE left, int right) => new BYTE((byte)(left.unsigned * right));
        public static BYTE operator /(BYTE left, int right) => new BYTE((byte)(left.unsigned / right));
        public static BYTE operator %(BYTE left, int right) => new BYTE((byte)(left.unsigned % right));

        public static BYTE operator +(int left, BYTE right) => new BYTE((byte)(left + right.unsigned));
        public static BYTE operator -(int left, BYTE right) => new BYTE((byte)(left - right.unsigned));
        public static BYTE operator *(int left, BYTE right) => new BYTE((byte)(left * right.unsigned));
        public static BYTE operator /(int left, BYTE right) => new BYTE((byte)(left / right.unsigned));
        public static BYTE operator %(int left, BYTE right) => new BYTE((byte)(left % right.unsigned));
        #endregion

        #region 모호함 제거
        public static BYTE operator ++(BYTE left) => new BYTE((byte)(left.unsigned + 1));
        public static BYTE operator --(BYTE left) => new BYTE((byte)(left.unsigned - 1));
        #endregion
    }
}
