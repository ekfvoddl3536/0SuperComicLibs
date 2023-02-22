// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    public static unsafe class ArrayUnsafeGetElementReferenceExtension
    {
        /// <summary>
        /// Skip array bounds checking, get reference to first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_dotnet<T>([DisallowNull] this T[] array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skip array bounds checking, get reference to first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_dotnet<T>([DisallowNull] this T[] array, [ValidRange] IntPtr index) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skip array bounds checking, get reference to relative first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_dotnet<T>([DisallowNull, ValidRange] this in Memory<T> array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skip array bounds checking, get read-only reference to relative first element.
        /// <br/>
        /// To index a read-only reference address, use <see cref="ILUnsafe.Add{T}(in T, int)"/>
        /// <br/>
        /// API for high-performance scenarios.
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T refdata_dotnet<T>([DisallowNull, ValidRange] this in ConstMemory<T> array) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Skip array bounds checking, get reference to first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>([DisallowNull] this T[] array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skip array bounds checking, get reference to first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>([DisallowNull] this T[] array, [ValidRange] IntPtr index) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skip array bounds checking, get reference to relative first element.
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T refdata_mono<T>([DisallowNull, ValidRange] this in Memory<T> array) => throw new PlatformNotSupportedException();
        /// <summary>
        /// Skip array bounds checking, get read-only reference to relative first element.
        /// <br/>
        /// To index a read-only reference address, use <see cref="ILUnsafe.Add{T}(in T, int)"/>
        /// <br/>
        /// API for high-performance scenarios.
        /// <seealso href="https://blog.naver.com/ekfvoddl3535/222985670274">refer (KO-KR).</seealso>
        /// </summary>
        [AssumeInputsValid, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T refdata_mono<T>([DisallowNull, ValidRange] this in ConstMemory<T> array) => throw new PlatformNotSupportedException();
    }
}
