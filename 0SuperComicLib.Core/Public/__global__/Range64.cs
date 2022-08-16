using System;

namespace SuperComicLib
{
    public struct Range64 : IEquatable<Range64>, IComparable<Range64>
    {
        public long start;
        public long end;

        public Range64(long start, long end)
        {
            this.start = start;
            this.end = end;
        }

        public long Length => end - start;

        public bool Equals(Range64 other) => this == other;
        public int CompareTo(Range64 other)
        {
            if (end < other.start)
                return -1;
            else if (start > other.end)
                return 1;
            else
                return start.CompareTo(other.start);
        }

        public override bool Equals(object obj) => obj is Range64 other && Equals(other);
        public override int GetHashCode() => (start ^ end).GetHashCode();

        public static bool operator ==(Range64 left, Range64 right) => ((left.start - right.start) | (left.end - right.end)) == 0;
        public static bool operator !=(Range64 left, Range64 right) => ((left.start - right.start) | (left.end - right.end)) != 0;

        public static bool operator <(Range64 left, Range64 right) => left.end < right.start;
        public static bool operator >(Range64 left, Range64 right) => left.start > right.end;

        public static bool operator <=(Range64 left, Range64 right) => left.end <= right.start;
        public static bool operator >=(Range64 left, Range64 right) => left.start >= right.end;
    }
}
