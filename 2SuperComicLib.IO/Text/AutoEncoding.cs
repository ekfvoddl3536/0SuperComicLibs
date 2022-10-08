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
using System.IO;
using System.Text;

namespace SuperComicLib.Text
{
    public static unsafe class AutoEncoding
    {
        public static Encoding Detect(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (info.Exists == false)
                throw new FileNotFoundException();

            byte* ptr = stackalloc byte[4];
            int numRead = 0;

            FileStream stream = info.OpenRead();
            do
            {
                int read = stream.ReadByte();
                if (read < 0)
                    break;

                ptr[numRead++] = (byte)read;
            } while (numRead < 5);
            stream.Close();

            uint data = *(uint*)ptr;
            if (numRead >= 4)
            {
                if (data == 0xFF_FE_00_00)
                    return Encoding.GetEncoding("utf-32BE");
                else if (data == 0xFE_FF)
                    return Encoding.UTF32;
            }

            if (numRead >= 2)
            {
                if (data == 0xFF_FE)
                    return Encoding.BigEndianUnicode;
                else if (data == 0xFE_FF)
                    return Encoding.Unicode;
            }

            if (numRead >= 3)
            {
                if (data == 0xBF_BB_EF)
                    return Encoding.UTF8;
                else if (data == 0x76_2F_2B)
                    return Encoding.UTF7;
            }

            throw new NotSupportedException();
        }
    }
}
