﻿// MIT License
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

#pragma warning disable IDE1006
#pragma warning disable CS1591

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Runtime
{
    /// <summary>
    /// see: <see cref="Refpoint{T}"/> <br/>
    /// "<c><see langword="readonly"/> <see cref="Refpoint{T}"/></c>" (const refpoint&lt;&gt;)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 1)]
    public readonly unsafe ref struct ReadOnlyRefpoint<T>
    {
        public void* address 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        public ref readonly T value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        public ref readonly T this[long offset]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        public ref readonly ReadOnlyRefpoint<T> inc
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        public ref readonly ReadOnlyRefpoint<T> dec
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new PlatformNotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<U> cast<U>() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add(int offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> sub(int offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add(uint offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> sub(uint offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add(long offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> sub(long offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add(ulong offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> sub(ulong offset) => throw new PlatformNotSupportedException();

        #region extend 'add', 'sub
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add(long offset, int displacement) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add<TSize>(long offset) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> add<TSize>(long offset, int displacement) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ReadOnlyRefpoint<T> sub<TSize>(long offset) => throw new PlatformNotSupportedException();
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long byteOffset(in ReadOnlyRefpoint<T> other) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long elementOffset(in ReadOnlyRefpoint<T> other) => throw new PlatformNotSupportedException();

        public override int GetHashCode() => throw new PlatformNotSupportedException();
        public override bool Equals(object obj) => throw new PlatformNotSupportedException();
        public override string ToString() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference() => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly ReadOnlyRefpoint<T> As(void* source) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly ReadOnlyRefpoint<T> As(in T source) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly ReadOnlyRefpoint<U> As<U>(in T source) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long operator -(in ReadOnlyRefpoint<T> left, in ReadOnlyRefpoint<T> right) => throw new PlatformNotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator +(in ReadOnlyRefpoint<T> v) => throw new PlatformNotSupportedException();
    }
}
