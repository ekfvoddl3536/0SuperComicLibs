// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

using SuperComicLib.RuntimeMemoryMarshals;

namespace SuperComicLib
{
    unsafe partial class MemoryBlock
    {
        public static void Fill<T>(T* ptr, ulong length, in T value) where T : unmanaged
        {
            if (sizeof(T) == 1)
            {
                Fill_SZ1((byte*)ptr, length, ILUnsafe.ReadOnlyAs<T, byte>(value));
                return;
            }

            var cb = (ulong)sizeof(T) << 3;
            for (var i = length >> 3; i != 0; --i)
            {
                ptr[0] = ptr[1] = ptr[2] = ptr[3] =
                ptr[4] = ptr[5] = ptr[6] = ptr[7] = value;

                ptr = (T*)((byte*)ptr + cb);
            }

            if (((uint)length & 4) != 0)
            {
                ptr[0] = ptr[1] = ptr[2] = ptr[3] = value;

                ptr += 4;
                length -= 4;
            }

            if (((uint)length & 2) != 0)
            {
                ptr[0] = ptr[1] = value;

                ptr += 2;
                length -= 2;
            }

            if (((uint)length & 1) != 0)
                *ptr = value;
        }

        private static void Fill_SZ1(byte* ptr, ulong length, byte value)
        {
            const uint SZ_2GiB = 0x8000_0000u;
            const long ALIGN8 = 0x7u;

            // pointer = 'unknown', number of bytes = 'unknown'
            var aligned = ((long)ptr + ALIGN8) & ~ALIGN8;

            var cb = CMath.Min((ulong)(aligned - (long)ptr), length);

            ILUnsafe.InitBlockUnaligned(ptr, 0, (uint)cb);

            length -= cb;

            // pointer = 'aligned', number of bytes = 'alilgned'
            for (var i = length >> 31; i != 0; --i)
            {
                ILUnsafe.InitBlock((byte*)aligned, value, SZ_2GiB);
                aligned += SZ_2GiB;
            }

            // pointer = 'aligned', number of bytes = 'unaligned'
            ILUnsafe.InitBlock((byte*)aligned, value, (uint)length & 0x7FFF_FFF8u);
        }
    }
}
