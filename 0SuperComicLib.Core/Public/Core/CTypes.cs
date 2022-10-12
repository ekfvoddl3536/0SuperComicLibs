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

namespace SuperComicLib.Core
{
    public static class CTypes
    {
        public static readonly Type VOID_T = typeof(void);
        public static readonly Type BOOL_T = typeof(bool);
        public static readonly Type SBYTE_T = typeof(sbyte);
        public static readonly Type BYTE_T = typeof(byte);
        public static readonly Type SHORT_T = typeof(short);
        public static readonly Type INT_T = typeof(int);
        public static readonly Type LONG_T = typeof(long);
        public static readonly Type FLOAT_T = typeof(float);
        public static readonly Type DOUBLE_T = typeof(double);
        public static readonly Type STR_T = typeof(string);

        public static readonly Type INTPTR_T = typeof(IntPtr);

        public static readonly Type VALUETYPE_T = typeof(ValueType);
        public static readonly Type OBJECT_T = typeof(object);
    }

    public static class UCTypes
    {
        public static readonly Type SHORT_T = typeof(ushort);
        public static readonly Type INT_T = typeof(uint);
        public static readonly Type LONG_T = typeof(ulong);
    }
}
