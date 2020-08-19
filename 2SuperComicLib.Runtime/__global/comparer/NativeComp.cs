using SuperComicLib.LowLevel;
using System;

namespace SuperComicLib.Runtime
{
    internal sealed class NativeComp<T> : FastComparer<T>
        where T : struct
    {
        public override bool EqualsAB(T left, T right) => NativeClass.CompareTo(ref left, ref right) == 0;

        public override bool Greater(T left, T right) => NativeClass.CompareTo(ref left, ref right) > 0;

        public override bool GreatOrEquals(T left, T right) => NativeClass.CompareTo(ref left, ref right) >= 0;

        public override bool Lesser(T left, T right) => NativeClass.CompareTo(ref left, ref right) < 0;

        public override bool LessOrEquals(T left, T right) => NativeClass.CompareTo(ref left, ref right) <= 0;
    }

    internal sealed class ObjectNativeComparer<T> : FastComparer<T>
    {
        public override bool EqualsAB(T left, T right) => ReferenceEquals(left, right);

        public override bool Greater(T left, T right) => NativeClass.ReferenceCompare(left, right) > 0;

        public override bool GreatOrEquals(T left, T right) => NativeClass.ReferenceCompare(left, right) >= 0;

        public override bool Lesser(T left, T right) => NativeClass.ReferenceCompare(left, right) < 0;

        public override bool LessOrEquals(T left, T right) => NativeClass.ReferenceCompare(left, right) <= 0;
    }

    internal sealed class NullableNativeComparer<T> : FastComparer<T?>
        where T : struct
    {
        public override bool EqualsAB(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? NativeClass.CompareTo(left.Value, right.Value) == 0
                : false
            : right.HasValue == false;

        public override bool Greater(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? NativeClass.CompareTo(left.Value, right.Value) > 0
                : false
            : right.HasValue == false;

        public override bool GreatOrEquals(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? NativeClass.CompareTo(left.Value, right.Value) >= 0
                : false
            : right.HasValue == false;

        public override bool Lesser(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? NativeClass.CompareTo(left.Value, right.Value) < 0
                : false
            : right.HasValue == false;

        public override bool LessOrEquals(T? left, T? right) =>
            left.HasValue
            ?
                right.HasValue
                ? NativeClass.CompareTo(left.Value, right.Value) <= 0
                : false
            : right.HasValue == false;
    }
}
