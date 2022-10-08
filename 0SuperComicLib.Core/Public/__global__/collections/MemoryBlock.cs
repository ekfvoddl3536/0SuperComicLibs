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
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        public static void Memset32<T>(T* dest, uint count, T value) where T : unmanaged
        {
            for (uint i = count >> 4; i-- > 0;)
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

            __memset15(dest, count, value);
        }

#if AnyCPU || X64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset64<T>(T* dest, ulong count, T value) where T : unmanaged
        {
            for (ulong i = count >> 4; i-- > 0;)
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
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset<T>(T* dest, size_t count, in T value) where T : unmanaged
        {
#if AnyCPU
            if (IntPtr.Size == sizeof(int))
                Memset32(dest, (uint)count.value, value);
            else
                Memset64(dest, (ulong)count.value, value);
#elif X86
            Memset32(dest, count.value, value);
#else
            Memset64(dest, count.value, value);
#endif
        }
        #endregion

        #region clear
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void __clear1023b(Block64* ptr, uint sizeInBytes)
        {
            if ((sizeInBytes & 0x200) != 0)
            {
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
            }

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
        private static Block64* __clear64k(Block4096* pdst, uint count)
        {
            for (uint i = count; i-- != 0;)
            {
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;

                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
            }

            return (Block64*)pdst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Block4096* __clear16m(Block2048k* pdst, size_t count)
        {
            for (byte* i = (byte*)count.value; i-- != null;)
            {
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
                *pdst++ = default; *pdst++ = default;
            }

            return (Block4096*)pdst;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear32(byte* dest, uint sizeInBytes)
        {
            var ptr64k = (Block4096*)dest;
            if (sizeInBytes > ushort.MaxValue)
            {
                __clear64k(ptr64k, sizeInBytes >> 16);
                sizeInBytes &= ushort.MaxValue;
            }

            // 64x8
            Block64* ptr = (Block64*)dest;
            for (uint i = sizeInBytes >> 10; i-- > 0;)
            {
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;

                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
            }

            __clear1023b(ptr, sizeInBytes & 0x1FF);
        }

#if AnyCPU || X64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear64(byte* dest, ulong sizeInBytes)
        {
            var ptr64k = __clear16m((Block2048k*)dest, sizeInBytes >> 24);

            // 64x16
            var ptr = __clear64k(ptr64k, ((uint)sizeInBytes & 0xFF_FFFF) >> 16);
            for (uint i = ((uint)sizeInBytes & ushort.MaxValue) >> 10; i-- > 0;)
            {
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;

                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
                *ptr++ = default; *ptr++ = default;
            }

            __clear1023b(ptr, (uint)sizeInBytes & 0x1FF);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(void* dest, size_t sizeInBytes)
        {
#if AnyCPU
            if (IntPtr.Size == sizeof(int))
                Clear32((byte*)dest, (uint)sizeInBytes.value);
            else
                Clear64((byte*)dest, (ulong)sizeInBytes.value);
#elif X86
            Clear32((byte*)dest, sizeInBytes.value);
#else
            Clear64((byte*)dest, sizeInBytes.value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(void* dest, size_t count, int type_size)
        {
#if AnyCPU
            if (IntPtr.Size == sizeof(int))
                Clear32((byte*)dest, (uint)count.value * (uint)type_size);
            else
                Clear64((byte*)dest, (ulong)count.value * (uint)type_size);
#elif X86
            Clear32((byte*)dest, count.value * (uint)type_size);
#else
            Clear64((byte*)dest, count.value * (uint)type_size);
#endif
        }
        #endregion

        #region memmove
        /// <summary>
        /// Indirect call <see cref="Buffer.MemoryCopy(void*, void*, ulong, ulong)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memmove(void* src, void* dst, size_t sizeInBytes)
        {
#if AnyCPU
            if (IntPtr.Size == sizeof(int))
                Buffer.MemoryCopy(src, dst, (ulong)(uint)sizeInBytes.value, (uint)sizeInBytes.value);
            else
                Buffer.MemoryCopy(src, dst, (ulong)sizeInBytes.value, (ulong)sizeInBytes.value);
#else
            Buffer.MemoryCopy(src, dst, sizeInBytes.value, sizeInBytes.value);
#endif
        }

        /// <summary>
        /// Indirect call <see cref="Buffer.MemoryCopy(void*, void*, ulong, ulong)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memmove(void* src, void* dst, size_t count, int type_size)
        {
#if AnyCPU
            ulong sizeInBytes =
                IntPtr.Size == sizeof(int)
                ? (uint)count.value * (uint)type_size
                : (ulong)count.value * (uint)type_size;

            Buffer.MemoryCopy(src, dst, sizeInBytes, sizeInBytes);
#else
            ulong sizeInBytes = count.value * (uint)type_size;
            Buffer.MemoryCopy(src, dst, sizeInBytes, sizeInBytes);
#endif
        }
#endregion

#region memalloc
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Memalloc(size_t count, int type_size) =>
#if AnyCPU
            IntPtr.Size == sizeof(int)
            ? (void*)Marshal.AllocHGlobal((int)count.value * type_size)
            : (void*)Marshal.AllocHGlobal((IntPtr)((ulong)count.value * (uint)type_size));
#elif X86
            (void*)Marshal.AllocHGlobal((int)count.value * type_size);
#else
            (void*)Marshal.AllocHGlobal((IntPtr)(count.value * (uint)type_size));
#endif
#endregion

#region structures
        [StructLayout(LayoutKind.Sequential, Size = 64)]
        private readonly ref struct Block64 { }

        [StructLayout(LayoutKind.Sequential, Size = 16)]
        private readonly ref struct Block16 { }

        [StructLayout(LayoutKind.Sequential, Size = 4096)]
        private readonly ref struct Block4096 { }

        [StructLayout(LayoutKind.Sequential, Size = 0x20_0000)]
        private readonly ref struct Block2048k { }
#endregion
    }
}
