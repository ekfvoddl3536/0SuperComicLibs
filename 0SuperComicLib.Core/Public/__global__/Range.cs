using System;

namespace SuperComicLib
{
    public readonly struct Range : IEquatable<Range>
    {
        public readonly int start;
        public readonly int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public override bool Equals(object obj) =>
            obj is Range other && Equals(other);

        public bool Equals(Range other) => this == other;

        public override int GetHashCode() => 39 * (start + end);

        public static bool operator ==(Range left, Range right) =>
            left.start == right.start &&
            left.end == right.end;

        public static bool operator !=(Range left, Range right) =>
            left.start != right.start ||
            left.end != right.end;
    }
}
