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
using System.Reflection;

namespace SuperComicLib
{
    public static class DataObject_Helper
    {
        public static readonly char[] split_char_0 = new char[] { '=' };
        public static readonly char[] split_char_1 = new char[] { ' ' };
        public static readonly char[] split_char_2 = new char[] { ':' };

        public static readonly string[] split_nl = new string[] { Environment.NewLine };

        public const StringSplitOptions opt = StringSplitOptions.RemoveEmptyEntries;
        public const BindingFlags stbflag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        public const string sbool = "bool";
        public const string s32 = "int";
        public const string s64 = "long";
        public const string sr4 = "float";
        public const string sr8 = "double";
        public const string sstr = "string";
    }
}
