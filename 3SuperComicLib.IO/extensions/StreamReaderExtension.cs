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

using System.IO;

namespace SuperComicLib
{
    public static class StreamReaderExtension
    {
        public static string MoveNext(this StreamReader reader)
        {
            if (reader.EndOfStream)
                return string.Empty;

            string now;
            do
                now = reader.ReadLine().Trim();
            while (!reader.EndOfStream && now.Length == 0);

            return now;
        }

        public static string MoveNext(this StreamReader reader, ref int count)
        {
            if (reader.EndOfStream)
                return string.Empty;

            string now;
            do
            {
                now = reader.ReadLine().Trim();
                count++;
            }
            while (!reader.EndOfStream && now.Length == 0);

            return now;
        }

        public static bool EndOfLine(this StreamReader sr)
        {
            int read = sr.Peek();
            if (read == '\r')
            {
                sr.Read();
                if (sr.Peek() == '\n')
                    sr.Read();

                return true;
            }
            if (read == '\n')
            {
                sr.Read();
                return true;
            }
            return false;
        }
    }
}
