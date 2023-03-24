// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    /// <summary>
    /// SoftwareFallback bit operations
    /// </summary>
    public static class BitMath
    {
        #region underbits
        /// <summary>
        /// 0b0001_0100 -> 0b0001_1111
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetUnderbits(this uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            return value >> 16;
        }

        /// <summary>
        /// 0b0001_0100 -> 0b0001_1111
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SetUnderbits64(this ulong value)
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
        public static int MSB(this int value) => (int)MSB((uint)value);

        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MSB(this uint value)
        {
            value = SetUnderbits(value);
            return value ^ (value >> 1);
        }

        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MSB64(this long value) => (long)MSB64((ulong)value);

        /// <summary>
        /// Most Significant Bit (최상위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MSB64(this ulong value)
        {
            value = SetUnderbits64(value);
            return value ^ (value >> 1);
        }
        #endregion

        #region lsb
        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LSB(this uint value) => (uint)LSB((int)value);

        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LSB(this int value) => value & -value;

        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong LSB64(this ulong value) => (ulong)LSB64((long)value);

        /// <summary>
        /// Least Significant Bit (최하위 비트)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LSB64(this long value) => value & -value;
        #endregion

        #region popcnt
        /// <summary>
        /// <see href="https://learn.microsoft.com/cpp/intrinsics/popcnt16-popcnt-popcnt64"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt(this int value) => Popcnt((uint)value);

        /// <summary>
        /// <see href="https://learn.microsoft.com/cpp/intrinsics/popcnt16-popcnt-popcnt64"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt(this uint value)
        {
            value -= (value >> 1) & 0x5555_5555u;
            value = (value & 0x3333_3333u) + ((value >> 2) & 0x3333_3333u);
            return (int)(((value + (value >> 4)) & 0xF0F_0F0Fu) * 0x101_0101u) >> 24;
        }

        /// <summary>
        /// <see href="https://learn.microsoft.com/cpp/intrinsics/popcnt16-popcnt-popcnt64"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt64(this long value) => Popcnt64((ulong)value);

        /// <summary>
        /// <see href="https://learn.microsoft.com/cpp/intrinsics/popcnt16-popcnt-popcnt64"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Popcnt64(this ulong value)
        {
            value -= (value >> 1) & 0x5555_5555_5555_5555UL;
            value = (value & 0x3333_3333_3333_3333UL) + ((value >> 2) & 0x3333_3333_3333_3333UL);
            return (int)((((value + (value >> 4)) & 0x0F0F_0F0F_0F0F_0F0FUL) * 0x0101_0101_0101_0101UL) >> 56);
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
        public static int FLS(this uint value) => Popcnt(SetUnderbits(value));

        /// <summary>
        /// Find Last Set, 최상위 비트 position (index + 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FLS64(this long value) => FLS64((ulong)value);

        /// <summary>
        /// Find Last Set, 최상위 비트 position (index + 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FLS64(this ulong value) => Popcnt64(SetUnderbits64(value));
        #endregion
    }
}
