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

namespace SuperComicLib.Text
{
    public static class FRngStringExtension
    {
        public static HashedString ToHashedString(this FRngString value)
        {
#if DEBUG
            if (value is null)
                throw new System.ArgumentNullException(nameof(value));
#endif
            int sidx = value.sidx;
            int cnt = value.eidx - sidx;
            return new HashedString(value.str.GetFixedHashcode(sidx, cnt), cnt);
        }

        public static bool TryToInt32(this FRngString value, out int result)
        {
            string str = value.str;
            int sidx = value.sidx;
            int eidx = value.eidx;

            char first = str[sidx];

            int locr;
            if (char.IsDigit(first))
                locr = first - '0';
            else
            {
                sidx++;
                locr = 0;
            }

            for (; sidx < eidx; sidx++)
            {
                char c = str[sidx];
                if (!char.IsDigit(c))
                {
                    result = 0;
                    return false;
                }
                else
                    locr = locr * 10 + (c - '0');
            }

            result =
                first == '-'
                ? -locr
                : locr;
            return true;
        }

        public static int ToInt32(this FRngString value)
        {
            TryToInt32(value, out int result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ToDecimal(this FRngString value) => decimal.Parse(value.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(this FRngString value) => double.Parse(value.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(this FRngString value) => float.Parse(value.ToString());
    }
}
