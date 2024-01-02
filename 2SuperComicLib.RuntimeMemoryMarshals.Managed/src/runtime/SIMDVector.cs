// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    public static class SIMDVector
    {
        [Flags]
        public enum Intrinsics : long
        {
            NoHardwareAcceleration = 0,

            SSE = 1,

            SSE2 = SSE << 1,

            SSE3 = SSE << 2,

            SSSE3 = SSE << 3,

            SSE4_1 = SSE << 4,

            SSE4_2 = SSE << 5,

            AVX = SSE << 6,

            AVX2 = AVX << 1,
        }

        public static readonly Intrinsics Supports;
        public static readonly long ByteCount;

        static SIMDVector()
        {
            var flags = _GetFlagSIMDExtensions();
            
            Supports = flags;

            ByteCount =
                (flags & (Intrinsics.AVX | Intrinsics.AVX2)) != 0
                ? sizeof(long) * 4
                : (flags != 0)
                ? sizeof(long) * 2
                : 0;
        }

        [SkipLocalsInit]
        private static Intrinsics _GetFlagSIMDExtensions()
        {
            var cpuid = X86Function._CPUID(1);

            // SSE, SSE2
            long result = (cpuid.rdx >> 25) & 0b11;

            // SSE3, SSSE3, SSE4.1, SSE4.2
            var tmp = cpuid.rcx;
            result |=
                ((tmp << 02) & 0x04) |
                ((tmp >> 06) & 0x08) |
                ((tmp >> 15) & 0x30) |
                ((tmp >> 22) & 0x40);

            cpuid = X86Function._CPUID(7, 0);
            result |= (cpuid.rbx << 2) & 0x80;

            return (Intrinsics)result;
        }
    }
}
