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
using System.Runtime.InteropServices;

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Save state of <see cref="arrayrefSegment{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public readonly unsafe struct CapturedArrayrefSegment<T>
    {
        private readonly arrayrefSegment<T> _reference;
        private readonly IntPtr _typehnd;
        private readonly IntPtr _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal CapturedArrayrefSegment(in arrayrefSegment<T> item)
        {
            ref readonly var source = ref item._source;

            _reference = item;
            _typehnd = *(IntPtr*)source._pClass;
            _length = *(IntPtr*)source._pLength;
        }

        /// <summary>
        /// Restore the <see cref="arrayrefSegment{T}"/> to the state it was in when this capture was made.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayrefSegment<T> restore()
        {
            ref readonly var source = ref _reference._source;

            *(IntPtr*)source._pClass = _typehnd;
            *(IntPtr*)source._pLength = _length;

            return _reference;
        }
    }
}
