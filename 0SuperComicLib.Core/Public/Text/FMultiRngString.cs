using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SuperComicLib.Core;

namespace SuperComicLib.Text
{
    public sealed class FMultiRngString : IEquatable<string>, IEnumerable<char>, IDisposable
    {
        private string str;
        private int sidx;
        private int eidx;
        private FMultiRngString next;

        internal FMultiRngString(string str, int sidx, int eidx, FMultiRngString next) : this(str, sidx, eidx) =>
            this.next = next;

        internal FMultiRngString(string str, int sidx, int eidx)
        {
            this.str = str;
            this.sidx = sidx;
            this.eidx = eidx;
        }

        public FMultiRngString(in FRngString value) : this(value.str, value.sidx, value.eidx)
        {
        }

        public FMultiRngString(string value) : this(value, 0, value.Length) 
        { 
        }

        public int Length => eidx - sidx;

        #region method
        /// <summary>
        /// Caculate total nodes count & string length
        /// </summary>
        /// <returns>nodes count, total string length</returns>
        public void FullSize(out int nodes, out int totalLength)
        {
            int sidx = this.sidx;
            int eidx = this.eidx;
            
            int length = eidx - sidx;
            int cnt = 0;

            FMultiRngString t = next;
            for (; t != null; t = t.next)
            {
                cnt++;
                length += t.eidx - t.sidx;
            }

            nodes = cnt;
            totalLength = length;
        }

        public void ApeendTo(StringBuilder sb)
        {
            string str = this.str;
            int sidx = this.sidx;
            int eidx = this.eidx;

            while (sidx < eidx)
                sb.Append(str[sidx++]);
        }

        public void AddLast(in FRngString value)
        {
            if (value.IsEmpty)
                throw new ArgumentNullException(nameof(value));

            LastNode().next =
                new FMultiRngString(value.str, value.sidx, value.eidx);
        }

        public void AddLast(string value) => AddLast(new FRngString(value, 0, value.Length));
        #endregion

        #region to string
        public string ToString(char joinCharacter)
        {
            FullSize(out int nodes, out int len);
            
            StringBuilder sb = new StringBuilder(nodes + len);
            ApeendTo(sb);

            FMultiRngString t = next;
            for (; t != null; t = t.next)
            {
                sb.Append(joinCharacter);
                t.ApeendTo(sb);
            }

            return sb.ToString();
        }

        public override string ToString() => ToString(' ');
        #endregion

        #region compare
        public bool ValueEquals(FMultiRngString other)
        {
            if (this == other)
                return true;

            FMultiRngString left = this;
            do
            {
                if (left.next == null != (other.next == null))
                    return false;

                if (left.str != other.str ||
                    left.sidx != other.sidx ||
                    left.eidx != other.eidx)
                    return false;

                left = left.next;
                other = other.next;
            }
            while (left != null);

            return true;
        }

        public bool Equals(string other, char joinCharacter)
        {
            Enumerator fe = new Enumerator(this, joinCharacter);
            CharEnumerator ce = other.GetEnumerator();

            do
                if (fe.MoveNext())
                {
                    if (!ce.MoveNext())
                        break;
                }
                else if (ce.MoveNext())
                    break;
                else
                {
                    fe.Dispose();
                    ce.Dispose();

                    return true;
                }
            while (fe.Current == ce.Current);

            fe.Dispose();
            ce.Dispose();

            return false;
        }

        public bool Equals(string other) => Equals(other, ' ');
        #endregion

        #region add
        private FMultiRngString LastNode()
        {
            FMultiRngString sel = this;
            FMultiRngString n = next;

            for (; n != null; n = n.next)
                sel = n;

            return sel;
        }

        public static FMultiRngString operator +(FMultiRngString left, in FRngString right)
        {
            left.AddLast(right);
            return left;
        }

        public static FMultiRngString operator +(FMultiRngString left, string right)
        {
            left.AddLast(right);
            return left;
        }
        #endregion

        #region override
        public override bool Equals(object obj) =>
            obj is FMultiRngString fmrs && ValueEquals(fmrs) ||
            obj is string str && Equals(str);
        public override int GetHashCode()
        {
            int sidx = this.sidx;
            int hashcode = str.GetFixedHashcode(sidx, eidx - sidx);

            for (FMultiRngString n = next; n != null; n = n.next)
            {
                sidx = n.sidx;
                hashcode = IntHash.CombineMOD(hashcode, str.GetFixedHashcode(sidx, n.eidx - sidx));
            }

            return hashcode;
        }
        #endregion

        #region enumerable
        public IEnumerator<char> GetEnumerator(char joinCharacter) => new Enumerator(this, joinCharacter);

        public IEnumerator<char> GetEnumerator() => GetEnumerator(' ');

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region operator
        public static bool operator ==(FMultiRngString left, string right) => left.Equals(right);
        public static bool operator !=(FMultiRngString left, string right) => !left.Equals(right);

        public static bool operator ==(string left, FMultiRngString right) => right.Equals(left);
        public static bool operator !=(string left, FMultiRngString right) => !right.Equals(left);
        #endregion

        #region dispose (optional)
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                str = null;
                sidx = 0;
                eidx = 0;
            }

            // 모든 레퍼런스를 삭제
            FMultiRngString p = this;
            while (p.next != null)
            {
                FMultiRngString n = p.next;
                p.next = null;
                
                p = n;
            }
        }

        ~FMultiRngString()
        {
            Dispose(false);
        }

        /// <summary>
        /// [Optional]
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region inner class
        private struct Enumerator : IEnumerator<char>
        {
            private readonly char joinCharacter;
            private FMultiRngString first;
            private FMultiRngString curr;
            private string str;
            private int cidx;
            private int eidx;

            public Enumerator(FMultiRngString first, char joinCharacter)
            {
                this.first = first;
                this.joinCharacter = joinCharacter;

                str = first.str;
                cidx = first.sidx - 1;
                eidx = first.eidx;

                curr = first;
            }

            public char Current => 
                str == null
                ? joinCharacter
                : str[cidx];

            public bool MoveNext()
            {
                if (str == null)
                    str = curr.str;
                else if (++cidx < eidx)
                {
                    FMultiRngString next = curr.next;
                    if (next != null)
                    {
                        curr = next;

                        str = null;
                        cidx = next.sidx;
                        eidx = next.eidx;
                    }
                    else
                        return false;
                }

                return true;
            }

            public void Reset()
            {
                FMultiRngString first = this.first;

                cidx = first.sidx - 1;
                eidx = first.eidx;
                str = first.str;

                curr = first;
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                first = null;
                curr = null;
                str = null;
            }
        }
        #endregion
    }
}
