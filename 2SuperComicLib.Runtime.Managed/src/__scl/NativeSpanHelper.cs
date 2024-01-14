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

using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    internal static unsafe class NativeSpanHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(T* source, long length) where T : unmanaged
        {
            var len_i4 = (int)length;

            if (len_i4 <= 0)
                return Array.Empty<T>();

            T[] res = new T[len_i4];

            fixed (T* pdst = &res[0])
                CopyTo(source, pdst, (ulong)len_i4);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCopyTo<T>(T* source, long sourceLength, T* dest, long destLength) where T : unmanaged
        {
            if ((ulong)sourceLength > (ulong)destLength)
                return false;

            CopyTo(source, dest, (ulong)sourceLength);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(T* source, T* dest, ulong length) where T : unmanaged
        {
            ulong copysize = length * (uint)sizeof(T);
            Buffer.MemoryCopy(source, dest, copysize, copysize);
        }
    }
}
