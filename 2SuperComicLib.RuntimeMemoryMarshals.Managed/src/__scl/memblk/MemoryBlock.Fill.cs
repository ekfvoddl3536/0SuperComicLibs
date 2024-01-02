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
using SuperComicLib.RuntimeMemoryMarshals;
using m128i = System.Numerics.Vector4;

namespace SuperComicLib
{
    unsafe partial class MemoryBlock
    {
        // optimize target = .NET Framework X64 RyuJIT CoreCLR runtime<br/>
        // ref.: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs,22fac8ad1f92d5ab,references
        // benchmark results: https://docs.google.com/spreadsheets/d/1EcfnCxDA7ffUkDjn0_N07Ql6xCMpCxQejKQlNnLKRHE
        [SkipLocalsInit]
        public static void Fill<T>(T* ptr, ulong elementLength, in T value) where T : unmanaged
        {
            const int VECTOR_SIZE = sizeof(long) * 16;

            if (sizeof(T) > VECTOR_SIZE) goto CannotVectorize;
            if ((sizeof(T) & (sizeof(T) - 1)) != 0) goto CannotVectorize;

            ulong i;
            if (sizeof(T) < sizeof(m128i))
            {
                if (elementLength >= (uint)(4 * sizeof(m128i) / sizeof(T)))
                {
                    Range64 xmmTemp;

                    // broadcast
                    i = sizeof(T) == sizeof(byte)
                        ? ILUnsafe.ReadOnlyAs<T, byte>(in value).BroadcastToUint64()
                        : sizeof(T) == sizeof(short)
                        ? ILUnsafe.ReadOnlyAs<T, ushort>(in value).BroadcastToUint64()
                        : sizeof(T) == sizeof(int)
                        ? ILUnsafe.ReadOnlyAs<T, uint>(in value).BroadcastToUint64()
                        : ILUnsafe.ReadOnlyAs<T, ulong>(in value);

                    xmmTemp.start = (long)i;
                    xmmTemp.end = (long)i;

                    Fill_XMM1cVectorize_GR((byte*)ptr, elementLength * (ulong)sizeof(T), ref ILUnsafe.As<Range64, m128i>(ref xmmTemp));
                }
                else
                    goto CannotVectorize;

                return;
            }
            else if (sizeof(T) == sizeof(m128i))
            {
                Fill_XMM1cVectorize_SSE((byte*)ptr, elementLength, ref ILUnsafe.AsRef<T, m128i>(in value));
                return;
            }
            else if (sizeof(T) == sizeof(m128i) * 2)
            {
                Fill_XMM2cVectorize((byte*)ptr, elementLength, ref ILUnsafe.AsRef<T, m128i>(in value));
                return;
            }
            else if (sizeof(T) == sizeof(m128i) * 4)
            {
                Fill_XMM4cVectorize((byte*)ptr, elementLength, ref ILUnsafe.AsRef<T, m128i>(in value));
                return;
            }
            else if (sizeof(T) == sizeof(m128i) * 8)
            {
                Fill_XMM8cVectorize((byte*)ptr, elementLength, ref ILUnsafe.AsRef<T, m128i>(in value));
                return;
            }

        CannotVectorize:
            // setup
            i = 0;
            T tmp = value;

            if (elementLength >= 8)
            {
                ulong end = elementLength & ~7ul;
                do
                {
                    ptr[i + 0] = tmp; ptr[i + 1] = tmp;
                    ptr[i + 2] = tmp; ptr[i + 3] = tmp;
                    ptr[i + 4] = tmp; ptr[i + 5] = tmp;
                    ptr[i + 6] = tmp; ptr[i + 7] = tmp;
                } while ((i += 8) < end);
            }

            if (((uint)elementLength & 4) != 0)
            {
                ptr[i + 0] = tmp; ptr[i + 1] = tmp;
                ptr[i + 2] = tmp; ptr[i + 3] = tmp;

                i += 4;
            }

            if (((uint)elementLength & 2) != 0)
            {
                ptr[i + 0] = tmp; ptr[i + 1] = tmp;

                i += 2;
            }
            
            if (((uint)elementLength & 1) != 0)
                ptr[i] = tmp;
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Fill_XMM1cVectorize_SSE(byte* ptr, ulong length, ref m128i value)
        {
            m128i* rdi;

            m128i xmm0 = value;

            ulong offset = 0;

            if (length >= 8)
            {
                ulong stopLoopAtOffset = (length * (ulong)sizeof(m128i)) & (ulong)(8L * -sizeof(m128i));
                do
                {
                    rdi = (m128i*)(ptr + offset);

                    rdi[0] = xmm0; rdi[1] = xmm0; rdi[2] = xmm0; rdi[3] = xmm0;
                    rdi[4] = xmm0; rdi[5] = xmm0; rdi[6] = xmm0; rdi[7] = xmm0;

                    offset += (ulong)(8 * sizeof(m128i));
                } while (offset < stopLoopAtOffset);
            }

            if (((uint)length & 4) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0] = xmm0; rdi[1] = xmm0; rdi[2] = xmm0; rdi[3] = xmm0;

                offset += (ulong)(4 * sizeof(m128i));
            }

            if (((uint)length & 2) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0] = xmm0; rdi[1] = xmm0;

                offset += (ulong)(2 * sizeof(m128i));
            }

