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
using System.Runtime.InteropServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Save state of <see cref="arrayref{T}"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public readonly unsafe struct CapturedArrayref<T>
    {
        private readonly arrayref<T> _reference;
        private readonly IntPtr _typehnd;
        private readonly IntPtr _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CapturedArrayref(in arrayref<T> item)
        {
            _reference = item;
            _typehnd = *(IntPtr*)item._pClass;
            _length = *(IntPtr*)item._pLength;
        }

        /// <summary>
        /// Restore the <see cref="arrayref{T}"/> to the state it was in when this capture was made.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T> restore()
        {
            *(IntPtr*)_reference._pClass = _typehnd;
            *(IntPtr*)_reference._pLength = _length;

            return _reference;
        }
    }
}
