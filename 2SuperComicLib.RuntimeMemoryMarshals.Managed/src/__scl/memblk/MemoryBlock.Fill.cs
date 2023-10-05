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

using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial class MemoryBlock
    {
        public static void Fill<T>(T* ptr, nuint_t length, in T value) where T : unmanaged
        {
            const uint SMALL_SIZE = 0x1_0000u;

            if (length == 0)
                return;

            ptr[0] = value;

            T* dest = ptr;
            nuint_t next = 1u;
            nuint_t next_cb = (uint)sizeof(T);

            // 2^n 개씩 값 대입
            for (--length; next <= length && (uint)next_cb <= SMALL_SIZE; next <<= 1, next_cb <<= 1)
            {
                length -= next;
                dest = (T*)((byte*)ptr + (long)next_cb);
                Unsafe.CopyBlockUnaligned(dest, ptr, (uint)next_cb);
            }
            for (; next <= length; next <<= 1, next_cb <<= 1)
            {
                length -= next;
                dest = (T*)((byte*)ptr + (long)next_cb);
                Buffer.MemoryCopy(ptr, dest, next_cb, (ulong)next_cb);
            }

            for (; ; )
            {
                for (; next > length; next_cb >>= 1, ptr += (long)next)
                    next >>= 1;

                if (next_cb <= SMALL_SIZE)
                    break;

                length -= next;
                dest = (T*)((byte*)ptr + (long)next_cb);
                Buffer.MemoryCopy(ptr, dest, next_cb, (ulong)next_cb);
            }

            while ((uint)next != 0u)
            {
                length -= next;
                dest = (T*)((byte*)ptr + (long)next_cb);
                Unsafe.CopyBlockUnaligned(dest, ptr, (uint)next_cb);

                for (; next > length; next_cb >>= 1, ptr += (long)next)
                    next >>= 1;
            }
        }
    }
}
