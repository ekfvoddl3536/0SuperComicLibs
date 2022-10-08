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

namespace SuperComicLib.Arithmetic
{
    public static unsafe class CustomIntMathExtension
    {
        public static T PowBase2<T>(this T inst, int pow) where T : unmanaged, ICustomInteger
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException(nameof(pow));
            else if (pow >= inst.Length32)
                return default;

            T result;
            if (pow == 0)
                *(uint*)&result = 1;
            else
            {
                result = inst;
                ulong* ptr = (ulong*)&result;
                int max;
                if (BigIntArithmetic.IsZero(ptr, max = inst.Length64))
                    *(uint*)ptr = 1;

                BigIntArithmetic.LSHIFT(ptr, pow, max);
            }

            return result;
        }

        public static T PowBase2_Unsafe<T>(this T inst, int pow) where T : unmanaged, ICustomInteger
        {
            T result;
            if (pow <= 0)
                *(uint*)&result = 1;
            else
            {
                result = inst;
                BigIntArithmetic.LSHIFT((ulong*)&result, pow, inst.Length64);
            }

            return result;
        }
    }
}
