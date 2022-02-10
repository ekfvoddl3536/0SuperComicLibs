using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static class CMath
    {
        #region integer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int left, int right)
        {
            int temp = left - right;
            return right + (temp & (temp >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int left, int right)
        {
            int temp = left - right;
            return left - (temp & (temp >> 31));
        }

        // RngIn --> Clampi 이름 변경
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clampi(this int num, int min, int max) => Max(min, Min(max, num));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRngIn(this int num, int min, int max) => num >= min && num <= max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRngOut(this int num, int min, int max) => num < min || num > max;

        /// <summary>
        /// 양수부분만 남도록 강제로 자릅니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pos1(this int value) => value & int.MaxValue;

        /// <summary>
        /// 강제로 음수로 설정합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Neg1(this int value) => value | int.MinValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value)
        {
            int temp = value >> 31;
            return (value ^ temp) - temp;
        }

        // https://blog.naver.com/ekfvoddl3535/222607247076
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Normal(this int value) => (value >> 31) | (int)((uint)-value >> 31);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static int Flip(this int value, int max_include) => max_include - value;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static int Flip(this int value, int min_include, int max_include) => max_include - (value + min_include);

        // https://blog.naver.com/ekfvoddl3535/222629296802
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Flip_s(this int value, int max_include, int state)
        {
            Contract.Requires(state >= 0 && state <= 1, "invalid state value");
            return value - ((value << 1) - max_include) * state;
        }

        // https://blog.naver.com/ekfvoddl3535/222629296802
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Flip_s(this int value, int min_include, int max_include, int state)
        {
            Contract.Requires(state >= 0 && state <= 1, "invalid state value");
            return value - ((value << 1) - max_include - min_include) * state;
        }
        #endregion

        #region long
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(this long left, long right)
        {
            long temp = left - right;
            return right + (temp & (temp >> 63));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Max(this long left, long right)
        {
            long temp = left - right;
            return left - (temp & (temp >> 63));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clampi(this long num, long min, long max) => Max(min, Min(max, num));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Abs(this long value)
        {
            long temp = value >> 63;
            return (value ^ temp) - temp;
        }

        // https://blog.naver.com/ekfvoddl3535/222607247076
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Normal(this long value) => (value >> 63) | (long)((ulong)-value >> 63);
        #endregion
    }
}
