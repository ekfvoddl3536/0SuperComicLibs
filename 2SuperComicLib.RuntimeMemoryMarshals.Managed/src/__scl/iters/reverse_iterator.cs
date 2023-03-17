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

#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public readonly unsafe ref struct reverse_iterator<T>
        where T : unmanaged
    {
        public readonly T* _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator(T* source) => _ptr = source;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(_ptr - index);
        }

        public ref T value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *_ptr;
        }

        #region special methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> _base() => new _iterator<T>(_ptr + 1);
        #endregion

        #region override
        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();
        #endregion

        #region sum & sub
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static reverse_iterator<T> operator +(reverse_iterator<T> left, int right) => new reverse_iterator<T>(left._ptr - right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static reverse_iterator<T> operator -(reverse_iterator<T> left, int right) => new reverse_iterator<T>(left._ptr + right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static reverse_iterator<T> operator +(reverse_iterator<T> left, nint_t right) => new reverse_iterator<T>(left._ptr + (long)right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static reverse_iterator<T> operator -(reverse_iterator<T> left, nint_t right) => new reverse_iterator<T>(left._ptr - (long)right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static reverse_iterator<T> operator ++(reverse_iterator<T> left) => new reverse_iterator<T>(left._ptr - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static reverse_iterator<T> operator --(reverse_iterator<T> left) => new reverse_iterator<T>(left._ptr + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* operator -(reverse_iterator<T> left, reverse_iterator<T> right) => (void*)(left._ptr - right._ptr);
        #endregion

        #region compare & equals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(reverse_iterator<T> left, reverse_iterator<T> right) => left._ptr == right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(reverse_iterator<T> left, reverse_iterator<T> right) => left._ptr != right._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(reverse_iterator<T> left, reverse_iterator<T> right) => left._ptr <= right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(reverse_iterator<T> left, reverse_iterator<T> right) => left._ptr >= right._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(reverse_iterator<T> left, reverse_iterator<T> right) => left._ptr < right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(reverse_iterator<T> left, reverse_iterator<T> right) => left._ptr > right._ptr;
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator reverse_iterator<T>(T* ptr) => new reverse_iterator<T>(ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator reverse_iterator<T>(_iterator<T> iter) => new reverse_iterator<T>(iter._ptr - 1);
        #endregion
    }
}
