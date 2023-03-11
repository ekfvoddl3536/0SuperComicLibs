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

namespace SuperComicLib.Text
{
    public static class HexCharInvalidCheckerExtension
    {
        public static bool IsInvalidHex(this char c)
        {
            // A ~ F(a ~ f), 0 ~ 9
            const int VALID_MAP = 0x7E_03FF;

            int bit = ((c & 0x40) >> 2) | (c & 0xF);

            return ((VALID_MAP >> bit) & 1) == 0;
        }

        public static unsafe bool IsInvalidHexString(this string str)
        {
            const int VALID_MAP = 0x7E_03FF;

            fixed (char* psrc = str)
            {
                char* ptr = psrc;
                for (int i = str.Length; --i >= 0;)
                {
                    int tmp = *ptr++;
                    int bit = ((tmp & 0x40) >> 2) | (tmp & 0xF);

                    if (((VALID_MAP >> bit) & 1) == 0)
                        return true;
                }
            }

            return false;
        }
    }
}
