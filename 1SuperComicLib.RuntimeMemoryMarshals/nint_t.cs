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
    /// <see langword="native int"/> for the .NET Framework<para/>
    /// If need <c>T* + <see cref="nint_t"/></c> operation, use <c>T* + (<see langword="long"/>)<see cref="nint_t"/></c>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct nint_t : IComparable<nint_t>, IEquatable<nint_t>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(nint_t other) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(nint_t other) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(nint_t a, nint_t b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator +(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator -(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator *(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator /(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator %(nint_t a, nint_t b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator <<(nint_t a, int b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator >>(nint_t a, int b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator &(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator |(nint_t a, nint_t b) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator ^(nint_t a, nint_t b) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator !(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator ~(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator -(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator +(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator ++(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t operator --(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator true(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator false(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint_t(IntPtr a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nint_t(UIntPtr a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint_t(bool a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint_t(int a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nint_t(uint a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint_t(long a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator nint_t(ulong a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator sbyte(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator byte(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator short(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ushort(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator char(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator uint(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ulong(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IntPtr(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UIntPtr(nint_t a) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe explicit operator void*(nint_t a) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe explicit operator nint_t(void* a) => throw new PlatformNotSupportedException();
    }
}
