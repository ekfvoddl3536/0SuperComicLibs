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

#pragma warning disable IDE1006 // 명명 스타일
using System;

namespace SuperComicLib.Core
{
    public static class PRNG
    {
        private const int B = 8;

        public static int Seed = DateTime.Now.Ticks.GetHashCode();
        private static uint _previous = (uint)Seed;

        private static int _rand(int seed)
        {
            uint value = _previous + 1;

            for (uint u_seed = (uint)seed ^ value, loop = 7; u_seed != 0 && loop-- != 0;)
                unchecked
                {
                    value = value * u_seed + value + 3;

                    u_seed =
                        (u_seed << 1) &
                        ((u_seed & byte.MaxValue) - value * 3);
                }

            _previous = value;
            return (int)value;
        }


        public static decimal NextDecimal() =>
            new decimal(
                (Next() << B) | Next(1 << B),
                (Next() << B) | Next(1 << B),
                (Next() << B) | Next(1 << B),
                (Next() & 1) == 0,
                (byte)Next(29));

        public static double NextDouble() => (uint)Next() / (double)uint.MaxValue;

        /// <returns>1.192093E-7 ~ 1.0</returns>
        public static float NextFloat() => (float)(Next() & 0x7F_FFFF) / 0x7F_FFFF;

        public static int Next(int max) => Next() % max;

        public static int Next(int min, int max) => Next() % max + min;

        public static int NextSign() => -(Next() & 1);

        public static int Next() => _rand(Seed);

        public static unsafe void NextBytes(byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                NextBytes((int*)ptr, (uint)buffer.Length);
        }

        public static unsafe void NextBytes(in NativeSpan<byte> buffer) => 
            NextBytes((int*)buffer.Source, (uint)buffer.Length);

        private static unsafe void NextBytes(int* ptr, uint length)
        {
            for (var k4 = length >> 2; k4-- != 0;)
                *ptr++ = Next();

            int rand = Next();

            byte* pbyte = (byte*)ptr;
            for (var k1 = length & 3; k1-- != 0; rand >>= 8)
                *pbyte = (byte)rand;
        }
    }
}
