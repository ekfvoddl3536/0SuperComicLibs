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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    readonly unsafe ref partial struct NativeConstSpan<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);

        [StructLayout(LayoutKind.Sequential)]
        public ref struct Enumerator
        {
            private T* _Last;
            private readonly T* _End;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(NativeConstSpan<T> span)
            {
                _Last = span._source - 1;
                _End = span._source + span.Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(T* last, T* end)
            {
                _Last = last;
                _End = end;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_Last != _End;

            public ref readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref *_Last;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator Enumerator(NativeSpan<T>.Enumerator v) => new Enumerator(v._Last, v._End);
        }
    }
}