            if (((uint)length & 1) != 0)
                *(m128i*)(ptr + offset) = xmm0;
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Fill_XMM1cVectorize_GR(byte* ptr, ulong byteLength, ref m128i value)
        {
            m128i* rdi;

            m128i xmm0 = value;

            if (byteLength >= (uint)(8 * sizeof(m128i)))
            {
                ulong stopLoopAtOffset = byteLength & (ulong)(8L * -sizeof(m128i));
                ulong offset = 0;

                do
                {
                    rdi = (m128i*)(ptr + offset);

                    rdi[0] = xmm0; rdi[1] = xmm0; rdi[2] = xmm0; rdi[3] = xmm0;
                    rdi[4] = xmm0; rdi[5] = xmm0; rdi[6] = xmm0; rdi[7] = xmm0;

                    offset += (ulong)(8 * sizeof(m128i));
                } while (offset < stopLoopAtOffset);

                rdi = (m128i*)(ptr + byteLength - (ulong)(8 * sizeof(m128i)));

                rdi[0] = xmm0; rdi[1] = xmm0; rdi[2] = xmm0; rdi[3] = xmm0;
                rdi[4] = xmm0; rdi[5] = xmm0; rdi[6] = xmm0; rdi[7] = xmm0;

                return;
            }

            rdi = (m128i*)ptr;

            rdi[00] = xmm0; rdi[01] = xmm0; rdi[02] = xmm0; rdi[03] = xmm0;

            rdi = (m128i*)(ptr + byteLength - (ulong)(4 * sizeof(m128i)));

            rdi[00] = xmm0; rdi[01] = xmm0; rdi[02] = xmm0; rdi[03] = xmm0;
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Fill_XMM2cVectorize(byte* ptr, ulong length, ref m128i value)
        {
            const uint T_SIZE = sizeof(ulong) * 4;
            const ulong T_NEXT = 8 * T_SIZE;
            const ulong T_MASK = ~(T_NEXT - 1);

            ulong totalByteLength = length * T_SIZE;
            ulong offset = 0;

            m128i* rdi;

            m128i xmm0 = value;
            m128i xmm1 = ILUnsafe.Add(ref value, 1);

            if (length >= 8)
            {
                ulong stopLoopAtOffset = (length * T_SIZE) & T_MASK;
                do
                {
                    // hard-optimization (Don't make method)
                    rdi = (m128i*)(ptr + offset);

                    rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm0; rdi[0x00 + 3] = xmm1;
                    rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm1; rdi[0x00 + 6] = xmm0; rdi[0x00 + 7] = xmm1;

                    rdi[0x08 + 0] = xmm0; rdi[0x08 + 1] = xmm1; rdi[0x08 + 2] = xmm0; rdi[0x08 + 3] = xmm1;
                    rdi[0x08 + 4] = xmm0; rdi[0x08 + 5] = xmm1; rdi[0x08 + 6] = xmm0; rdi[0x08 + 7] = xmm1;

                    offset += T_NEXT;
                } while (offset < stopLoopAtOffset);
            }

            if (((uint)length & 4) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm0; rdi[0x00 + 3] = xmm1;
                rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm1; rdi[0x00 + 6] = xmm0; rdi[0x00 + 7] = xmm1;

                offset += T_NEXT >> 1;
            }

            if (((uint)length & 2) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm0; rdi[0x00 + 3] = xmm1;

                offset += T_NEXT >> 2;
            }

            // hard-optimization (Don't make method)
            if (((uint)length & 1) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1;
            }
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Fill_XMM4cVectorize(byte* ptr, ulong length, ref m128i value)
        {
            const uint T_SIZE = sizeof(ulong) * 8;
            const ulong T_NEXT = 8 * T_SIZE;
            const ulong T_MASK = ~(T_NEXT - 1);

            ulong totalByteLength = length * T_SIZE;
            ulong offset = 0;

            m128i* rdi;

            m128i xmm0 = value;
            m128i xmm1 = ILUnsafe.Add(ref value, 1);
            m128i xmm2 = ILUnsafe.Add(ref value, 2);
            m128i xmm3 = ILUnsafe.Add(ref value, 3);

            if (length >= 8)
            {
                ulong stopLoopAtOffset = (length * T_SIZE) & T_MASK;
                do
                {
                    // hard-optimization (Don't make method)
                    rdi = (m128i*)(ptr + offset);

                    rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                    rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm1; rdi[0x00 + 6] = xmm2; rdi[0x00 + 7] = xmm3;

                    rdi[0x08 + 0] = xmm0; rdi[0x08 + 1] = xmm1; rdi[0x08 + 2] = xmm2; rdi[0x08 + 3] = xmm3;
                    rdi[0x08 + 4] = xmm0; rdi[0x08 + 5] = xmm1; rdi[0x08 + 6] = xmm2; rdi[0x08 + 7] = xmm3;

                    rdi[0x10 + 0] = xmm0; rdi[0x10 + 1] = xmm1; rdi[0x10 + 2] = xmm2; rdi[0x10 + 3] = xmm3;
                    rdi[0x10 + 4] = xmm0; rdi[0x10 + 5] = xmm1; rdi[0x10 + 6] = xmm2; rdi[0x10 + 7] = xmm3;

                    rdi[0x18 + 0] = xmm0; rdi[0x18 + 1] = xmm1; rdi[0x18 + 2] = xmm2; rdi[0x18 + 3] = xmm3;
                    rdi[0x18 + 4] = xmm0; rdi[0x18 + 5] = xmm1; rdi[0x18 + 6] = xmm2; rdi[0x18 + 7] = xmm3;

                    offset += T_NEXT;
                } while (offset < stopLoopAtOffset);
            }

            if (((uint)length & 4) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm1; rdi[0x00 + 6] = xmm2; rdi[0x00 + 7] = xmm3;

                rdi[0x08 + 0] = xmm0; rdi[0x08 + 1] = xmm1; rdi[0x08 + 2] = xmm2; rdi[0x08 + 3] = xmm3;
                rdi[0x08 + 4] = xmm0; rdi[0x08 + 5] = xmm1; rdi[0x08 + 6] = xmm2; rdi[0x08 + 7] = xmm3;

                offset += T_NEXT >> 1;
            }

            if (((uint)length & 2) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                rdi[0x00 + 4] = xmm0; rdi[0x00 + 5] = xmm1; rdi[0x00 + 6] = xmm2; rdi[0x00 + 7] = xmm3;

                offset += T_NEXT >> 2;
            }

            // hard-optimization (Don't make method)
            if (((uint)length & 1) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
            }
        }

        [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Fill_XMM8cVectorize(byte* ptr, ulong length, ref m128i value)
        {
            const uint T_SIZE = sizeof(ulong) * 16;
            const ulong T_NEXT = 8 * T_SIZE;
            const ulong T_MASK = ~(T_NEXT - 1);

            ulong totalByteLength = length * T_SIZE;
            ulong offset = 0;

            m128i* rdi;

            m128i xmm0 = value;
            m128i xmm1 = ILUnsafe.Add(ref value, 1);
            m128i xmm2 = ILUnsafe.Add(ref value, 2);
            m128i xmm3 = ILUnsafe.Add(ref value, 3);
            m128i xmm4 = ILUnsafe.Add(ref value, 4);
            m128i xmm5 = ILUnsafe.Add(ref value, 5);
            m128i xmm6 = ILUnsafe.Add(ref value, 6);
            m128i xmm7 = ILUnsafe.Add(ref value, 7);
            m128i xmm8 = ILUnsafe.Add(ref value, 8);

            if (length >= 8)
            {
                ulong stopLoopAtOffset = (length * T_SIZE) & T_MASK;
                do
                {
                    // hard-optimization (Don't make method)
                    rdi = (m128i*)(ptr + offset);

                    rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                    rdi[0x00 + 4] = xmm4; rdi[0x00 + 5] = xmm5; rdi[0x00 + 6] = xmm6; rdi[0x00 + 7] = xmm7;

                    rdi[0x08 + 0] = xmm0; rdi[0x08 + 1] = xmm1; rdi[0x08 + 2] = xmm2; rdi[0x08 + 3] = xmm3;
                    rdi[0x08 + 4] = xmm4; rdi[0x08 + 5] = xmm5; rdi[0x08 + 6] = xmm6; rdi[0x08 + 7] = xmm7;

                    rdi[0x10 + 0] = xmm0; rdi[0x10 + 1] = xmm1; rdi[0x10 + 2] = xmm2; rdi[0x10 + 3] = xmm3;
                    rdi[0x10 + 4] = xmm4; rdi[0x10 + 5] = xmm5; rdi[0x10 + 6] = xmm6; rdi[0x10 + 7] = xmm7;

                    rdi[0x18 + 0] = xmm0; rdi[0x18 + 1] = xmm1; rdi[0x18 + 2] = xmm2; rdi[0x18 + 3] = xmm3;
                    rdi[0x18 + 4] = xmm4; rdi[0x18 + 5] = xmm5; rdi[0x18 + 6] = xmm6; rdi[0x18 + 7] = xmm7;

                    rdi[0x20 + 0] = xmm0; rdi[0x20 + 1] = xmm1; rdi[0x20 + 2] = xmm2; rdi[0x20 + 3] = xmm3;
                    rdi[0x20 + 4] = xmm4; rdi[0x20 + 5] = xmm5; rdi[0x20 + 6] = xmm6; rdi[0x20 + 7] = xmm7;

                    rdi[0x28 + 0] = xmm0; rdi[0x28 + 1] = xmm1; rdi[0x28 + 2] = xmm2; rdi[0x28 + 3] = xmm3;
                    rdi[0x28 + 4] = xmm4; rdi[0x28 + 5] = xmm5; rdi[0x28 + 6] = xmm6; rdi[0x28 + 7] = xmm7;

                    rdi[0x30 + 0] = xmm0; rdi[0x30 + 1] = xmm1; rdi[0x30 + 2] = xmm2; rdi[0x30 + 3] = xmm3;
                    rdi[0x30 + 4] = xmm4; rdi[0x30 + 5] = xmm5; rdi[0x30 + 6] = xmm6; rdi[0x30 + 7] = xmm7;

                    rdi[0x38 + 0] = xmm0; rdi[0x38 + 1] = xmm1; rdi[0x38 + 2] = xmm2; rdi[0x38 + 3] = xmm3;
                    rdi[0x38 + 4] = xmm4; rdi[0x38 + 5] = xmm5; rdi[0x38 + 6] = xmm6; rdi[0x38 + 7] = xmm7;

                    offset += T_NEXT;
                } while (offset < stopLoopAtOffset);
            }

            if (((uint)length & 4) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                rdi[0x00 + 4] = xmm4; rdi[0x00 + 5] = xmm5; rdi[0x00 + 6] = xmm6; rdi[0x00 + 7] = xmm7;

                rdi[0x08 + 0] = xmm0; rdi[0x08 + 1] = xmm1; rdi[0x08 + 2] = xmm2; rdi[0x08 + 3] = xmm3;
                rdi[0x08 + 4] = xmm4; rdi[0x08 + 5] = xmm5; rdi[0x08 + 6] = xmm6; rdi[0x08 + 7] = xmm7;

                rdi[0x10 + 0] = xmm0; rdi[0x10 + 1] = xmm1; rdi[0x10 + 2] = xmm2; rdi[0x10 + 3] = xmm3;
                rdi[0x10 + 4] = xmm4; rdi[0x10 + 5] = xmm5; rdi[0x10 + 6] = xmm6; rdi[0x10 + 7] = xmm7;

                rdi[0x18 + 0] = xmm0; rdi[0x18 + 1] = xmm1; rdi[0x18 + 2] = xmm2; rdi[0x18 + 3] = xmm3;
                rdi[0x18 + 4] = xmm4; rdi[0x18 + 5] = xmm5; rdi[0x18 + 6] = xmm6; rdi[0x18 + 7] = xmm7;

                offset += T_NEXT >> 1;
            }

            if (((uint)length & 2) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                rdi[0x00 + 4] = xmm4; rdi[0x00 + 5] = xmm5; rdi[0x00 + 6] = xmm6; rdi[0x00 + 7] = xmm7;

                rdi[0x08 + 0] = xmm0; rdi[0x08 + 1] = xmm1; rdi[0x08 + 2] = xmm2; rdi[0x08 + 3] = xmm3;
                rdi[0x08 + 4] = xmm4; rdi[0x08 + 5] = xmm5; rdi[0x08 + 6] = xmm6; rdi[0x08 + 7] = xmm7;

                offset += T_NEXT >> 2;
            }

            // hard-optimization (Don't make method)
            if (((uint)length & 1) != 0)
            {
                rdi = (m128i*)(ptr + offset);

                rdi[0x00 + 0] = xmm0; rdi[0x00 + 1] = xmm1; rdi[0x00 + 2] = xmm2; rdi[0x00 + 3] = xmm3;
                rdi[0x00 + 4] = xmm4; rdi[0x00 + 5] = xmm5; rdi[0x00 + 6] = xmm6; rdi[0x00 + 7] = xmm7;
            }
        }
    }
}
