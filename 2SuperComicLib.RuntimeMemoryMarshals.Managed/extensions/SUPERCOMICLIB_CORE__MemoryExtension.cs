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
    public static unsafe class SUPERCOMICLIB_CORE__MemoryExtension
    {
        /// <summary>
        /// Creates a new <see cref="arrayref{T}"/> and, copies the element data of the specified range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static arrayref<T> ToArrayUnmanaged<T>([DisallowNull, ValidRange] this in Memory<T> @this) =>
            JITPlatformEnvironment.IsRunningOnMono
            ? ToArrayUnmanaged_mono(@this)
            : ToArrayUnmanaged_dotnet(@this);

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, MonoRuntimeNotSupported]
        public static arrayref<T> ToArrayUnmanaged_dotnet<T>([DisallowNull, ValidRange] this in Memory<T> @this)
        {
            var res = arrayref<T>.newf_dotnet(@this.Length);

            var dst = res.AsManaged();
            Array.Copy(@this._source, @this._start, dst, 0, @this.Length);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public static arrayref<T> ToArrayUnmanaged_mono<T>([DisallowNull, ValidRange] this in Memory<T> @this)
        {
            var res = arrayref<T>.newf_mono(@this.Length);

            var dst = res.AsManaged();
            Array.Copy(@this._source, @this._start, dst, 0, @this.Length);

            return res;
        }
    }
}
