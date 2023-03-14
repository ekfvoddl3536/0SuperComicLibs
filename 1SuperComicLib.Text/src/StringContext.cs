// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

namespace SuperComicLib.Text
{
    public sealed class StringContext : IEquatable<StringContext>, IEquatable<FRngString>, IEnumerable<char>
    {
        internal readonly string str;
        internal int sidx;
        internal int eidx;
        private bool closed;

        public StringContext(FRngString value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            str = value.str;
            sidx = value.sidx;
            eidx = value.eidx;
        }

        public bool IsTrimmed => sidx > 0 || eidx < str.Length;

        public bool IsClosed => closed;

        #region method
        public void Add(FRngString value)
        {
            if (closed)
                throw new InvalidOperationException();
            else if (value is null)
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

        public static bool operator ==(StringContext left, FRngString right) =>
            left.str == right.str &&
            left.sidx == right.sidx &&
            left.eidx == right.eidx;
        public static bool operator !=(StringContext left, FRngString right) =>
            left.str != right.str ||
            left.sidx != right.sidx ||
            left.eidx != right.eidx;

        public static bool operator ==(FRngString left, StringContext right) =>
            left.str == right.str &&
            left.sidx == right.sidx &&
            left.eidx == right.eidx;
        public static bool operator !=(FRngString left, StringContext right) =>
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
