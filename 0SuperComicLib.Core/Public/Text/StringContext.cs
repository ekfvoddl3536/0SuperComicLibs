using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Text
{
    public sealed class StringContext : IEquatable<StringContext>, IEquatable<FRngString>, IEnumerable<char>
    {
        internal readonly string str;
        internal int sidx;
        internal int eidx;
        private bool closed;

        public StringContext(in FRngString value)
        {
            if (value.IsEmpty)
                throw new ArgumentNullException(nameof(value));

            str = value.str;
            sidx = value.sidx;
            eidx = value.eidx;
        }

        public bool IsTrimmed => sidx > 0 || eidx < str.Length;

        public bool IsClosed => closed;

        #region method
        public void Add(in FRngString value)
        {
            if (closed)
                throw new InvalidOperationException();
            else if (value.IsEmpty)
                throw new ArgumentNullException(nameof(value));

            if (str != value.str)
                throw new InvalidOperationException();
            else if (value.sidx < sidx)
                sidx = value.sidx;
            else
                eidx = value.eidx;
        }

        public FRngString EndContext()
        {
            closed = true;
            return new FRngString(str, sidx, eidx);
        }

        public override bool Equals(object obj) =>
            obj is StringContext fsc1 && this == fsc1;
        public override int GetHashCode() => str.GetHashCode() ^ sidx ^ eidx;
        public override string ToString()
        {
            int sidx = this.sidx;
            int eidx = this.eidx;

            string str = this.str;
            return 
                sidx > 0 || eidx < str.Length 
                ? str.Substring(sidx, eidx - sidx) 
                : str;
        }
        #endregion

        #region enumerable & equals
        public IEnumerator<char> GetEnumerator() => new FCharEnumerator(str, sidx, eidx);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [Obsolete("use 'operator ==' instead of this method")]
        public bool Equals(FRngString other) =>
            other.str == str &&
            other.sidx == sidx &&
            other.eidx == eidx;

        public bool Equals(StringContext other) =>
            str == other.str &&
            sidx == other.sidx &&
            eidx == other.eidx;
        #endregion

        #region operator
        public static bool operator ==(StringContext left, StringContext right) =>
            left == right || left.Equals(right);
        public static bool operator !=(StringContext left, StringContext right) =>
            left != right || !left.Equals(right);

        public static bool operator ==(StringContext left, in FRngString right) =>
            left.str == right.str &&
            left.sidx == right.sidx &&
            left.eidx == right.eidx;
        public static bool operator !=(StringContext left, in FRngString right) =>
            left.str != right.str ||
            left.sidx != right.sidx ||
            left.eidx != right.eidx;

        public static bool operator ==(in FRngString left, StringContext right) =>
            left.str == right.str &&
            left.sidx == right.sidx &&
            left.eidx == right.eidx;
        public static bool operator !=(in FRngString left, StringContext right) =>
            left.str != right.str ||
            left.sidx != right.sidx ||
            left.eidx != right.eidx;

        public static bool operator ==(StringContext left, string right) =>
            left.str == right;
        public static bool operator !=(StringContext left, string right) =>
            left.str != right;

        public static bool operator ==(string left, StringContext right) =>
            left == right.str;
        public static bool operator !=(string left, StringContext right) =>
            left != right.str;
        #endregion
    }
}
