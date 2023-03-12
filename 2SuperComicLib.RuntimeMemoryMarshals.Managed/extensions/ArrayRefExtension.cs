// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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
using SuperComicLib.CodeContracts;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<T> Slice<T>(this in arrayref<T> @this, int startIndex, int count) where T : unmanaged =>
            new arrayrefSegment<T>(@this, startIndex, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<T> Slice<T>(this in arrayref<T> @this, int startIndex) where T : unmanaged =>
            new arrayrefSegment<T>(@this, startIndex);

        /// <summary>
        /// Returns a virtually casted semi-managed array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayrefSegment<TTo> Cast<TFrom, TTo>(this in arrayrefSegment<TFrom> @this)
        {
            ref readonly var source = ref @this._source;

            var convert = new arrayref<TTo>(source._pClass, source._pLength);

            var idx_i4 = @this._start * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>();
            var len_i4 = @this.Length * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>();

            return new arrayrefSegment<TTo>(convert, idx_i4, len_i4);
        }

        /// <summary>
        /// Returns a virtually casted semi-managed array.<para/>
        /// This method does not modify the <see cref="arrayref{T}.Length"/> of the original <see cref="arrayref{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public static arrayrefSegment<TTo> Cast<TFrom, TTo>(this in arrayref<TFrom> @this)
        {
            var convert = new arrayref<TTo>(@this._pClass, @this._pLength);
            var len_u = @this.size() * (uint)Unsafe.SizeOf<TFrom>() / (uint)Unsafe.SizeOf<TTo>();

            return new arrayrefSegment<TTo>(convert, 0, (int)len_u);
        }

        /// <summary>
        /// Returns a casted semi-managed array.<para/>
        /// This method modifies the <see cref="arrayref{T}.Length"/> of the original <see cref="arrayref{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public static arrayref<TTo> CastDirect<TFrom, TTo>(this in arrayref<TFrom> @this)
        {
            var len_u = @this.size() * (uint)Unsafe.SizeOf<TFrom>() / (uint)Unsafe.SizeOf<TTo>();
            *(int*)@this._pLength = (int)len_u;

            return new arrayref<TTo>(@this._pClass, @this._pLength);
        }
    }
}
