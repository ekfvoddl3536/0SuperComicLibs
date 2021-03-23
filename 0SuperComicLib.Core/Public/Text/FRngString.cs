using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Text
{
    public readonly struct FRngString : IEquatable<string>, IEnumerable<char>
    {
        internal readonly string str;
        internal readonly int sidx;
        internal readonly int eidx;

        public FRngString(string str, int sidx, int eidx)
        {
            this.str = str;
            this.sidx = sidx;
            this.eidx = eidx;
        }

        #region method
        public char this[int index] => str[index + sidx];

        public int Length => eidx - sidx;

        public bool IsEmpty => str == null;

        public override bool Equals(object obj) => false;
        public override int GetHashCode()
        {
            int sidx = this.sidx;
            return str.GetFixedHashcode(sidx, eidx - sidx);
        }
        public override string ToString()
        {
            int sidx = this.sidx;
            return str.Substring(sidx, eidx - sidx);
        }

        public IEnumerator<char> GetEnumerator() => new FCharEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region operator
        public static bool operator ==(string left, in FRngString right) => right.Equals(left);
        public static bool operator !=(string left, in FRngString right) => !right.Equals(left);

        public static bool operator ==(in FRngString left, string right) => left.Equals(right);
        public static bool operator !=(in FRngString left, string right) => !left.Equals(right);

        public static FMultiRngString operator +(in FRngString left, in FRngString right) =>
            new FMultiRngString(left.str, left.sidx, left.eidx,
                new FMultiRngString(right.str, right.sidx, right.eidx));
        #endregion

        #region method 2
        public bool Equals(string other)
        {
            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen == dlen && CompareChars(str, other, sidx, dlen);
        }

        public bool StartsWith(string other)
        {
            int sidx = this.sidx;
            int slen = eidx - sidx;

            int dlen = other.Length;
            return slen >= dlen && CompareChars(str, other, sidx, dlen);
        }

        public bool EndsWith(string other)
        {
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
    }
}
