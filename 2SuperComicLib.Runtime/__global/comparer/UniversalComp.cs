using System;

namespace SuperComicLib.Runtime
{
    internal sealed class GenericUniversalComparer<T> : FastComparer<T>
        where T : IComparable<T>
    {
        public override bool EqualsAB(T left, T right) => left.CompareTo(right) == 0;

        public override bool Greater(T left, T right) => left.CompareTo(right) > 0;

        public override bool GreatOrEquals(T left, T right) => left.CompareTo(right) >= 0;

        public override bool Lesser(T left, T right) => left.CompareTo(right) < 0;

        public override bool LessOrEquals(T left, T right) => left.CompareTo(right) <= 0;
    }

    internal sealed class NullableUniversalComparer<T> : FastComparer<T?>
        where T : struct, IComparable<T>
    {
        public override bool EqualsAB(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? left.Value.CompareTo(right.Value) == 0
                : false
            : right.HasValue == false;

        public override bool Greater(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? left.Value.CompareTo(right.Value) > 0
                : false
            : right.HasValue == false;

        public override bool GreatOrEquals(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? left.Value.CompareTo(right.Value) >= 0
                : false
            : right.HasValue == false;

        public override bool Lesser(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? left.Value.CompareTo(right.Value) < 0
                : false
            : right.HasValue == false;

        public override bool LessOrEquals(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? left.Value.CompareTo(right.Value) <= 0
                : false
            : right.HasValue == false;
    }
}
