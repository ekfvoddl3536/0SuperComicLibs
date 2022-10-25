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

using System;
using System.Runtime.CompilerServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    public static unsafe class SCL_GLOBAL_ArrayExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> Slice<T>(this T[] source, int startIndex, int count)
        {
            if ((uint)(startIndex + count) > (uint)source.Length)
                throw new ArgumentNullException(nameof(startIndex));

            return new Memory<T>(source, startIndex, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] source, in NativeSpan<T> dest) where T : unmanaged =>
            CopyTo(source, 0, dest, source.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] source, int index, in NativeSpan<T> dest) where T : unmanaged =>
            CopyTo(source, index, dest, source.Length - index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] source, in NativeSpan<T> dest, int count) where T : unmanaged => 
            CopyTo(source, 0, dest, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] source, int index, in NativeSpan<T> dest, int count) where T : unmanaged
        {
            FastContract.Requires(source != null);

            if ((uint)source.Length < (uint)(index + count))
                throw new ArgumentOutOfRangeException(nameof(index));

            if ((uint)dest.Length < (uint)count)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (source.Length > 0)
                fixed (T* ptr = &source[0])
                {
                    ulong sizeInBytes = (uint)count * (uint)sizeof(T);
                    Buffer.MemoryCopy(ptr + index, dest.Source, sizeInBytes, sizeInBytes);
                }
        }
    }
}
