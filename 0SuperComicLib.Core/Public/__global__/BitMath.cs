// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

        #region broadcast
        /// <summary>
        /// Broadcasts an <see langword="byte"/> value to an <see langword="uint"/>.
        /// </summary>
        /// <remarks>
        /// Takes an 8-bit <see langword="byte"/> value and duplicates it into each 8-bit segment of a 32-bit <see langword="uint"/>.<br/>
        /// The resulting 32-bit number consists of the original 8-bit value repeated 4-times.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BroadcastToUint32(this byte value)
        {
            // movzx eax, cl
            // imul eax, eax, 01010101h
            return value * 0x0101_0101u; 
        }

        /// <summary>
        /// Broadcasts an <see langword="ushort"/> value to an <see langword="uint"/>.
        /// </summary>
        /// <remarks>
        /// Takes an 16-bit <see langword="ushort"/> value and duplicates it into each 16-bit segment of a 32-bit <see langword="uint"/>.<br/>
        /// The resulting 32-bit number consists of the original 16-bit value repeated 2-times.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BroadcastToUint32(this ushort value)
        {
            // movzx eax, cx
            // sal ecx, 16
            // add eax, ecx
            uint eax = value;
            return ((uint)value << 16) | eax;
        }

        /// <summary>
        /// Broadcasts an <see langword="byte"/> value to an <see langword="ulong"/>.
        /// </summary>
        /// <remarks>
        /// Takes an 8-bit <see langword="byte"/> value and duplicates it into each 8-bit segment of a 64-bit <see langword="ulong"/>.<br/>
        /// The resulting 64-bit number consists of the original 8-bit value repeated 8-times. 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong BroadcastToUint64(this byte value)
        {
            // movabs rdx, 0101010101010101h
            // movzx eax, cl
            // imul rax, rdx
            uint eax = value;
            return eax * 0x01010101_01010101ul;
        }

        /// <summary>
        /// Broadcasts an <see langword="ushort"/> value to an <see langword="ulong"/>.
        /// </summary>
        /// <remarks>
        /// Takes an 16-bit <see langword="ushort"/> value and duplicates it into each 16-bit segment of a 64-bit <see langword="ulong"/>.<br/>
        /// The resulting 64-bit number consists of the original 16-bit value repeated 4-times. 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong BroadcastToUint64(this ushort value)
        {
            // movzx ecx, cx
            // mov rax, rcx
            // sal rax, 16
            // add rax, rcx
            // mov rdx, rax
            // sal rdx, 32
            // add rax, rdx
            ulong rax = value;
            rax = (rax << 16) + rax;
            return (rax << 32) + rax;
        }

        /// <summary>
        /// Broadcasts an <see langword="uint"/> value to an <see langword="ulong"/>.
        /// </summary>
        /// <remarks>
        /// Takes an 32-bit <see langword="uint"/> value and duplicates it into each 32-bit segment of a 64-bit <see langword="ulong"/>.<br/>
        /// The resulting 64-bit number consists of the original 32-bit value repeated 2-times. 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong BroadcastToUint64(this uint value)
        {
            // mov eax, ecx
            // sal rcx, 32
            // add rax, rcx
            ulong rax = value;
            return (rax << 32) + rax;
        }
        #endregion
    }
}
