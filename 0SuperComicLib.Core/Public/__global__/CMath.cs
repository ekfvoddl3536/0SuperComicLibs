namespace SuperComicLib
{
    public static class CMath
    {
        public static int Min(this int left, int right) => left < right ? left : right;

        public static int Max(this int left, int right) => left > right ? left : right;

        public static int RngOut(this int num, int min, int max) =>
            num < min
            ? num
            : num > max
            ? num
            : min;

        public static int RngIn(this int num, int min, int max) =>
            num > max
            ? max
            : num < min
            ? min
            : num;

        public static bool IsRngIn(this int num, int min, int max) => num >= min && num <= max;

        public static bool IsRngOut(this int num, int min, int max) => num < min || num > max;

        public static int Positive(this int value) => value & int.MaxValue;

        public static int Negative(this int value) => value | int.MinValue;

        public static int Abs(this int value) => value < 0 ? -value : value;

        public static int Div10fs(this int value) => (int)(((ulong)value * 0xCCCC_CCCD) >> 35);

        public static uint Div10fs(this uint value) => (uint)(((ulong)value * 0xCCCC_CCCD) >> 35);
    }
}
