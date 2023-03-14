// MIT License
//
// Copyright (c) 2020-2023. SuperComic (ekfvoddl3535@naver.com)
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

namespace SuperComicLib.Runtime
{
    public static class FsInt
    {
        public static int Parse(char value) => value - '0';

        public static bool TryParse(char value, out int result)
        {
            if (value >= '0' && value <= '9')
            {
                result = value - '0';
                return true;
            }

            result = 0;
            return false;
        }

        public static int Parse(string value)
        {
            int len = value.Length - 1;

            int result = 0;
            int e = 1;
            for (int x = len; x >= 0 && e > 0; x--, e = 10 * (len - x))
                result += (value[x] - '0') * e;

            return result;
        }

        public static bool TryParse(string value, out int result)
        {
            int len = value.Length - 1;

            result = 0;
            int e = 1;
            for (int x = len; x >= 0 && e > 0; x--, e = 10 * (len - x))
                if (TryParse(value[x], out int tmp))
                    result += tmp * e;
                else
                    return false;

            return true;
        }

        public static int Parse(string value, int idx)
        {
            int len = value.Length;

            int result = 0;
            int e = 1;
            for (int x = idx; x < len && e > 0; x++, e = 10 * (x - idx))
                if (TryParse(value[x], out int tmp))
                    result = (result + tmp) * e;
                else
                    break;

            return result;
        }

        public static int ParseHex(char value) =>
            value >= 'A' && value <= 'F' || value >= 'a' && value <= 'f'
            ? (value & 7) + 9
            : value >= '0' && value <= '9'
            ? value - '0'
            : -1;
    }
}
