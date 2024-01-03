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
    /// <summary>
    /// (for .NET Framework) Provides hardware acceleration information for the running machine.
    /// </summary>
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

            PCLMULQDQ = AVX << 2,

            FMA = AVX << 3,

            POPCNT = AVX << 4,

            BMI = AVX << 5,
            
            BMI2 = AVX << 6,
        }

        [Flags]
        public enum AVX512Intrinsics : long
        {
            NoHardwareAcceleration = 0,

            AVX512_F = 1,

            AVX512_DQ = AVX512_F << 1,

            AVX512_IFMA = AVX512_F << 2,

            AVX512_PF = AVX512_F << 3,

            AVX512_ER = AVX512_F << 4,

            AVX512_CD = AVX512_F << 5,

            AVX512_BW = AVX512_F << 6,

            AVX512_VL = AVX512_F << 7,

            AVX512_VBMI = AVX512_F << 8,

            AVX512_VBMI2 = AVX512_VBMI << 1,

            AVX512_VNNI = AVX512_VBMI << 2,

            AVX512_BITALG = AVX512_VBMI << 3,

            AVX512_VPOPCNTDQ = AVX512_VBMI << 4,

            AVX512_4VNNIW = AVX512_VBMI << 5,

            AVX512_4FMAPS = AVX512_4VNNIW << 1,

            AVX512_VP2INTERSECT = AVX512_4VNNIW << 2,

            AVX512_FP16 = AVX512_4VNNIW << 3,

            AVX512_BF16 = AVX512_4VNNIW << 4,
        }

        public static readonly Intrinsics Supports;

        public static readonly AVX512Intrinsics AVX512Supports;

        /// <summary>
        /// <code>
        /// 0       :   NoHardwareAcceleration (or, not supported)
        /// 16      :   'xmm' register (128-bit) available
        /// 32      :   'ymm' register (256-bit) available
        /// 64      :   'zmm' register (512-bit) available
        /// </code>
        /// </summary>
        public static readonly long ByteCount;

        static SIMDVector()
        {
            var f512 = _GetFlagSIMDAVX512Extensions();

            AVX512Supports = f512;

            var flags = _GetFlagSIMDExtensions();
            
            Supports = flags;

            ByteCount =
                // AVX-512 available
                f512 != 0
                ? (sizeof(long) << 3)
                // AVX available
                : (flags & (Intrinsics.AVX | Intrinsics.AVX2)) != 0
                ? (sizeof(long) << 2)
                // SSE available
                : (flags != 0)
                ? (sizeof(long) << 1)
                : 0;
        }

        [SkipLocalsInit]
        private static AVX512Intrinsics _GetFlagSIMDAVX512Extensions()
        {
            // ref.: https://en.wikipedia.org/wiki/CPUID
            var cpuid = X86Function._CPUID(7, 0);

            var tmp = cpuid.rbx;
            long result =
                ((tmp >> (16 - 00)) & 0x03) | // f, dq
                ((tmp >> (21 - 02)) & 0x04) | // ifma
                ((tmp >> (26 - 03)) & 0x38) | // pf, er, cd
                ((tmp >> (30 - 06)) & 0xC0);  // bw, vl

            tmp = cpuid.rcx;
            result |=
                ((tmp << (08 - 01)) & 0x01_00) | // vbmi
                ((tmp << (09 - 06)) & 0x02_00) | // vbmi2
                ((tmp >> (11 - 10)) & 0x0C_00) | // vnni, bitalg
                ((tmp >> (14 - 12)) & 0x10_00);  // vpopcntdq

            tmp = cpuid.rdx;
            result |=
                ((tmp << (13 - 02)) & 0x0_60_00) | // 4vnniw, 4fmaps
                ((tmp << (15 - 08)) & 0x0_80_00) | // vp2intersect
                ((tmp >> (23 - 16)) & 0x1_00_00);  // fp16

            cpuid = X86Function._CPUID(7, 1);
            result |= (cpuid.rax << (17 - 05)) & 0x2_00_00; // bf16

            return (AVX512Intrinsics)result;
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

            // PCLMULQDQ, FMA, POPCNT
            result |=
                ((tmp << 07) & 0x100) |
                ((tmp >> 03) & 0x200) |
                ((tmp >> 13) & 0x400);

            cpuid = X86Function._CPUID(7, 0);

            // AVX2
            result |= (cpuid.rbx << 2) & 0x80;

            // BMI1, BMI2
            tmp = cpuid.rbx;
            result |=
                ((tmp << 08) & 0x0800) |
                ((tmp << 04) & 0x1000);

            return (Intrinsics)result;
        }
    }
}
