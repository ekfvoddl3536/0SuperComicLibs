// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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

using System.Globalization;
using System.Runtime.InteropServices;

namespace SuperComicLib.Arithmetic
{
    internal static unsafe class BUtils
    {
        public static readonly NumberFormatInfo formatInfo = NumberFormatInfo.CurrentInfo;

        #region allocate
        public static uint* RtlzAlloc(int size)
        {
            uint* temp = (uint*)Marshal.AllocHGlobal(size << 2);
            Zeromem(temp, size);

            return temp;
        }

        public static void Zeromem(uint* ptr, int arr_size)
        {
            long* v = (long*)ptr;
            for (; arr_size >= 2; arr_size -= 2)
                *v++ = 0;

            *(int*)v = 0;
        }

        // -1 to 1
        public static uint* NegAlloc(uint* src, int size)
        {
            uint* temp = (uint*)Marshal.AllocHGlobal(size << 2);
            BigIntArithmetic.NEG(src, temp, size);

            return temp;
        }
        #endregion

        #region some
        public static int GetLast(uint* ptr, int size)
        {
            uint* v = ptr + --size;
            for (; size > 0; v--, size--)
                if (*v != 0)
                    return size;

            return 0;
        }

        public static int Approx(uint* ptr, int idx, out ulong mantissa)
        {
            if (idx == 0) // empty exponent
            {
                mantissa = *ptr;
                return 0;
            }

            mantissa = ((ulong)*ptr << 32) | *(ptr - 1);
            int exp = --idx * 32;

            int bit;
            if (idx > 0 && (bit = BitMath.FLS64(*ptr)) > 0)
            {
                mantissa = (mantissa << bit) | (*(ptr - 2) >> (32 - bit));
                exp -= bit;
            }

            return exp;
        }

        public static ulong ReadBits(uint* ptr, int size)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(size > 0);
#endif
            return
                size > 1
                ? ((ulong)*(ptr - 1) << 32) | *(ptr - 2)
                : ((ulong)*(ptr - 1) << 32);
        }

        public static ulong FormatIEEE754BFP(
            ulong mantissa,
            int exp,
            int bit_exp_size,
            int bit_mt_size,
            int exp_bias,
            int mask1,
            ulong mask2)
        {
            int t0;
            if ((t0 = mantissa.FLS64() - bit_exp_size) < 0)
                mantissa >>= -t0;
            else
                mantissa <<= t0;

            ulong result;
            if ((exp = exp - t0 + bit_mt_size + exp_bias) >= mask1) // 0x7FF or 0xFF
            { // infinity
                result = (ulong)mask1 << bit_mt_size;
            }
            else if (exp <= 0)
            { // denormalized
                if (--exp < -bit_mt_size) // underflow
                    result = 0;
                else
                    result = mantissa >> -exp;
            }
            else
                result = (mantissa & mask2) | ((ulong)exp << bit_mt_size);

            return result;
        }
        #endregion
    }
}
