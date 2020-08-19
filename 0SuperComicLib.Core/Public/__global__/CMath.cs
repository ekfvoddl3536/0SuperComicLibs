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
    }
}
