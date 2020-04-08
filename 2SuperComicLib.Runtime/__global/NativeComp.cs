using SuperComicLib.LowLevel;

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

    internal sealed class ObjectUniversalComparer<T> : FastComparer<T>
    {
        public override bool EqualsAB(T left, T right) => ReferenceEquals(left, right);

        public override bool Greater(T left, T right) => NativeClass.ReferenceCompare(left, right) > 0;

        public override bool GreatOrEquals(T left, T right) => NativeClass.ReferenceCompare(left, right) >= 0;

        public override bool Lesser(T left, T right) => NativeClass.ReferenceCompare(left, right) < 0;

        public override bool LessOrEquals(T left, T right) => NativeClass.ReferenceCompare(left, right) <= 0;
    }
}
