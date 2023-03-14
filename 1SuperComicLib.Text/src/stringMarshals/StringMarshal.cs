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

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperComicLib.Text
{
    public static unsafe class StringMarshal
    {
        public static IntPtr ToHGlobalLPSTR(string value, Encoding encoding)
        {
            if (value is null)
                return IntPtr.Zero;

            return ToHGlobalLPSTR_internal(value, encoding ?? Encoding.Default);
        }

        public static IntPtr ToHGlobalLPSTR_internal(string notnull_value, Encoding notnull_encoding)
        {
            int nb = notnull_value.Length + 1;
            long lnb = ((long)nb << 1) + nb; // nb * 3
            nb = (int)lnb;

            if (nb != lnb)
                throw new ArgumentOutOfRangeException(nameof(notnull_value));

            var ptr = Marshal.AllocHGlobal((IntPtr)nb);

            StringToAnsiString_internal(notnull_value, notnull_encoding, (byte*)ptr, nb);
            return ptr;
        }

        public static IntPtr ToCoTaskMemLPSTR(string value, Encoding encoding)
        {
            if (value is null)
                return IntPtr.Zero;

            return ToCoTaskMemLPSTR_internal(value, encoding ?? Encoding.Default);
        }

        public static IntPtr ToCoTaskMemLPSTR_internal(string notnull_value, Encoding notnull_encoding)
        {
            int nb = notnull_value.Length + 1;
            long lnb = ((long)nb << 1) + nb; // nb * 3
            nb = (int)lnb;

            if (nb != lnb)
                throw new ArgumentOutOfRangeException(nameof(notnull_value));

            var ptr = Marshal.AllocCoTaskMem(nb);

            StringToAnsiString_internal(notnull_value, notnull_encoding, (byte*)ptr, nb);
            return ptr;
        }

        private static int StringToAnsiString_internal(string s, Encoding enc, byte* buf, int bufLen)
        {
            fixed (char* first = s)
            {
                int convBytes = enc.GetBytes(first, s.Length, buf, bufLen);
                buf[convBytes] = 0;

                return convBytes;
            }
        }
    }
}
