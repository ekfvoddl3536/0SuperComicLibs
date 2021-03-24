using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Text
{
    public readonly struct FRngString : IEquatable<string>, IEquatable<FRngString>, IEquatable<Range>, IEnumerable<char>
    {
        private const string errMsg1 = "can't use negative count";

        internal readonly string str;
        internal readonly int sidx;
        internal readonly int eidx;

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

        public FRngString(string str) : this(str, 0, str.Length)
        {
        }

        #region method
        public char this[int relative_index] => str[relative_index + sidx];

        public FRngString this[int relative_startIndex, int relative_endIndex] =>
            this & new Range(relative_startIndex + sidx, relative_endIndex + sidx);

        public int Length => eidx - sidx;

        public bool IsEmpty => str == null;

        public Range GetRange() => new Range(sidx, eidx);

        public override bool Equals(object obj) => Equals(obj?.ToString());
        public override int GetHashCode()
        {
            if (IsEmpty || Length <= 0)
                return 0;

            int sidx = this.sidx;
            return str.GetFixedHashcode(sidx, eidx - sidx);
        }
        public override string ToString()
        {
            if (IsEmpty || Length <= 0)
                return string.Empty;

            int sidx = this.sidx;
            return str.Substring(sidx, eidx - sidx);
        }

        public IEnumerator<char> GetEnumerator() => new FCharEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region method 2
        public bool Contains(Range range) =>
            range.end >= range.start &&
            sidx >= range.start &&
            eidx <= range.end;

        public bool Equals(Range other) =>
            sidx == other.start &&
            eidx == other.end;

        public bool Equals(FRngString other) => 
            this == other;

        public bool Equals(string other)
        {
            if (other == null)
                return IsEmpty;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen == dlen && CompareChars(str, other, sidx, dlen);
        }

        public bool StartsWith(string other)
        {
            if (other == null)
                return IsEmpty;

            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen >= dlen && CompareChars(str, other, sidx, dlen);
        }

        public bool EndsWith(string other)
        {
            if (other == null)
                return IsEmpty;

            int dlen = other.Length;
            int sidx = eidx - dlen;

            return sidx >= this.sidx && CompareChars(str, other, sidx, dlen);
        }

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

        private static bool CompareChars(string str, string other, int sidx, int dlen)
        {
            for (int dx = 0; dx < dlen; dx++, sidx++)
                if (str[sidx] != other[dx])
                    return false;

            return true;
        }
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

        public FRngString SubString(int relative_startIndex) =>
            SubString(relative_startIndex, Length - relative_startIndex);

        public FRngString RemoveLast(int count) => SubString(0, Length - count);

        public FRngString RemoveHead(int count) => SubString(count);

        /// <summary>
        /// use <see cref="operator ~(in FRngString)"/> instead of this method
        /// </summary>
        public FMultiRngString Exclude() => ~this;

        /// <summary>
        /// use <see cref="operator +(in FRngString, in FRngString)"/> instead of this method
        /// </summary>
        public FMultiRngString Add(in FRngString value) => this + value;

        /// <summary>
        /// use <see cref="op_BitwiseAnd(in FRngString, Range)"/> instead of this method
        /// </summary>
        public FRngString Limit(int relative_startIndex, int relative_endIndex) => Limit(new Range(relative_startIndex, relative_endIndex));

        /// <summary>
        /// use <see cref="op_BitwiseAnd(in FRngString, Range)"/> instead of this method
        /// </summary>
        public FRngString Limit(Range relative_range) => this & relative_range;

        public char[] ToCharArray()
        {
            int len = Math.Max(0, Length);
            
            char[] vs = new char[len];

            int sidx = this.sidx;
            string str = this.str;
            for (int x = 0; x < len;)
                vs[x++] = str[sidx++];

            return vs;
        }
        #endregion

        #region operator
        public static explicit operator FRngString(string value) => new FRngString(value);

        #region eq
        public static bool operator ==(in FRngString left, in FRngString right) =>
            left.str == right.str &&
            left.sidx == right.sidx &&
            left.eidx == right.eidx;
        public static bool operator !=(in FRngString left, in FRngString right) =>
            left.str != right.str ||
            left.sidx != right.sidx ||
            left.eidx != right.eidx;

        public static bool operator ==(string left, in FRngString right) => right.Equals(left);
        public static bool operator !=(string left, in FRngString right) => !right.Equals(left);

        public static bool operator ==(in FRngString left, string right) => left.Equals(right);
        public static bool operator !=(in FRngString left, string right) => !left.Equals(right);

        public static bool operator ==(Range left, in FRngString right) => right.Equals(left);
        public static bool operator !=(Range left, in FRngString right) => !right.Equals(left);

        public static bool operator ==(in FRngString left, Range right) => left.Equals(right);
        public static bool operator !=(in FRngString left, Range right) => !left.Equals(right);
        #endregion

        #region arithmetic
        public static FMultiRngString operator +(in FRngString left, in FRngString right) =>
            new FMultiRngString(left.str, left.sidx, left.eidx,
                new FMultiRngString(right.str, right.sidx, right.eidx));

        public static FMultiRngString operator ~(in FRngString v)
        {
            string str = v.str;
            return new FMultiRngString(str, 0, v.sidx,
                   new FMultiRngString(str, v.eidx, str.Length));
        }

        /// <summary>
        /// Add <paramref name="count"/> to startIndex and endIndex
        /// <para/>
        /// 'returns <see cref="Length"/>' = 'input <see cref="Length"/>' + (2 * <paramref name="count"/>)
        /// </summary>
        public static FRngString operator *(in FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentException(errMsg1);

            string str = left.str;
            return
                new FRngString(
                    str,
                    Math.Max(0, left.sidx - count),
                    Math.Min(str.Length, left.eidx + count));
        }

        /// <summary>
        /// opposite action of '<see cref="operator *(in FRngString, int)"/>'
        /// </summary>
        public static FRngString operator /(in FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentException(errMsg1);

            int eidx = left.eidx - count;

            string str = left.str;
            return
                new FRngString(
                    str,
                    Math.Min(left.sidx + count, eidx),
                    eidx);
        }

        /// <summary>
        /// Move the range forward by <paramref name="count"/>
        /// <para/>
        /// e.g.) ab'cdef'g >> 1 = abc'defg'
        /// </summary>
        public static FRngString operator >>(in FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentException(errMsg1);

            string str = left.str;
            int len = str.Length;

            return
                new FRngString(
                    str,
                    Math.Min(left.sidx + count, len),
                    Math.Min(left.eidx + count, len));
        }

        /// <summary>
        /// Move the range backward by <paramref name="count"/>
        /// /// <para/>
        /// e.g.) xy'zw' << 2 = 'xy'zw
        /// </summary>
        public static FRngString operator <<(in FRngString left, int count)
        {
            if (count < 0)
                throw new ArgumentException(errMsg1);

            string str = left.str;
            int len = str.Length;

            return
                new FRngString(
                    str,
                    Math.Max(left.sidx - count, 0),
                    Math.Max(left.eidx - count, 0));
        }

        /// <summary>
        /// <see href="Constrain"/> to the specified <see cref="Range"/>
        /// </summary>
        public static FRngString operator &(in FRngString left, Range right)
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
                    Math.Max(sidx, ds),
                    Math.Min(eidx, de));
        }
        #endregion
        #endregion
    }
}
