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

//namespace SuperComicLib.Arithmetic
//{
//    internal static class HalfToString
//    {
//        public static void Integer(StringBuilder sb, int exp, int man)
//        {
//            int i = man;
//            if (exp > 0)
//                i <<= exp;
//            else if (exp < -10)
//            {
//                sb.Append('0');
//                return;
//            }
//            else if (exp < 0)
//                i >>= -exp;

//            char[] vs = new char[5]; // 65504
//            int k = 0;
//            while (i != 0)
//            {
//                vs[k++] = (char)(i % 10 + '0');
//                i /= 10;
//            }

//            while (--k >= 0)
//                sb.Append(vs[k]);
//        }

//        public static void Mantissa(StringBuilder sb, int exp, int man)
//        {
//            int shift = 28 + exp; // 32 + exp - 4 -> (32 - 4) + exp

//            if (shift >= 28 || shift < 0)
//                return; // no mantissa

//            sb.Append('.');

//            StringBuilder sb2 = new StringBuilder();
//            uint m = (uint)man << shift;

//            int v = 4;
//            while (m != 0 && v > 0)
//            {
//                m = (m & 0x0FFF_FFFF) * 10;

//                uint digit = m >> 28;
//                sb2.Append((char)(digit + '0'));

//                if (digit != 0 || v != 3)
//                    v--;
//            }

//            v = sb2.Length - 1;
//            while (v > 0 && sb2[v] == '0')
//                v--;

//            for (exp = 0; exp <= v;)
//                sb.Append(sb2[exp++]);
//        }
//    }
//}
