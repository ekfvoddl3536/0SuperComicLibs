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

using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static unsafe class ArgValidateHelper
    {
        /// <summary>
        /// Exception thrown if: <c><paramref name="offset"/> &gt;= <paramref name="length"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfIndexOutOfRange(long offset, long length)
        {
            if ((ulong)offset >= (ulong)length)
                throw new ArgumentOutOfRangeException(nameof(offset));
        }

        /// <summary>
        /// Exception thrown if: <c><paramref name="offset"/> &gt; <paramref name="length"/></c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfLengthOutOfRange(long offset, long length)
        {
            if ((ulong)offset > (ulong)length)
                throw new ArgumentOutOfRangeException(nameof(offset));
        }

        /// <summary>
        /// Exception thrown if: <c><paramref name="value"/> &lt; 0</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative(long value)
        {
            if (value < 0)
                throw new ArgumentException("Negative values are not allowed!", nameof(value));
        }

        /// <summary>
        /// Exception thrown if: <c><paramref name="value"/> == null</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
        }
    }
}
