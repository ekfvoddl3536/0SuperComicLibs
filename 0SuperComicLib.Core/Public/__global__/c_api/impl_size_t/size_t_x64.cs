// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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

#if X64
#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct size_t
    {
        public readonly ulong value;

#region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(void* val) => value = (ulong)val;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(long val) => value = (ulong)val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(ulong val) => value = val;
#endregion

#region interface impl
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(size_t other) => value.CompareTo(other.value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(size_t other) => value == other.value;
#endregion

#region override
        public override bool Equals(object obj) => obj is size_t other && value == other.value;
        public override int GetHashCode() => (int)value;
#endregion

#region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(void* val) => new size_t(val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(long val) => new size_t(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(ulong val) => new size_t(val);

        // !== Compatibility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(int val) => new size_t((ulong)(uint)val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(uint val) => new size_t((ulong)val);
#endregion

#region offset math (quick)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator ++(size_t val) => new size_t(val.value + 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator --(size_t val) => new size_t(val.value - 1);
#endregion

#region size_t math operation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator +(size_t left, size_t right) => new size_t(left.value + right.value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator -(size_t left, size_t right) => new size_t(left.value - right.value);
#endregion

#region eq & compare
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(size_t left, size_t right) => left.value == right.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(size_t left, size_t right) => left.value != right.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(size_t left, size_t right) => left.value < right.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(size_t left, size_t right) => left.value > right.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(size_t left, size_t right) => left.value >= right.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(size_t left, size_t right) => left.value <= right.value;
#endregion
    }
}
#endif