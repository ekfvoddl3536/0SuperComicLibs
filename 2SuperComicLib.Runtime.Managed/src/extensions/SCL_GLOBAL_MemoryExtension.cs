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
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    public static unsafe class SCL_GLOBAL_MemoryExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public static void CopyTo<T>(this in Memory<T> source, in NativeSpan<T> destination) where T : unmanaged
        {
            if (!source.IsValid)
                throw new ArgumentOutOfRangeException(nameof(source));

            if (destination.Source == null)
                throw new ArgumentNullException(nameof(destination));

            if ((uint)source.Length > (uint)destination.Length)
                throw new ArgumentOutOfRangeException(nameof(destination));

            CopyTo_unsafe(source, destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength, AssumeInputsValid]
        public static void CopyTo_unsafe<T>([DisallowNull, ValidRange] this in Memory<T> source, [DisallowNull, ValidRange] in NativeSpan<T> destination) where T : unmanaged
        {
            ulong sz = (uint)source.Length * (uint)sizeof(T);
            fixed (T* psrc = &source[0])
                Buffer.MemoryCopy(psrc, destination.Source, sz, sz);
        }
    }
}
