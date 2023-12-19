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
using SuperComicLib.CodeContracts;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    public static class Arrayrefs
    {
        internal const string ERROR_FIXEDSIZECOLLECTION = "FixedSizeCollection";

        public const int MaxLength = 0x7FFF_FFC7;


        #region universal
        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> semi-managed array.
        /// <para/>
        /// Available for use in the CoreCLR (.NET Framework, .NET) implementation as well as the Mono implementation.
        /// </summary>
        /// <param name="length">
        /// Must be greater than or equal to 0 and less than or equal to `<c><see cref="MaxLength"/></c>`.
        /// <para/>
        /// If you wish to create an array larger than the `<c><see cref="MaxLength"/></c>` limit, use `<c><see langword="new"/> <see cref="arrayref{T}.arrayref(int)"/></c>` instead.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayref<T> Alloc<T>(int length)
            where T : unmanaged
        {
            if ((uint)length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(length));

            return arrayref<T>.newf(length);
        }

        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> semi-managed array.
        /// <para/>
        /// Available for use in the CoreCLR (.NET Framework, .NET) implementation as well as the Mono implementation.
        /// <para/>
        /// This method does not perform range checks on the provided <paramref name="length"/> parameter.
        /// </summary>
        /// <param name="length">
        /// Must be greater than or equal to 0.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static arrayref<T> Alloc_fast<T>([ValidRange] int length) where T : unmanaged => arrayref<T>.newf(length);
        #endregion

        #region core-clr
        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> semi-managed array.
        /// <para/>
        /// Available for use only in the CoreCLR (.NET Framework, .NET) implementation.
        /// </summary>
        /// <param name="length">
        /// Must be greater than or equal to 0 and less than or equal to `<c><see cref="MaxLength"/></c>`.
        /// <para/>
        /// If you wish to create an array larger than the `<c><see cref="MaxLength"/></c>` limit, use `<c><see langword="new"/> <see cref="arrayref{T}.arrayref(int)"/></c>` instead.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static arrayref<T> CLRAlloc<T>(int length)
            where T : unmanaged
        {
            if ((uint)length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(length));

            return arrayref<T>.newf_clr(length);
        }

        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> semi-managed array.
        /// <para/>
        /// Available for use only in the CoreCLR (.NET Framework, .NET) implementation.
        /// <para/>
        /// This method does not perform range checks on the provided <paramref name="length"/> parameter.
        /// </summary>
        /// <param name="length">
        /// Must be greater than or equal to 0.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported, AssumeInputsValid]
        public static arrayref<T> CLRAlloc_fast<T>([ValidRange] int length) where T : unmanaged => arrayref<T>.newf_clr(length);
        #endregion

        #region mono
        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> semi-managed array.
        /// <para/>
        /// Available for use only in the Mono implementation.
        /// </summary>
        /// <param name="length">
        /// Must be greater than or equal to 0 and less than or equal to `<c><see cref="MaxLength"/></c>`.
        /// <para/>
        /// If you wish to create an array larger than the `<c><see cref="MaxLength"/></c>` limit, use `<c><see langword="new"/> <see cref="arrayref{T}.arrayref(int)"/></c>` instead.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static arrayref<T> MonoAlloc<T>(int length)
            where T : unmanaged
        {
            if ((uint)length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(length));

            return arrayref<T>.newf_mono(length);
        }

        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> semi-managed array.
        /// <para/>
        /// Available for use only in the Mono implementation.
        /// <para/>
        /// This method does not perform range checks on the provided <paramref name="length"/> parameter.
        /// </summary>
        /// <param name="length">
        /// Must be greater than or equal to 0.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static arrayref<T> MonoAlloc_fast<T>([ValidRange] int length) where T : unmanaged => arrayref<T>.newf_mono(length);
        #endregion
    }
}
