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

using System.Runtime.CompilerServices;
using System.Security;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class NativeInstanceTStructExtension
    {
        /// <summary>
        /// Directly references the managed structure data of this <see cref="NativeInstance{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static ref T Value<T>(this in NativeInstance<T> @this) where T : struct =>
            ref Unsafe.AsRef<T>(@this._Ptr + (sizeof(void*) << 1));

        /// <summary>
        /// Gets a reference to the managed structure data of this <see cref="NativeInstance{T}"/>
        /// <br/>
        /// Use in some scenarios where a reference to data must be passed without the <see langword="ref"/> keyword.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static varef_t<T> GetValueReference<T>(this in NativeInstance<T> @this) where T : struct =>
            new varef_t<T>(@this._Ptr + (sizeof(void*) << 1));

        /// <summary>
        /// Gets a read-only reference to the managed structure data of this <see cref="NativeInstance{T}"/>
        /// <br/>
        /// Use in some scenarios where a read-only reference to data must be passed without the <see langword="ref"/> keyword.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), MonoRuntimeNotSupported]
        public static cvaref<T> GetReadOnlyValueReference<T>(this in NativeInstance<T> @this) where T : struct =>
            new cvaref<T>(@this._Ptr + (sizeof(void*) << 1));
    }
}
