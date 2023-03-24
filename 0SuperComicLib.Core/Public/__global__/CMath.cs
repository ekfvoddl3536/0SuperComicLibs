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
using System.Security;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    /// <summary>
    /// If-less logical math operations
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class CMath
    {
        #region integer
        /// <summary>
        /// <see cref="System.Math.Min(int, int)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Min(this int left, int right)
        {
            bool t = left < right;
            return ((*(byte*)&t - 1) & (right - left)) + left;
        }

        /// <summary>
        /// <see cref="System.Math.Max(int, int)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Max(this int left, int right)
        {
            bool t = left < right;
            return ((*(byte*)&t - 1) & (left - right)) + right;
        }

        /// <summary>
        /// <see href="https://learn.microsoft.com/dotnet/api/system.math.clamp"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clampi(this int num, int min, int max) => Max(min, Min(max, num));

        /// <summary>
        /// <paramref name="num"/> 값이 <paramref name="min"/>(포함) 부터 <paramref name="max"/>(포함) 사이에 있는지 여부
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRngIn(this int num, int min, int max) => num >= min && num <= max;

        /// <summary>
        /// <paramref name="num"/> 값이 <paramref name="min"/>(포함) 부터 <paramref name="max"/>(포함) 사이에 존재하지 않는지 여부
        /// </summary>
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

        /// <summary>
        /// <see cref="System.Math.Abs(int)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value)
        {
            int temp = value >> 31;
            return (value ^ temp) - temp;
        }

        /// <summary>
        /// 값을 -1, 0, 1로 노멀라이즈<para/>
        /// 이 함수는 if를 사용하는 방법보다 최대 130.24%p, 평균 71.5485%p 더 빠름: <see href="https://blog.naver.com/ekfvoddl3535/222607247076">ref. blog</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Normal(this int value) => (value >> 31) | (int)((uint)-value >> 31);

        /// <summary>
        /// value to -1 or 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Normal_11(this int value) => (value >> 31) | (int)((uint)~value >> 31);

        /// <summary>
        /// value to 0 or 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Normal_01(this int value) => (int)Normal((uint)value);

        /// <summary>
        /// <paramref name="value"/>를 <paramref name="state"/>이 1일 때, <paramref name="max_include"/>를 최대값으로 하여 뒤집습니다.<para/>
        /// 이 함수는 if를 사용하는 방법보다 평균 16%p 더 빠름: <see href="https://blog.naver.com/ekfvoddl3535/222629296802">ref. blog</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Flip_s(this int value, int max_include, int state)
        {
            FastContract.Requires((uint)state <= 1u, "invalid state value");
            return value - ((value << 1) - max_include) * state;
        }

        /// <summary>
        /// <paramref name="value"/>를 <paramref name="state"/>이 1일 때, <paramref name="max_include"/>를 최대값으로 <paramref name="min_include"/>을 최소값으로 하여 뒤집습니다.<para/>
        /// 이 함수는 if를 사용하는 방법보다 평균 16%p 더 빠름: <see href="https://blog.naver.com/ekfvoddl3535/222629296802">ref. blog</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Flip_s(this int value, int min_include, int max_include, int state)
        {
            FastContract.Requires(state >= 0 && state <= 1, "invalid state value");
            return value - ((value << 1) - max_include - min_include) * state;
        }
        #endregion

        #region uint
        /// <summary>
        /// <see cref="System.Math.Min(uint, uint)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Min(this uint left, uint right)
        {
            uint temp = left - right;
            return right + (temp & (uint)((int)temp >> 31));
        }

        /// <summary>
        /// <see cref="System.Math.Max(uint, uint)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Max(this uint left, uint right)
        {
            uint temp = left - right;
            return left - (temp & (uint)((int)temp >> 31));
        }

        /// <summary>
        /// 값을 0 또는 1로 노멀라이즈.<para/>
        /// 이 함수는 if를 사용하는 방법보다 더 빠름: <see href="https://blog.naver.com/ekfvoddl3535/222607247076">ref. blog</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Normal(this uint value) => (value | (uint)-(int)value) >> 31;

        /// <summary>
        /// <see href="https://learn.microsoft.com/dotnet/api/system.math.clamp"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Clampu(this uint value, uint min, uint max) => Min(Max(value, min), max);
        #endregion

        #region long
        /// <summary>
        /// <see cref="System.Math.Min(long, long)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long Min(this long left, long right)
        {
            bool t = left < right;
            return ((*(byte*)&t - 1) & (right - left)) + left;
        }

        /// <summary>
        /// <see cref="System.Math.Max(long, long)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long Max(this long left, long right)
        {
            bool t = left < right;
            return ((*(byte*)&t - 1) & (left - right)) + right;
        }

        /// <summary>
        /// <see href="https://learn.microsoft.com/dotnet/api/system.math.clamp"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clampi(this long num, long min, long max) => Max(min, Min(max, num));

        /// <summary>
        /// <see cref="System.Math.Abs(int)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Abs(this long value)
        {
            long temp = value >> 63;
            return (value ^ temp) - temp;
        }

        /// <summary>
        /// 값을 -1, 0, 1로 노멀라이즈<para/>
        /// 이 함수는 if를 사용하는 방법보다 최대 130.24%p, 평균 71.5485%p 더 빠름: <see href="https://blog.naver.com/ekfvoddl3535/222607247076">ref. blog</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Normal(this long value) => (value >> 63) | (long)((ulong)-value >> 63);

        /// <summary>
        /// value to -1 or 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Normal_11(this long value) => (value >> 63) | (long)((ulong)~value >> 63);
        #endregion

        #region ulong
        /// <summary>
        /// <see cref="System.Math.Min(ulong, ulong)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Min(this ulong left, ulong right)
        {
            ulong temp = left - right;
            return right + (temp & (ulong)((long)temp >> 63));
        }

        /// <summary>
        /// <see cref="System.Math.Max(ulong, ulong)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Max(this ulong left, ulong right)
        {
            ulong temp = left - right;
            return left - (temp & (ulong)((long)temp >> 63));
        }

        /// <summary>
        /// 값을 0, 1로 노멀라이즈<para/>
        /// 이 함수는 if를 사용하는 방법보다 최대 130.24%p, 평균 71.5485%p 더 빠름: <see href="https://blog.naver.com/ekfvoddl3535/222607247076">ref. blog</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Normal(this ulong value) => (value | (ulong)-(long)value) >> 63;

        /// <summary>
        /// <see href="https://learn.microsoft.com/dotnet/api/system.math.clamp"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Clampu(this ulong value, ulong min, ulong max) => Min(value + min, max);
        #endregion
    }
}
