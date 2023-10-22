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
    public static unsafe partial class MemoryBlock
    {
        public static void Clear(byte* ptr, ulong bytes)
        {
            const ulong SZ_300 = 0x300u;
            const long ALIGN8 = 0x7u;

            if (bytes == 0)
                return;

            // pointer = 'unknown', number of bytes = 'unknown'
            var aligned = ((long)ptr + ALIGN8) & ~ALIGN8;

            var cb = CMath.Min((ulong)(aligned - (long)ptr), bytes);

            ILUnsafe.InitBlockUnaligned(ptr, 0, (uint)cb);

            bytes -= cb;

            cb = bytes & unchecked((ulong)~ALIGN8);

            // pointer = 'aligned', number of bytes = 'alilgned'
            if (cb > SZ_300)
                aligned = (long)ClearLarge768_internal((byte*)aligned, cb);
            else if (cb != 0)
            {
                ILUnsafe.InitBlock((byte*)aligned, 0, (uint)cb);
                aligned += (long)cb;
            }

            // pointer = 'aligned', number of bytes = 'unaligned'
            ILUnsafe.InitBlock((byte*)aligned, 0, (uint)(bytes & ALIGN8));
        }

        private static byte* ClearLarge768_internal(byte* ptr, ulong nb)
        {
            const ulong SZ_300 = 0x300u;
            const ulong SZ_2GB = 1u << 31;

            ILUnsafe.InitBlock(ptr, 0, (uint)SZ_300);

            ptr += SZ_300;
            nb -= SZ_300;

            while (nb != 0)
            {
                var cb1 = CMath.Min(nb, SZ_2GB);

                ILUnsafe.InitBlock(ptr, 0, (uint)cb1);

                ptr += cb1;
                nb -= cb1;
            }

            return ptr;
        }
    }
}
