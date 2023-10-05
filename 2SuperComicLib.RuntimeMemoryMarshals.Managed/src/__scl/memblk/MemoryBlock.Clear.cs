// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

namespace SuperComicLib
{
    public static unsafe partial class MemoryBlock
    {
        public static void Clear(byte* ptr, nuint_t bytes)
        {
            const uint ALIGN8 = 0x7u;

            if (bytes == 0)
                return;

            switch ((uint)bytes & ALIGN8)
            {
                case 1:
                    *ptr = 0;
                    break;

                case 2:
                    *(ushort*)ptr = 0;
                    break; 

                case 3:
                    *(ushort*)ptr = 0;
                    ptr[sizeof(ushort)] = 0;
                    break;

                case 4:
                    *(uint*)ptr = 0;
                    break;

                case 5:
                    *(uint*)ptr = 0;
                    ptr[sizeof(uint)] = 0;
                    break;

                case 6:
                    *(uint*)ptr = 0;
                    *(ushort*)(ptr + sizeof(uint)) = 0;
                    break;

                case 7:
                    *(uint*)ptr = 0;
                    *(ushort*)(ptr + sizeof(uint)) = 0;
                    ptr[sizeof(uint) + sizeof(ushort)] = 0;
                    break;

                default:
                    break;
            }

            ptr += (uint)bytes & ALIGN8;

            var nb = bytes & ~ALIGN8;
            if ((nb & sizeof(long)) != 0)
            {
                *(long*)ptr = 0;
                
                ptr += sizeof(long);
                nb -= sizeof(long);
            }

            // aligned 16
            if (nb > 0x300u) // 768
                ClearLarge768_internal(ptr, nb);
            else if (nb != 0)
                Unsafe.InitBlock(ptr, 0, (uint)nb);
        }

        private static void ClearLarge768_internal(byte* ptr, nuint_t nb)
        {
            const uint SZ_300 = 0x300u;
            const uint SZ_2GB = 1u << 31;
            
            Unsafe.InitBlock(ptr, 0, SZ_300);

            ptr += SZ_300;
            nb -= SZ_300;

            while (nb != 0)
            {
                var cb1 = CMathi.Min(nb, SZ_2GB);

                Unsafe.InitBlock(ptr, 0, (uint)cb1);

                ptr += (uint)cb1;
                nb -= cb1;
            }
        }
    }
}
