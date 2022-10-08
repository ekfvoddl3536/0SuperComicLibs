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

#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static unsafe class ptr_math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t and(size_t a, size_t b) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)a.value & (int)b.value
            : (long)a.value & (long)b.value;
#else
            a.value & b.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t or(size_t a, size_t b) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)a.value | (int)b.value
            : (long)a.value | (long)b.value;
#else
            a.value | b.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t xor(size_t a, size_t b) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)a.value ^ (int)b.value
            : (long)a.value ^ (long)b.value;
#else
            a.value ^ b.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isNeg(size_t v) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)v.value < 0
            : (long)v.value < 0;
#elif X86
            (int)v.value < 0;
#else
            (long)v.value < 0;
#endif
    }
}
