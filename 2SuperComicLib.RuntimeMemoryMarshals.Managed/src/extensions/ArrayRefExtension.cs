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
using System.Security;

namespace SuperComicLib.RuntimeMemoryMarshals
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
        public static arrayrefSegment<T> Slice<T>(this in arrayref<T> @this, int startIndex, int count) =>
            new arrayrefSegment<T>(@this, startIndex, count);

        /// <summary>
        /// Returns a subarray of the specified <see cref="arrayref{T}"/>. This operation does not create a copy.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<T> Slice<T>(this in arrayref<T> @this, int startIndex) =>
            new arrayrefSegment<T>(@this, startIndex);

        /// <summary>
        /// Returns a casted semi-managed array.<para/>
        /// This method modifies the values of the original <see cref="arrayref{T}"/> without making a copy.<br/>
        /// Therefore, do not use the original <see cref="arrayref{T}"/> after calling this method.<para/>
        /// To avoid <see cref="arrayref{T}.Length"/> loss, back up by calling <see cref="Capture{T}(in arrayref{T})"/> before calling this method.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayref<TTo> CastDirect<TFrom, TTo>(this in arrayref<TFrom> @this)
        {
            *(IntPtr*)@this._pClass = *(IntPtr*)ILUnsafe.AsPointer(Array.Empty<TTo>());

            var len_u = (ulong)@this.LongLength * (uint)ILUnsafe.SizeOf<TFrom>() / (uint)ILUnsafe.SizeOf<TTo>();
            *(ulong*)@this._pLength = len_u;

            return new arrayref<TTo>(@this._pClass, @this._pLength);
        }

        /// <summary>
        /// Returns a casted semi-managed array segment.<para/>
        /// This method modifies the values of the original <see cref="arrayref{T}"/> without making a copy.<br/>
        /// Therefore, do not use the original <see cref="arrayrefSegment{T}"/> after calling this method.<para/>
        /// To avoid loss of unique data of <see cref="arrayrefSegment{T}"/>, back up by calling <see cref="Capture{T}(in arrayrefSegment{T})"/> before calling this method.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<TTo> CastDirect<TFrom, TTo>(this in arrayrefSegment<TFrom> @this)
        {
            var resref = @this._source.CastDirect<TFrom, TTo>();

            var idx_i4 = @this._start * ILUnsafe.SizeOf<TFrom>() / ILUnsafe.SizeOf<TTo>();
            var len_i4 = @this.Length * ILUnsafe.SizeOf<TFrom>() / ILUnsafe.SizeOf<TTo>();

            return new arrayrefSegment<TTo>(resref, idx_i4, len_i4);
        }

        /// <summary>
        /// Save the unique data of the current <see cref="arrayref{T}"/> for later restoration.
        /// </summary>
        /// <returns><see cref="CapturedArrayref{T}"/> that can later perform a restore of the unique data of the <see cref="arrayref{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CapturedArrayref<T> Capture<T>(this in arrayref<T> @this) =>
            new CapturedArrayref<T>(@this);

        /// <summary>
        /// Save the unique data of the current <see cref="arrayrefSegment{T}"/> for later restoration.
        /// </summary>
        /// <returns><see cref="CapturedArrayrefSegment{T}"/> that can later perform a restore of the unique data of the <see cref="arrayrefSegment{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CapturedArrayrefSegment<T> Capture<T>(this in arrayrefSegment<T> @this) =>
            new CapturedArrayrefSegment<T>(@this);
    }
}
