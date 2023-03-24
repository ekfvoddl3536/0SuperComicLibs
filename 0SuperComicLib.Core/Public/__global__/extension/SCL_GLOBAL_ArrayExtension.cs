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

#pragma warning disable CS1591
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static unsafe class SCL_GLOBAL_ArrayExtension
    {
        /// <summary>
        /// 원본에 대한 부분 배열을 생성합니다.<br/>
        /// 이 부분 배열은 원본 배열의 복사본이 아닙니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<T> Slice<T>(this T[] source, int startIndex, int count)
        {
            if (startIndex < 0 || (uint)(source.Length - startIndex) < (uint)count)
                throw new ArgumentNullException(nameof(startIndex));

            return new Memory<T>(source, startIndex, count);
        }
    }
}
