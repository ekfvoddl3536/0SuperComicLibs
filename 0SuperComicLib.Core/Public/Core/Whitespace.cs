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

namespace SuperComicLib.Core
{
    public static class Whitespace
    {
        public static readonly string w1 = Make(1);
        public static readonly string w2 = Make(2);
        public static readonly string w4 = Make(4);
        public static readonly string w8 = Make(8);

        /// <summary>
        /// using <see cref="Get(int)"/> method instead
        /// </summary>
        public static string Make(int cnt)
        {
            string str = string.Empty;
            for (; cnt > 0; cnt--)
                str += " ";

            return str;
        }

        /// <summary>
        /// better than <see cref="Make(int)"/> method.
        /// </summary>
        public static string Get(int cnt)
        {
            string str = string.Empty;
            for (; cnt > 8; cnt -= 8)
                str += w8;

            if (cnt > 4)
            {
                str += w4;
                cnt -= 4;
            }
            if (cnt > 2)
            {
                str += w2;
                cnt -= 2;
            }
            if (cnt != 0)
                str += w1;

            return str;
        }
    }
}
