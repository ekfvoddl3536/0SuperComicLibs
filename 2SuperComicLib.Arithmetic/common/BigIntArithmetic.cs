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

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperComicLib.Arithmetic
{
    public static unsafe class BigIntArithmetic
    {
        internal const uint kuBase = 1_000_000_000; // 10^9
        internal const int kcchBase = 9;

        #region print
        public static string FormatString(uint* value, int size, bool signed, string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                return ToString(value, size, signed);

            char f = char.ToLower(format[0]);
            int count = ToInteger.Positive(format, 1);

            return
                (f == 'h' || f == 'x')
                ? ToHexString(value, size, count)
                : ToExpToString(value, size, count, signed);
        }

        public static string ToHexString(uint* value, int count, int h_size)
        {
            StringBuilder sb = new StringBuilder(64);

            uint temp = value[--count];
            if (temp > 0)
            {
                sb.Append(temp.ToString("X"));
                h_size -= sb.Length;
            }
            int b_len = sb.Length;

            while (--count >= 0)
                if ((temp = value[count]) > 0)
                {
                    if (b_len > 0)
                        sb.Append(temp.ToString("X8"));
                    else
                        sb.Append(temp.ToString("X"));
                    h_size -= sb.Length - b_len;
                    b_len = sb.Length;
                }

            if (h_size > 0)
                sb.Insert(0, new string('0', h_size));

            return sb.ToString();
        }

        public static string ToExpToString(uint* value, int count, int e_size, bool signed)
        {
            StringBuilder sb = new StringBuilder(64);
            Print(sb, value, count, signed);

            int sz = sb.Length - 1;
            if (e_size > 0 && sz > e_size)
                sb.Remove(e_size + 1, sz - e_size);

            sb.Insert(1, BUtils.formatInfo.NumberDecimalSeparator);
            sb.Append("0e+");
            sb.Append(sz.ToString());

            return sb.ToString();
        }

        public static string ToString(uint* value, int count, bool signed)
        {
            StringBuilder sb = new StringBuilder(64);
            Print(sb, value, count, signed);
            return sb.ToString();
        }

        // reference: BigInteger.ToString()
        public static void Print(StringBuilder sb, uint* v, int count, bool signed)
        {
            bool allocated = false;

        loop:
            // 상위 비트가 0일때 시작할 index를 보정하는 작업
            int x = count;
            uint temp = v[count - 1];
            while (--x >= 0 && v[x] == 0)
                count--;

            // 모든 요소들이 0일 경우
            if (x < 0)
            {
                sb.Append('0');
                return;
            }

            if (signed && temp >= 0x8000_0000)
            {
                signed = false;
                allocated = true;

                v = BUtils.NegAlloc(v, count);
                sb.Append(BUtils.formatInfo.NegativeSign);

                goto loop;
            }

            uint[] rn = new uint[count * 10 / 9 + 2];

            int cuDst = 0;
            int y;

            while (x >= 0)
            {
                temp = v[x--];
                for (y = 0; y < cuDst; y++)
                {
                    ulong uuRes = ((ulong)rn[y] << 32) | temp;
                    rn[y] = (uint)(uuRes % kuBase);
                    temp = (uint)(uuRes / kuBase);
                }
                if (temp != 0)
                {
                    rn[cuDst++] = temp % kuBase;
                    temp /= kuBase;
                    if (temp != 0)
                        rn[cuDst++] = temp;
                }
            }

            int len;
            char[] buffer = new char[y = len = checked(cuDst * kcchBase)];

            cuDst--;
            for (x = 0; x < cuDst; x++)
            {
                temp = rn[x];
                for (count = kcchBase; --count >= 0;)
                {
                    buffer[--y] = (char)('0' + temp % 10);
                    temp /= 10;
                }
            }
            for (temp = rn[cuDst]; temp != 0;)
            {
                buffer[--y] = (char)('0' + temp % 10);
                temp /= 10;
            }

            sb.Append(buffer, y, len - y);

            if (allocated)
                Marshal.FreeHGlobal((IntPtr)v);
        }
        #endregion

        #region negation operand
        public static void NEG(uint* src, uint* res, int count)
        {
            uint borrow = 0;
            for (int x = 0; x < count;)
            {
                ulong k = src[x];
                ulong uuTemp = k - (k << 1) - borrow;

                borrow = (uint)-(int)(uuTemp >> 32);
                res[x++] = (uint)uuTemp;
            }
        }
        #endregion

        #region + -
        public static void AddFast(uint* values, uint add, int count)
        {
            ulong uuTemp = (ulong)*values + add;
            *values = (uint)uuTemp;
            uint carry = (uint)(uuTemp >> 32);

            for (int x = 1; x < count && carry != 0;)
            {
                uuTemp = (ulong)values[x] + carry;
                values[x++] = (uint)uuTemp;
                carry = (uint)(uuTemp >> 32);
            }
        }

        public static void SubFast(uint* values, uint sub, int count)
        {
            ulong uuTemp = (ulong)*values - sub;
            values[count] = (uint)uuTemp;
            uint borrow = (uint)-(int)(uuTemp >> 32);

            for (int x = 1; x < count && borrow != 0;)
            {
                uuTemp = (ulong)values[x] - borrow;
                values[x++] = (uint)uuTemp;
                borrow = (uint)-(int)(uuTemp >> 32);
            }
        }

        public static void Add(uint* values, uint* add, int count)
        {
            ulong uuTemp = (ulong)*values + *add;
            *values = (uint)uuTemp;
            uint carry = (uint)(uuTemp >> 32);

            for (int x = 1; x < count;)
            {
                uuTemp = (ulong)values[x] + add[x] + carry;
                values[x++] = (uint)uuTemp;
                carry = (uint)(uuTemp >> 32);
            }
        }

        public static void Sub(uint* values, uint* sub, int count)
        {
            ulong uuTemp = (ulong)values[--count] - sub[count];
            values[count] = (uint)uuTemp;
            uint borrow = (uint)-(int)(uuTemp >> 32);

            while (--count >= 0)
            {
                uuTemp = (ulong)values[count] - sub[count] - borrow;
                values[count] = (uint)uuTemp;
                borrow = (uint)-(int)(uuTemp >> 32);
            }
        }
        #endregion

        #region mul
        public static void Mul(uint* values, uint* mul, uint* res, int count)
        {
            for (int x = 0; x < count; x++)
            {
                uint carry = 0;
                ulong uuMul = mul[x];
                for (int y = x, z; y < count; y++)
                {
                    ulong uuTemp = values[y - x] * uuMul + res[z = x + y] + carry;
                    res[z] = (uint)uuTemp;
                    carry = (uint)(uuTemp >> 32);
                }
            }
        }

        public static void MulFast(uint* values, uint mul, uint* res, int count)
        {
            uint carry = 0;
            ulong uuMul = mul;
            for (int y = 0; y < count; y++)
            {
                ulong uuTemp = values[y] * uuMul + res[y] + carry;
                res[y] = (uint)uuTemp;
                carry = (uint)(uuTemp >> 32);
            }
        }
        #endregion

        #region bit
        public static void OR(ulong* value, ulong* num1, int count)
        {
            while (--count >= 0)
                value[count] |= num1[count];
        }

        public static void XOR(ulong* value, ulong* num1, int count)
        {
            while (--count >= 0)
                value[count] ^= num1[count];
        }

        public static void AND(ulong* value, ulong* num1, int count)
        {
            while (--count >= 0)
                value[count] &= num1[count];
        }

        public static void LSHIFT(ulong* value, int shift, int max)
        {
            // int idx = shift >> 6; // x 나누기 64

            int lsh = shift & 0x3F;
            int rsh = 64 - lsh;

            // 0 0 0 , 1 1 1 s0 d3
            // 0 0 1 , 1 1 0 s1 d4
            // 0 1 1 , 1 0 0 s2 d5
            // 1 1 1 , 0 0 0 s3 d6 - break

            int di = max - 1;
            int si = di - (shift >> 6);

            for (; si > 0; si--, di--)
                value[di] =
                    (value[si] << lsh) |
                    (value[si - 1] >> rsh);

            value[di] = value[si] << lsh;

            while (--di >= 0)
                value[di] = 0;
        }

        public static void RSHIFT(ulong* value, int shift, int max)
        {
            int rsh = shift & 0x3F;
            int lsh = 64 - rsh;

            int di = 0;
            int si = di + (shift >> 6);

            for (max--; si < max; si++, di++)
                value[di] =
                    (value[si] >> rsh) |
                    (value[si + 1] << lsh);

            value[di] = value[si] >> rsh;

            while (++di <= max)
                value[di] = 0;
        }

        public static void NOT(ulong* value, int count)
        {
            while (--count >= 0)
                value[count] = ~value[count];
        }
        #endregion

        #region div & mod
        // =========================================
        //  
        //  Reference:
        //      https://referencesource.microsoft.com/#System.Numerics/System/Numerics/BigIntegerBuilder.cs
        // 
        // =========================================
        private static int INDEX(ulong* value, int count)
        {
            uint* ptr = (uint*)value;

            count <<= 1;
            while (--count >= 0 && ptr[count] == 0)
                ;

            return count;
        }

        private static void QDiv(ulong* num1, ulong* result, uint den, int count)
        {
            if (den == 1)
                return;
            else if (count == 0)
            {
                *(uint*)result = *(uint*)num1 / den;
                return;
            }

            uint* psrc = (uint*)num1;
            uint* pres = (uint*)result;

            ulong uuTemp = 0, uuDen = den;

            count <<= 1;
            while (--count >= 0)
            {
                uuTemp = (uuTemp << 32) | psrc[count];
                pres[count] = (uint)(uuTemp / uuDen);
                uuTemp %= uuDen;
            }
        }

        private static uint QMod(ulong* num1, uint den, int count)
        {
            if (den == 1)
                return 0;
            else if (count == 0)
                return *(uint*)num1 % den;

            uint* psrc = (uint*)num1;
            ulong uuTemp = 0, uuDen = den;

            count <<= 1;
            while (--count >= 0)
            {
                uuTemp = (uuTemp << 32) | psrc[count];
                uuTemp %= uuDen;
            }

            return (uint)uuDen;
        }

        public static void Div(ulong* num1, ulong* num2, ulong* result, int count)
        {
            int d_count = INDEX(num2, count);
            if (d_count < 0)
                throw new DivideByZeroException();
            else if (d_count == 0)
            {
                QDiv(num1, result, *(uint*)num2, count);
                return;
            }

            int n_count = INDEX(num1, count);
            if (n_count < 0)
                return;
            else if (d_count == 1 && n_count == 1)
            {
                *result = *num1 / *num2;
                return;
            }

            ModDivCore((uint*)num1, (uint*)num2, (uint*)result, n_count, d_count, true);
        }

        public static void Mod(ulong* num1, ulong* num2, int count)
        {
            int d_count = INDEX(num2, count);
            if (d_count < 0)
                return;
            else if (d_count == 0)
            {
                *(uint*)num1 = QMod(num1, *(uint*)num2, count);
                return;
            }

            int n_count = INDEX(num1, count);
            if (n_count == 0)
                return;
            else if (d_count == 1 && n_count == 1)
            {
                *num1 %= *num2;
                return;
            }

            ModDivCore((uint*)num1, (uint*)num2, null, n_count, d_count, false);
        }

        private static void ModDivCore(uint* num_s, uint* den_s, uint* quo, int num_count, int den_count, bool divMode)
        {
            if (num_count < den_count)
                return;

            int cDen = den_count + 1;
            int cNum = num_count;
            int cTemp = cNum - den_count;

            int cuQuo = cTemp;
            for (; ; num_count--)
                if (den_s[num_count - cTemp] != num_s[num_count])
                {
                    if (den_s[num_count - cTemp] < num_s[num_count])
                        cuQuo++;

                    break;
                }

            if (cuQuo == 0)
                return;

            uint uDen = den_s[cDen - 1];
            uint uDenNext = den_s[cDen - 2];
            int lsh = uDen.FLS();
            int rsh = 32 - lsh;
            if (lsh > 0)
            {
                uDen = (uDen << lsh) | (uDenNext >> rsh);
                uDenNext <<= lsh;
                if (cDen > 2)
                    uDenNext |= den_s[cDen - 3] >> rsh;
            }

            // reuse
            for (cTemp = cuQuo; --cTemp >= 0;)
            {
                uint h =
                    cTemp + cDen <= cNum
                    ? num_s[cTemp + cDen]
                    : 0;

                ulong uuTemp = ((ulong)h << 32) | num_s[cTemp + cDen - 1];
                uint n_tmp = num_s[cTemp + cDen - 2];
                if (lsh > 0)
                {
                    uuTemp = (uuTemp << lsh) | (n_tmp >> rsh);
                    n_tmp <<= lsh;
                    if (cTemp + cDen >= 3)
                        n_tmp |= num_s[cTemp + cDen - 3] >> rsh;
                }

                ulong uuQuo = uuTemp / uDen;
                uuTemp = (uint)(uuTemp % uDen);
                if (uuQuo > uint.MaxValue)
                {
                    uuTemp += uDen * (uuQuo - uint.MaxValue);
                    uuQuo = uint.MaxValue;
                }
                while (uuTemp <= uint.MaxValue && uuQuo * uDenNext > ((uuTemp << 32) | n_tmp))
                {
                    uuQuo--;
                    uuTemp += uDen;
                }

                if (uuQuo > 0)
                {
                    uuTemp = 0; // reuse
                    int c2;
                    for (c2 = 0; c2 < cDen; c2++)
                    {
                        uuTemp += den_s[c2] * uuQuo;
                        n_tmp = (uint)uuTemp; // reuse
                        uuTemp >>= 32;
                        if (num_s[cTemp + c2] < n_tmp)
                            uuTemp++;

                        num_s[cTemp + c2] -= n_tmp;
                    }

                    if (h < uuTemp)
                    {
                        n_tmp = 0; // reuse
                        for (c2 = 0; c2 < cDen; c2++)
                        {
                            uuTemp = (ulong)num_s[cTemp + c2] + den_s[c2] + n_tmp; // reuse
                            num_s[cTemp + c2] = (uint)uuTemp;
                            n_tmp = (uint)(uuTemp >> 32);
                        }

                        uuQuo--;
                    }

                    cNum = cTemp + cDen - 1;
                }

                if (divMode)
                    quo[cTemp] = (uint)uuQuo;
            }
        }
        #endregion

        #region div + mod
        private static uint Q_DIVMOD(ulong* num1, ulong* result, uint den, int count)
        {
            if (den == 1)
                return 0;
            else if (count == 0)
            {
                *(uint*)result = *(uint*)num1 / den;
                return *(uint*)num1 % den;
            }

            uint* psrc = (uint*)num1;
            uint* pres = (uint*)result;

            ulong uuTemp = 0, uuDen = den;

            count <<= 1;
            while (--count >= 0)
            {
                uuTemp = (uuTemp << 32) | psrc[count];
                pres[count] = (uint)(uuTemp / uuDen);
                uuTemp %= uuDen;
            }

            return (uint)uuDen;
        }

        public static void DIVMOD(ulong* num1, ulong* num2, ulong* result, int count)
        {
            int d_count = INDEX(num2, count);
            if (d_count < 0)
                throw new DivideByZeroException();
            else if (d_count == 0)
            {
                *(uint*)num1 = Q_DIVMOD(num1, result, *(uint*)num2, count);
                return;
            }

            int n_count = INDEX(num1, count);
            if (n_count == 0)
                return;
            else if (d_count == 1 && n_count == 1)
            {
                *result = *num1 / *num2;
                *num1 %= *num2;
                return;
            }

            ModDivCore((uint*)num1, (uint*)num2, (uint*)result, n_count, d_count, true);
        }

        #endregion

        #region conversion
        public static bool FormatIEEE754(float value, ulong* result, int count)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return false;

            int raw = *(int*)&value;
            bool negative = raw < 0;
            // if (signed && raw < 0)
            // {
            //     ((int*)result)[(count << 1) - 1] = int.MinValue;
            //     raw &= int.MaxValue;
            // }

            int exp = (raw >> 23) - 127;
            int x = exp >> 64;
            int y = exp & 0x3F;
            result[x] = 1u << y;

            int temp = raw & 0x7F_FFFF;
            if (y < 24)
            {
                result[x] |= (uint)temp >> (y = 24 - y);
                if (--x >= 0)
                    result[x] = (uint)temp << (64 - y);
            }
            else
                result[x] |= (uint)temp << (y - 24);

            return negative;
        }

        public static float ToIEEE754(ulong* value)
        {
            int exp =
                127 +
                (value[1] == 0
                ? 63 + BitMath.FLS64(value[1])
                : BitMath.FLS64(*value) - 1);

            if (exp < 127)
                return 0f;

            uint raw = (uint)(exp << 23);

            int x = exp >> 6; // x / 64
            int y = exp & 0x3F; // x % 64
            if (y < 23)
            {
                raw |= (uint)value[x] & ((1u << y) - 1);
                if (--x >= 0)
                    raw |= (uint)(value[x] >> (41 + y)) & ((1u << (23 - y)) - 1);
            }
            else
                raw |= (uint)(value[x] >> (y - 23)) & 0x7F_FFFF;

            return *(float*)&raw;
        }
        #endregion

        #region compare + equals
        public static bool IsZero(ulong* value, int count)
        {
            while (--count >= 0)
                if (value[count] != 0)
                    return false;

            return true;
        }

        public static bool Equals(ulong* left, ulong* right, int count)
        {
            while (--count >= 0)
                if (left[count] != right[count])
                    return false;

            return true;
        }

        public static bool Inequals(ulong* left, ulong* right, int count)
        {
            while (--count >= 0)
                if (left[count] == right[count])
                    return false;

            return true;
        }

        public static int CompareTo_Un(ulong* left, ulong* right, int count)
        {
            int k = left[--count].CompareTo(right[count]);
            while (k == 0 && --count >= 0)
                k = left[count].CompareTo(right[count]);

            return k;
        }

        public static int CompareTo(ulong* left, ulong* right, int count)
        {
            int k = ((long)left[--count]).CompareTo((long)right[count]);
            if (k != 0)
                return k;

            bool neg = (long)left[count] < 0;
            while (k == 0 && --count >= 0)
                k = left[count].CompareTo(right[count]);

            return
                neg
                ? -k
                : k;
        }
        #endregion
    }
}
