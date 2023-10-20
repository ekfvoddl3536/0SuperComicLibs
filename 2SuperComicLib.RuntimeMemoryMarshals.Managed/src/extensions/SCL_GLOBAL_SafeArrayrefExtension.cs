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

using System.Runtime.CompilerServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    public static unsafe class SCL_GLOBAL_SafeArrayrefExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<T> AsSpan<T>(this SafeArrayref<T> @this) where T : unmanaged =>
            @this._arr.AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* GetDataPointer<T>(this SafeArrayref<T> @this) where T : unmanaged => 
            @this._arr.GetDataPointer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<byte> GetDataReferenceAsSpan<T>(this SafeArrayref<T> @this) where T : unmanaged => 
            @this._arr.GetDataReferenceAsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<TTo> GetDataReferenceAsSpan<TFrom, TTo>(this SafeArrayref<TFrom> @this) 
            where TFrom : unmanaged
            where TTo : unmanaged => 
            @this._arr.GetDataReferenceAsSpan<TTo>();
    }
}
