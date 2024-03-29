﻿// The MIT License (MIT)
//
// Copyright (c) 2023-2024. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable
using System;
using System.Runtime.CompilerServices;
using System.Security;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Runtime
{
    [SuppressUnmanagedCodeSecurity, SecurityCritical]
    public static unsafe class ArrayUnsafeGetElementReferenceExtension
    {
        /// <summary>
        /// Skips array bounds checking and gets a reference to the first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_clr<T>([DisallowNull] this T[] array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skips array bounds checking and gets a reference to the element at the specified index position.
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_clr<T>([DisallowNull] this T[] array, [ValidRange] long index) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Skips array bounds checking and gets a reference to the first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>([DisallowNull] this T[] array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skips array bounds checking and gets a reference to the element at the specified index position.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>([DisallowNull] this T[] array, [ValidRange] long index) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Skips array bounds checking and gets an <see langword="addressable"/> reference to the first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Refpoint<T> refpoint_clr<T>([DisallowNull] this T[] array) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Skips array bounds checking and gets an <see langword="addressable"/> reference to the element at the specified index position..
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Refpoint<T> refpoint_clr<T>([DisallowNull] this T[] array, [ValidRange] long index) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Skips array bounds checking and gets an <see langword="addressable"/> reference to the first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Refpoint<T> refpoint_mono<T>([DisallowNull] this T[] array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skips array bounds checking and gets an <see langword="addressable"/> reference to the element at the specified index position.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Refpoint<T> refpoint_mono<T>([DisallowNull] this T[] array, [ValidRange] long index) => throw new PlatformNotSupportedException();
    }
}
