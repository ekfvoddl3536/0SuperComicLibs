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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Text
{
    public sealed class FRngString : IEquatable<string>, IEquatable<FRngString>, IEquatable<Range>, IEnumerable<char>
    {
        private const int
            TRIM_HEAD = 0,
            TRIM_TAIL = 1,
            TRIM_BOTH = 2;

        internal readonly string str;
        internal readonly int sidx;
        internal readonly int eidx;

        /// <summary/>
        /// <param name="str">original string</param>
        /// <param name="sidx">start index (include)</param>
        /// <param name="eidx">end index (exclude)</param>
        public FRngString(string str, int sidx, int eidx)
        {
#if DEBUG
            if (str == null)
                throw new ArgumentNullException();

            if (sidx > eidx ||
                eidx > str.Length)
                throw new ArgumentOutOfRangeException();
#endif
            this.str = str;
            this.sidx = sidx;
            this.eidx = eidx;
        }

        public FRngString(string str)
        {
            this.str = str;
            sidx = 0;
            eidx = str.Length;
        }

        #region property
        public char this[int relative_index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get =>
#if DEBUG
                (relative_index += sidx) >= eidx
                ? throw new ArgumentOutOfRangeException(nameof(relative_index))
                : str[relative_index];
#else
                str[relative_index + sidx];
#endif
        }

        public FRngString this[int relative_startIndex, int relative_endIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get =>
#if DEBUG
                (relative_startIndex += sidx) >= (relative_endIndex += sidx) ||
                relative_startIndex >= eidx
                ? throw new ArgumentOutOfRangeException(nameof(relative_startIndex))
                : relative_endIndex >= eidx
                ? throw new ArgumentOutOfRangeException(nameof(relative_endIndex))
                : this & new Range(relative_startIndex, relative_endIndex);
#else
                this & new Range(relative_startIndex + sidx, relative_endIndex + sidx);
#endif
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => eidx - sidx;
        }
        #endregion

        #region method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Range GetRange() => new Range(sidx, eidx);

        public char[] ToCharArray()
        {
            int len = CMath.Max(0, Length);

            char[] vs = new char[len];

            int sidx = this.sidx;
            string str = this.str;
            for (int x = 0; x < len;)
                vs[x++] = str[sidx++];

            return vs;
        }

        public override bool Equals(object obj) => Equals(obj?.ToString());
        public override int GetHashCode()
        {
            if (Length <= 0)
                return 0;

            int sidx = this.sidx;
            return str.GetFixedHashcode(sidx, eidx - sidx);
        }
        public override string ToString()
        {
            if (Length <= 0)
                return string.Empty;

            int sidx = this.sidx;
            return str.Substring(sidx, eidx - sidx);
        }

        public IEnumerator<char> GetEnumerator() => new FCharEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region method 2
        #region contains
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Range range) =>
            range.end >= range.start &&
            sidx >= range.start &&
            eidx <= range.end;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(FRngString other) =>
            IndexOf(other) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(string other) =>
            IndexOf(other) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(char other) =>
            IndexOf(other) >= 0;
        #endregion

        #region equals
        public bool Equals(Range other) =>
            sidx == other.start &&
            eidx == other.end;

        public bool Equals(FRngString other)
        {
            if (other is null)
                return false;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.eidx - other.sidx;
            return slen == dlen && CompareChars(str, other.str, sidx, dlen);
        }

        public bool Equals(string other)
        {
            if (other is null)
                return false;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen == dlen && CompareChars(str, other, sidx, dlen);
        }

        public bool Equals(string other, IEqualityComparer<char> comparer)
        {
            if (comparer == null)
                return Equals(other);
            else if (other == null)
                return false;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen == dlen && CompareChars(str, other, sidx, dlen, comparer);
        }

        public bool Equals(FRngString other, IEqualityComparer<char> comparer)
        {
            if (comparer == null)
                return Equals(other);
            else if (other is null)
                return false;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.eidx - other.sidx;
            return slen == dlen && CompareChars(str, other.str, sidx, dlen, comparer);
        }
        #endregion

        #region other
        public bool StartsWith(string other)
        {
            if (other == null)
                return false;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen >= dlen && CompareChars(str, other, sidx, dlen);
        }

        public bool EndsWith(string other)
        {
            if (other == null)
                return false;

            int dlen = other.Length;
            int sidx = eidx - dlen;

            return sidx >= this.sidx && CompareChars(str, other, sidx, dlen);
        }
        #endregion

        #region indexOf
        public int IndexOf(char value, int startIndex)
        {
            int len = eidx - sidx;
            startIndex += sidx;

            string str = this.str;

            for (; startIndex < len; startIndex++)
                if (str[startIndex] == value)
                    return startIndex - sidx;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(char value) =>
            IndexOf(value, 0);

        public int IndexOf(FRngString value, int startIndex)
        {
            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = value.eidx - value.sidx;

            string str1 = str;
            string str2 = value.str;

            for (; startIndex < slen; startIndex++)
                if (slen - startIndex >= dlen && CompareChars(str1, str2, sidx + startIndex, dlen))
                    return startIndex;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string value, int startIndex) =>
            IndexOf(new FRngString(value), startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(FRngString value) =>
            IndexOf(value, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string value) =>
            IndexOf(new FRngString(value), 0);

        public int IndexOfAny(char[] anyOf, int startIndex, int count)
        {
            int len = eidx - sidx;
            startIndex += sidx;

            string str = this.str;

            for (; startIndex < len; startIndex++)
                for (int x = 0; x < count; x++)
                if (str[startIndex] == anyOf[x])
                    return startIndex - sidx;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOfAny(char[] anyOf, int startIndex) =>
            IndexOfAny(anyOf, startIndex, anyOf.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOfAny(char[] anyOf) =>
            IndexOfAny(anyOf, 0, anyOf.Length);
        #endregion

        #region lastIndexOf
        public int LastIndexOf(char value, int startIndex)
        {
            int sidx = this.sidx;
            string str = this.str;

            for (startIndex += sidx; --startIndex >= sidx;)
                if (str[startIndex] == value)
                    return startIndex - sidx;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(char value) =>
            LastIndexOf(value, 0);

        public int LastIndexOf(FRngString value, int startIndex)
        {
            int sidx = this.sidx;
            int dlen = value.eidx - value.sidx;

            string str1 = str;
            string str2 = value.str;

            int x = startIndex + sidx - dlen;
            for (; x >= sidx; x--)
                if (CompareChars(str1, str2, x, dlen))
                    return x - sidx;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string value, int startIndex) =>
            LastIndexOf(new FRngString(value), startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(FRngString value) =>
            LastIndexOf(value, value.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string value) =>
            LastIndexOf(new FRngString(value), value.Length);

        public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
        {
            int sidx = this.sidx;
            string str = this.str;

            int x = startIndex + sidx;
            while (--x >= sidx)
            {
                for (int y = 0; y < count; y++)
                    if (str[x] == anyOf[y])
                        return x - sidx;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOfAny(char[] anyOf, int startIndex) =>
            LastIndexOfAny(anyOf, startIndex, anyOf.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOfAny(char[] anyOf) =>
            LastIndexOfAny(anyOf, 0, anyOf.Length);
        #endregion
        #endregion

        #region util methods
        public FRngString SubString(int relative_startIndex, int length)
        {
            if (relative_startIndex < 0 || length <= 0)
                throw new ArgumentOutOfRangeException();

            int sidx = this.sidx;
            relative_startIndex += sidx;
            length += sidx;

            int eidx = this.eidx;
            if (relative_startIndex >= eidx || length > eidx)
                throw new ArgumentOutOfRangeException();

            return new FRngString(str, relative_startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString SubString(int relative_startIndex) =>
            SubString(relative_startIndex, Length - relative_startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString RemoveTail(int count) => SubString(0, Length - count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString RemoveHead(int count) => SubString(count);

        public FMultiRngString Insert(int startIndex, FRngString value)
        {
            int sidx = this.sidx;
            int eidx = this.eidx;

            startIndex += sidx;
            if (startIndex >= eidx)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            else if (value is null)
                throw new ArgumentNullException(nameof(value));

            string str = this.str;
            return
                new FMultiRngString(str, sidx, startIndex,
                    new FMultiRngString(value.str, value.sidx, value.eidx,
                        new FMultiRngString(str, startIndex, eidx)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FMultiRngString Insert(int startIndex, string value) => Insert(startIndex, new FRngString(value));

        public FRngString Trim()
        {
            string str = this.str;
            
            int sidx = this.sidx,
                eidx = this.eidx - 1;

            while (sidx <= eidx && char.IsWhiteSpace(str[sidx]))
                sidx++;

            while (eidx >= sidx && char.IsWhiteSpace(str[eidx]))
                eidx--;

            return new FRngString(str, sidx, eidx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString Trim(params char[] trimChars) =>
            TrimHelper(this, trimChars, TRIM_BOTH);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString TrimEnd(params char[] trimChars) =>
            TrimHelper(this, trimChars, TRIM_TAIL);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString TrimHead(params char[] trimChars) =>
            TrimHelper(this, trimChars, TRIM_HEAD);

        public FRngString[] Split(char split)
        {
            List<FRngString> vs = new List<FRngString>();

            int sidx = this.sidx;
            int eidx = this.eidx;

            string str = this.str;

            int pidx = sidx;
            for (; sidx < eidx; sidx++)
                if (str[sidx] == split)
                {
                    vs.Add(new FRngString(str, pidx, sidx));
                    pidx = ++sidx;
                }

            if (pidx < eidx)
                vs.Add(new FRngString(str, pidx, eidx));

            return vs.ToArray();
        }

        #region operator method
        /// <summary>
        /// use <see cref="operator ~(FRngString)"/> instead of this method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FMultiRngString Exclude() => ~this;

        /// <summary>
        /// use <see cref="operator +(FRngString, FRngString)"/> instead of this method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FMultiRngString Add(FRngString value) => this + value;

        /// <summary>
        /// use <see cref="operator +(FRngString)"/> instead of this method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString ToEnd() => +this;

        /// <summary>
        /// use <see cref="operator -(FRngString)"/> instead of this method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString FirstTo() => -this;

        /// <summary>
        /// use <see cref="op_BitwiseAnd(FRngString, Range)"/> instead of this method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRngString Limit(Range relative_range) => this & relative_range;
        #endregion
        #endregion

        #region operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FRngString(string value) => new FRngString(value);

        #region eq
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FRngString left, FRngString right) =>
            left is null
            ? right is null
            : left.Equals(right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FRngString left, FRngString right) =>
            left is null
            ? !(right is null)
            : !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FRngString left, string right) => 
            left is null
            ? right is null
            : left.Equals(right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FRngString left, string right) =>
            left is null
            ? !(right is null)
            : !left.Equals(right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(string left, FRngString right) =>
            right is null
            ? left is null
            : right.Equals(left);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(string left, FRngString right) =>
            right is null
            ? !(left is null)
            : !right.Equals(left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FRngString left, Range right) => !(left is null) && left.Equals(right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FRngString left, Range right) => left is null || !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Range left, FRngString right) => !(right is null) && right.Equals(left);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Range left, FRngString right) => right is null || !right.Equals(left);
        #endregion

        #region arithmetic
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FMultiRngString operator +(FRngString left, FRngString right) =>
            new FMultiRngString(left.str, left.sidx, left.eidx,
                new FMultiRngString(right.str, right.sidx, right.eidx));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FMultiRngString operator +(FRngString left, string right) =>
            new FMultiRngString(left.str, left.sidx, left.eidx,
                new FMultiRngString(right));

        /// <summary>
        /// Delete and concatenate only the current range from the original string
        /// </summary>
        public static FMultiRngString operator ~(FRngString v)
        {
            string str = v.str;
            return new FMultiRngString(str, 0, v.sidx,
                   new FMultiRngString(str, v.eidx, str.Length));
        }

        /// <summary>
        /// Right side string of the truncated range
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FRngString operator +(FRngString v) => new FRngString(v.str, v.eidx, v.str.Length);

        /// <summary>
        /// Left side string of the truncated range
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FRngString operator -(FRngString v) => new FRngString(v.str, 0, v.sidx);

        /// <summary>
        /// Add <paramref name="count"/> to startIndex and endIndex
        /// <para/>
        /// 'returns <see cref="Length"/>' = 'input <see cref="Length"/>' + (2 * <paramref name="count"/>)
        /// <para/>
        /// e.g.) ab'cdef'gh * 1 = a'bcdefg'h
        /// </summary>
        public static FRngString operator *(FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            string str = left.str;
            return
                new FRngString(
                    str,
                    CMath.Max(0, left.sidx - count),
                    CMath.Min(str.Length, left.eidx + count));
        }

        /// <summary>
        /// opposite action of '<see cref="operator *(FRngString, int)"/>'
        /// </summary>
        public static FRngString operator /(FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            int eidx = left.eidx - count;

            string str = left.str;
            return
                new FRngString(
                    str,
                    CMath.Min(left.sidx + count, eidx),
                    eidx);
        }

        /// <summary>
        /// Move the range forward by <paramref name="count"/>
        /// <para/>
        /// e.g.) ab'cdef'g >> 1 = abc'defg'
        /// </summary>
        public static FRngString operator >>(FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            string str = left.str;
            int len = str.Length;

            return
                new FRngString(
                    str,
                    CMath.Min(left.sidx + count, len),
                    CMath.Min(left.eidx + count, len));
        }

        /// <summary>
        /// Move the range backward by <paramref name="count"/>
        /// /// <para/>
        /// e.g.) xy'zw' << 2 = 'xy'zw
        /// </summary>
        public static FRngString operator <<(FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            string str = left.str;
            int len = str.Length;

            return
                new FRngString(
                    str,
                    CMath.Max(left.sidx - count, 0),
                    CMath.Max(left.eidx - count, 0));
        }

        /// <summary>
        /// <see href="Constrain"/> to the specified <see cref="Range"/>
        /// </summary>
        public static FRngString operator &(FRngString left, Range right)
        {
            int ds = right.start,
                de = right.end;

            if (ds > de || ds < 0 || de <= 0)
                return default;

            int sidx = left.sidx,
                eidx = left.eidx;

            if (ds >= eidx || de <= sidx)
                return new FRngString(left.str, sidx, sidx);

            return
                new FRngString(
                    left.str,
                    CMath.Max(sidx, ds),
                    CMath.Min(eidx, de));
        }
        #endregion
        #endregion

        #region private static method
        private static bool CompareChars(string str, string other, int sidx, int dlen)
        {
            for (int dx = 0; dx < dlen; dx++, sidx++)
                if (str[sidx] != other[dx])
                    return false;

            return true;
        }

        private static bool CompareChars(string str, string other, int sidx, int dlen, IEqualityComparer<char> comparer)
        {
            for (int dx = 0; dx < dlen; dx++, sidx++)
                if (!comparer.Equals(str[sidx], other[dx]))
                    return false;

            return true;
        }

        private static FRngString TrimHelper(FRngString value, char[] trimChars, int trimMode)
        {
            string str = value.str;

            int sidx = value.sidx,
                eidx = value.eidx - 1;

            int _len = trimChars.Length;

            char _c;
            int _x;

            if (trimMode != TRIM_TAIL)
                for (; sidx <= eidx; sidx++)
                {
                    _c = str[sidx];
                    _x = 0;

                    while (_x < _len && trimChars[_x++] == _c)
                        ;

                    if (_x == _len)
                        break;
                }

            if (trimMode != TRIM_HEAD)
                for (; eidx >= sidx; eidx--)
                {
                    _c = str[eidx];
                    _x = 0;

                    while (_x < _len && trimChars[_x++] == _c)
                        ;

                    if (_x == _len)
                        break;
                }

            return new FRngString(str, sidx, eidx);
        }
        #endregion
    }
}
