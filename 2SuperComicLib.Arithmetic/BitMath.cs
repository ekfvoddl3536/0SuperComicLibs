// https://stackoverflow.com/questions/2709430/count-number-of-bits-in-a-64-bit-long-big-integer

using System.Runtime.CompilerServices;

namespace SuperComicLib.Arithmetic
{
    public static class BitMath
    {
        #region fill
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Fill(this uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            return value >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Fill64(this ulong value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value >> 32;
        }
        #endregion

        #region msb
        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MSB(this int value) => MSB((uint)value);

        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MSB(this uint value)
        {
            value = Fill(value);
            return (int)(value - (value >> 1));
        }

        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MSB64(this long value) => MSB64((ulong)value);

        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MSB64(this ulong value)
        {
            value = Fill64(value);
            return (long)(value - (value >> 1));
        }
        #endregion

        #region lsb
        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LSB(this int value) => LSB((uint)value);

        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LSB(this uint value) => (int)value & -(int)value;

        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LSB64(this long value) => LSB64((ulong)value);

        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LSB64(this ulong value) => (long)value & -(long)value;
        #endregion

        #region popcnt
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt(this int value) => Popcnt((uint)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt(this uint value)
        {
            value -= (value >> 1) & 0x5555_5555u;
            value = (value & 0x3333_3333u) + ((value >> 2) & 0x3333_3333u);
            return (int)(((value + (value >> 4)) & 0xF0F_0F0Fu) * 0x101_0101u) >> 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt64(this long value) => Popcnt64((ulong)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt64(this ulong value)
        {
            value -= (value >> 1) & 0x5555555555555555UL;
            value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
            return (int)((((value + (value >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }
        #endregion

        #region ffs
        /// <summary>
        /// Find First Set, 최하위 비트 index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FFS(this int value) => FFS((uint)value);

        /// <summary>
        /// Find First Set, 최하위 비트 index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FFS(this uint value) => Popcnt(value ^ (value - 1));

        /// <summary>
        /// Find First Set, 최하위 비트 index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FFS64(this long value) => FFS64((ulong)value);

        /// <summary>
        /// Find First Set, 최하위 비트 index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FFS64(this ulong value) => Popcnt64(value ^ (value - 1));
        #endregion

        #region fls
        /// <summary>
        /// Find Last Set, 최상위 비트 position (index + 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FLS(this int value) => FLS((uint)value);

        /// <summary>
        /// Find Last Set, 최상위 비트 position (index + 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FLS(this uint value) => Popcnt(Fill(value));

        /// <summary>
        /// Find Last Set, 최상위 비트 position (index + 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FLS64(this long value) => FLS64((ulong)value);

        /// <summary>
        /// Find Last Set, 최상위 비트 position (index + 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FLS64(this ulong value) => Popcnt64(Fill64(value));
        #endregion
    }
}
