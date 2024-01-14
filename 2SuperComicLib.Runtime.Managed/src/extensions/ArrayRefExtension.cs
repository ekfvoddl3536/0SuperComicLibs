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

using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.Runtime
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class ArrayRefExtension
    {
        /// <summary>
        /// Convert to <see cref="NativeSpan{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<T> AsSpan<T>(this in arrayref<T> @this) where T : unmanaged =>
            new NativeSpan<T>((T*)@this.GetDataPointer(), @this.Length);

        /// <summary>
        /// Convert to <see cref="NativeSpan{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<T> AsSpan<T>(this in arrayrefSegment<T> @this) where T : unmanaged =>
            new NativeSpan<T>((T*)@this._source.GetDataPointer() + @this._start, @this.Length);

        /// <summary>
        /// Returns a subarray of the specified <see cref="arrayref{T}"/>. This operation does not create a copy.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<T> Slice<T>(this in arrayref<T> @this, int startIndex, int count) where T : unmanaged =>
            new arrayrefSegment<T>(@this, startIndex, count);

        /// <summary>
        /// Returns a subarray of the specified <see cref="arrayref{T}"/>. This operation does not create a copy.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<T> Slice<T>(this in arrayref<T> @this, int startIndex) where T : unmanaged =>
            new arrayrefSegment<T>(@this, startIndex);
    }
}
