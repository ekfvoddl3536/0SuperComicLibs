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

using System.Runtime.CompilerServices;
using m128i = System.Numerics.Vector4;

namespace SuperComicLib
{
    public static unsafe partial class MemoryBlock
    {
        // benchmark results: https://docs.google.com/spreadsheets/d/1EcfnCxDA7ffUkDjn0_N07Ql6xCMpCxQejKQlNnLKRHE
        public static void Clear(byte* ptr, ulong bytes)
        {
            if (bytes >= 0x400)
            {
                _impl_clear1k(ptr, bytes);

                if (((uint)bytes & 7) != 0)
                    *(ulong*)(ptr + bytes - sizeof(long)) = 0;

                return;
            }

            _impl_clearSmall(ptr, bytes);
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _impl_clearSmall(byte* ptr, ulong bytes)
        {
            if ((uint)bytes == 0)
                return;

            ulong stopLoopAtOffset;
            ulong offset = 0;

            // NOTE (ko-kr)::
            //      128 ~ 512 바이트 구간에서 더 효율적이지만, 1024 바이트 또는 그 이상에서
            //      reg64/Fill 방식보다 더 느려지는 현상이 있다.
            if ((uint)bytes >= (uint)sizeof(m128i) * 8)
            {
                m128i xmm0 = m128i.Zero;

                m128i* rdi;

                stopLoopAtOffset = bytes & (ulong)(sizeof(m128i) * -8L);
                do
                {
                    rdi = (m128i*)(ptr + offset);

                    rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm0; rdi[0x00 + 2] = xmm0; rdi[0x00 + 3] = xmm0;
                    rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm0; rdi[0x00 + 6] = xmm0; rdi[0x00 + 7] = xmm0;

                    offset += (ulong)(sizeof(m128i) * 8);
                } while (offset < stopLoopAtOffset);

                rdi = (m128i*)(ptr + bytes - (uint)(sizeof(m128i) * 8));

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm0; rdi[0x00 + 2] = xmm0; rdi[0x00 + 3] = xmm0;
                rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm0; rdi[0x00 + 6] = xmm0; rdi[0x00 + 7] = xmm0;

                return;
            }

            ulong rax = 0;
            if (((uint)bytes & 0x70) != 0) // 16 <= x < 128
            {
                stopLoopAtOffset = bytes & ~15ul;
                do
                {
                    *(ulong*)(ptr + offset + 0) = rax;
                    *(ulong*)(ptr + offset + 8) = rax;
                } while ((offset += 16) < stopLoopAtOffset);

                *(ulong*)(ptr + bytes - 8) = rax;
                *(ulong*)(ptr + bytes - 0) = rax;

                return;
            }

            if (((uint)bytes & 8) != 0)
            {
                *(ulong*)ptr = rax;
                *(ulong*)(ptr + bytes - 8) = rax;
                return;
            }

            if (((uint)bytes & 4) != 0)
            {
                *(uint*)ptr = (uint)rax;
                *(uint*)(ptr + bytes - 4) = (uint)rax;
                return;
            }

            if (((uint)bytes & 2) != 0)
            {
                *(ushort*)ptr = (ushort)rax;
                *(ushort*)(ptr + bytes - 2) = (ushort)rax;
                return;
            }

            *ptr = (byte)rax;
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _impl_clear1k(byte* ptr, ulong bytes)
        {
            ulong r8 = 0;
            Fill((ulong*)ptr, bytes >> 3, r8);
        }
    }
}
