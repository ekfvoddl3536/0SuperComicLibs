﻿// MIT License
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
using System.Runtime.InteropServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// <see langword="const"/> <see cref="varef_t{T}"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), MonoRuntimeNotSupported]
    public readonly unsafe struct cvaref<T> where T : struct
    {
        internal readonly byte* DataReference;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal cvaref(byte* DataReference) => this.DataReference = DataReference;

        /// <summary>
        /// Get a reference to the value. (read only)
        /// </summary>
        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<T>(DataReference);
        }

        /// <summary>
        /// Convert to read only
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator cvaref<T>(varef_t<T> input) => new cvaref<T>(input.DataReference);
    }
}
