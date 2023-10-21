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

namespace SuperComicLib.RuntimeMemoryMarshals
{
    unsafe partial struct arrayref<T>
    {
        public readonly ref struct iterator
        {
            private readonly void* ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal iterator(ref T addr) => ptr = ILUnsafe.AsPointer(ref addr);

            public ref T value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref ILUnsafe.AsRef<T>(ptr);
            }

            public ref T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref ILUnsafe.Add<T>(ptr, index);
            }

            public ref T this[long index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref ILUnsafe.Add<T>(ptr, index);
            }

            /// <summary>
            /// Always throws an exception.
            /// </summary>
            public override bool Equals(object obj) => throw new NotSupportedException();
            /// <summary>
            /// Always throws an exception.
            /// </summary>
            public override int GetHashCode() => throw new NotSupportedException();
            /// <summary>
            /// Always throws an exception.
            /// </summary>
            public override string ToString() => throw new NotSupportedException();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(iterator left, iterator right) => left.ptr == right.ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(iterator left, iterator right) => left.ptr != right.ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator +(iterator left, long right) => new iterator(ref ILUnsafe.Add<T>(left.ptr, right));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator -(iterator left, long right) => new iterator(ref ILUnsafe.Subtract<T>(left.ptr, right));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator ++(iterator left) => new iterator(ref ILUnsafe.Increment(ref left.value));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static iterator operator --(iterator left) => new iterator(ref ILUnsafe.Decrement(ref left.value));
        }
    }
}
