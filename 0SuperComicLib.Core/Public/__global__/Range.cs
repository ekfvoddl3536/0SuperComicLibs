using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public struct Range : IEquatable<Range>, IComparable<Range>
    {
        public int start;
        public int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int Length => end - start;

        public bool Equals(Range other) => this == other;
        public int CompareTo(Range other)
        {
            if (end < other.start)
                return -1;
            else if (start > other.end)
                return 1;
            else
                return start.CompareTo(other.start);
        }

        public override bool Equals(object obj) => obj is Range other && Equals(other);
        public override int GetHashCode() => start ^ end;

        public static bool operator ==(Range left, Range right) => ((left.start - right.start) | (left.end - right.end)) == 0;
        public static bool operator !=(Range left, Range right) => ((left.start - right.start) | (left.end - right.end)) != 0;

        public static bool operator <(Range left, Range right) => left.end < right.start;
        public static bool operator >(Range left, Range right) => left.start > right.end;

        public static bool operator <=(Range left, Range right) => left.end <= right.start;
        public static bool operator >=(Range left, Range right) => left.start >= right.end;
    }
}
