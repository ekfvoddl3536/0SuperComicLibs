// The MIT License (MIT)
//
// Copyright (c) 2023-2024. SuperComic (ekfvoddl3535@naver.com)
// Copyright (c) .NET Foundation and Contributors
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

#pragma warning disable IDE1006
#pragma warning disable CS1591

namespace SuperComicLib
{
    public static class X64ABI
    {
        public enum CALLCONV
        {
            /// <summary>
            /// Unknown
            /// </summary>
            INVALID,

            /// <summary>
            /// Microsoft x64 ABI
            /// <para/>
            /// RCX, RDX, R8, R9
            /// </summary>
            MS_ABI,

            /// <summary>
            /// System V AMD64 ABI
            /// <para/>
            /// RDI, RSI, RDX, RCX, R8, R9
            /// </summary>
            SYS_V_ABI,
        }

        public static readonly CALLCONV CallingConvention = _GetCallConv();

        private static CALLCONV _GetCallConv() => throw new System.PlatformNotSupportedException();
    }
}
