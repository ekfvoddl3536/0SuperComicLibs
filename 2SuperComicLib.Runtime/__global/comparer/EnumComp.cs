namespace SuperComicLib.Runtime
{
    internal sealed unsafe class EnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(int*)&left == *(int*)&right;

        public override bool Greater(T left, T right) => *(int*)&left > *(int*)&right;

        public override bool GreatOrEquals(T left, T right) => *(int*)&left >= *(int*)&right;

        public override bool Lesser(T left, T right) => *(int*)&left < *(int*)&right;

        public override bool LessOrEquals(T left, T right) => *(int*)&left <= *(int*)&right;
    }

    internal sealed unsafe class UnsignedEnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(uint*)&left == *(uint*)&right;

        public override bool Greater(T left, T right) => *(uint*)&left > *(uint*)&right;

        public override bool GreatOrEquals(T left, T right) => *(uint*)&left >= *(uint*)&right;

        public override bool Lesser(T left, T right) => *(uint*)&left < *(uint*)&right;

        public override bool LessOrEquals(T left, T right) => *(uint*)&left <= *(uint*)&right;
    }

    internal sealed unsafe class LongEnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(long*)&left == *(long*)&right;

        public override bool Greater(T left, T right) => *(long*)&left > *(long*)&right;

        public override bool GreatOrEquals(T left, T right) => *(long*)&left >= *(long*)&right;

        public override bool Lesser(T left, T right) => *(long*)&left < *(long*)&right;

        public override bool LessOrEquals(T left, T right) => *(long*)&left <= *(long*)&right;
    }

    internal sealed unsafe class UnsignedLongEnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(ulong*)&left == *(ulong*)&right;

        public override bool Greater(T left, T right) => *(ulong*)&left > *(ulong*)&right;

        public override bool GreatOrEquals(T left, T right) => *(ulong*)&left >= *(ulong*)&right;

        public override bool Lesser(T left, T right) => *(ulong*)&left < *(ulong*)&right;

        public override bool LessOrEquals(T left, T right) => *(ulong*)&left <= *(ulong*)&right;
    }
}
