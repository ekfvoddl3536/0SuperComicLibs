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

namespace SuperComicLib
{
    /// <summary>
    /// 32비트 정수 2개로 범위를 지정합니다.<para/>
    /// 시작 위치와 끝 위치(포함하지 않음)를 갖습니다.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Range : IEquatable<Range>, IComparable<Range>
    {
        /// <summary>
        /// 인덱스의 시작 위치입니다.
        /// </summary>
        public int start;
        /// <summary>
        /// 인덱스의 끝 위치입니다.
        /// </summary>
        public int end;

        /// <summary>
        /// 시작-끝 위치로 <see cref="Range"/> 구조체를 초기화합니다.
        /// </summary>
        /// <param name="start">시작 위치</param>
        /// <param name="end">끝 위치 (포함하지 않음)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// 길이입니다.
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => end - start;
        }

#pragma warning disable CS1591
        /// <summary>
        /// <see cref="operator ==(Range, Range)"/>를 대신 사용하세요
        /// </summary>
        public bool Equals(Range other) => this == other;
        public int CompareTo(Range other)
        {
            if (end < other.start)
                return -1;
            else if (start > other.end)
                return 1;
            else
                return start.CompareTo(other.start);
        }

        public override bool Equals(object obj) => obj is Range other && Equals(other);
        public override int GetHashCode() => start ^ end;
        public override string ToString() => $"[{start}, {end}) Length: '{Length}'";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Range left, Range right) => *(long*)&left == *(long*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Range left, Range right) => *(long*)&left != *(long*)&right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Range left, Range right) => left.end < right.start;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Range left, Range right) => left.start > right.end;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Range left, Range right) => left.end <= right.start;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Range left, Range right) => left.start >= right.end;
#pragma warning restore CS1591
    }
}
