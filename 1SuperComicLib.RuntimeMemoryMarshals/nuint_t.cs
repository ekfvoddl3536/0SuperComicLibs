// MIT License
//
// Copyright (c) 2023. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    /// <summary>
    /// <see langword="native unsigned int"/> for the .NET Framework
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct nuint_t : IComparable<nuint_t>, IEquatable<nuint_t>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(nuint_t other) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(nuint_t other) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator +(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator -(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator *(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator /(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator %(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator <<(nuint_t a, int b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator >>(nuint_t a, int b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator &(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator |(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator ^(nuint_t a, nuint_t b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator !(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator ~(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator -(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator +(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator ++(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t operator --(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator true(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator false(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nuint_t(IntPtr a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nuint_t(UIntPtr a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nuint_t(bool a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nuint_t(int a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nuint_t(uint a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nuint_t(long a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nuint_t(ulong a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator sbyte(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator byte(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator short(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ushort(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator char(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator uint(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ulong(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nint_t(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nuint_t(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UIntPtr(nuint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe explicit operator void*(nuint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe explicit operator nuint_t(void* a) => throw new PlatformNotSupportedException();
    }
}
