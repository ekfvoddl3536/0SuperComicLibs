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

        /// <summary>
        /// 양수부분만 남도록 강제로 자릅니다
        /// </summary>
        public static int Pos1(this int value) => value & int.MaxValue;

        /// <summary>
        /// 강제로 음수로 설정합니다.
        /// </summary>
        public static int Neg1(this int value) => value | int.MinValue;

        public static int Abs(this int value) => value < 0 ? -value : value;
        
        // - slower than div 10
        // public static int Div10fs(this int value) => (int)((value * 1717986919L) >> 34) - (value >> 31);
        // 
        // public static uint Div10fs(this uint value) => (uint)Div10fs((int)value);
        // 
        // public static int DivMod10fs(this int value, out int mod)
        // {
        //     int div = Div10fs(value);
        //     mod = value - ((div << 2) + div << 1);
        // 
        //     return div;
        // }
        // 
        // public static uint DivMod10fs(this uint value, out uint mod)
        // {
        //     uint div = (uint)Div10fs((int)value);
        //     mod = value - ((div << 2) + div << 1);
        // 
        //     return div;
        // }
    }
}
