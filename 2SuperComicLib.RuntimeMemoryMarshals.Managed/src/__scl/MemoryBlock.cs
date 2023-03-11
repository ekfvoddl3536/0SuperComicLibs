
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
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    public static unsafe class MemoryBlock
    {
        #region memset
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void __memset15<T>(T* dest, uint count, T value) where T : unmanaged
        {
            if ((count & 8) != 0)
            {
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
            }

            if ((count & 4) != 0)
            {
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
            }

            if ((count & 2) != 0)
            {
                *dest++ = value;
                *dest++ = value;
            }

            if ((count & 1) != 0)
                *dest = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset<T>(T* dest, nint_t count, T value) where T : unmanaged
        {
            for (var i = (nuint_t)count >> 4; i != 0; --i)
            {
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;

                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
                *dest++ = value; *dest++ = value;
            }

            __memset15(dest, (uint)count, value);
        }
        #endregion

        #region clear
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void __clear511b(Block64* ptr, uint sizeInBytes)
        {
            if ((sizeInBytes & 0x100) != 0)
            {
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
            }

            if ((sizeInBytes & 0x80) != 0)
            {
                *ptr++ = default;
                *ptr++ = default;
            }

            if ((sizeInBytes & 0x40) != 0)
                *ptr++ = default;

            Block16* p2 = (Block16*)ptr;
            if ((sizeInBytes & 0x20) != 0)
            {
                *p2++ = default;
                *p2++ = default;
            }

            if ((sizeInBytes & 0x10) != 0)
                *p2++ = default;

            uint* p3 = (uint*)p2;
            if ((sizeInBytes & 0x8) != 0)
            {
                *p3++ = default;
                *p3++ = default;
            }

            if ((sizeInBytes & 0x4) != 0)
                *p3++ = default;

            var dst = (byte*)p3;
            var end = dst + (sizeInBytes & 0x3);
            if (dst != end)
                *dst++ = default;

            if (dst != end)
                *dst++ = default;

            if (dst != end)
                *dst++ = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Block64* __clear512b(Block64* pdst, uint count)
        {
            for (uint i = count; i != 0; --i)
            {
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
            }

            return pdst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Block64* __clear32k(Block32k* pdst, uint count)
        {
            for (uint i = count; i != 0; --i)
                *pdst++ = default;

            return (Block64*)pdst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Block32k* __clear256k(Block256k* pdst, uint count)
        {
            for (uint i = count; i != 0; --i)
                *pdst++ = default;

            return (Block32k*)pdst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Block256k* __clear16m(Block2048k* pdst, nuint_t count)
        {
            for (var i = count; i != 0; --i)
            {
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
            }

            return (Block256k*)pdst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public static void Clear(byte* dest, nuint_t sizeInBytes)
        {
            const uint SZ_16M = 1u << 24;
            const uint MASK_256K = (1u << 18) - 1;
            const uint MASK_32K = (1u << 15) - 1;
            const uint MASK_512 = (1u << 9) - 1;

            var p256k =
                sizeInBytes >= SZ_16M
                ? __clear16m((Block2048k*)dest, sizeInBytes >> 24)
                : (Block256k*)dest;

            var p32k = __clear256k(p256k, ((uint)sizeInBytes & (SZ_16M - 1)) >> 18);
            var p64 = __clear32k(p32k, ((uint)sizeInBytes & MASK_256K) >> 15);

            p64 = __clear512b(p64, ((uint)sizeInBytes & MASK_32K) >> 9);
            __clear511b(p64, (uint)sizeInBytes & MASK_512);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(void* dest, nint_t count, int type_size) =>
            Clear((byte*)dest, (nuint_t)count * (uint)type_size);
        #endregion

        #region memmove
        /// <summary>
        /// Indirect call <see cref="Buffer.MemoryCopy(void*, void*, ulong, ulong)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public static void Memmove(void* src, void* dst, nuint_t sizeInBytes) => 
            Buffer.MemoryCopy(src, dst, (ulong)sizeInBytes, (ulong)sizeInBytes);

        /// <summary>
        /// Indirect call <see cref="Buffer.MemoryCopy(void*, void*, ulong, ulong)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memmove(void* src, void* dst, nint_t count, int type_size) =>
            Memmove(src, dst, (nuint_t)count * (uint)type_size);
        #endregion

        #region memalloc
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Memalloc(nint_t count, int type_size) =>
            (void*)Marshal.AllocHGlobal((IntPtr)(count * (uint)type_size));
        #endregion

        #region structures

        [StructLayout(LayoutKind.Sequential, Size = 0x10)]
        private readonly ref struct Block16 { }

        [StructLayout(LayoutKind.Sequential, Size = 0x40)]
        private readonly ref struct Block64 { }

        [StructLayout(LayoutKind.Sequential, Size = 0x8000)]
        private readonly ref struct Block32k { }

        [StructLayout(LayoutKind.Sequential, Size = 0x4_0000)]
        private readonly ref struct Block256k { }

        [StructLayout(LayoutKind.Sequential, Size = 0x20_0000)]
        private readonly ref struct Block2048k { }
        #endregion
    }
}
