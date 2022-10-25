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

#pragma warning disable IDE1006 // 명명 스타일
#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public readonly unsafe ref struct const_iterator<T> 
        where T : unmanaged
    {
        public readonly T* _ptr;

        public const_iterator(T* source) => _ptr = source;

        public ref readonly T this[int index] => ref *(_ptr + index);

        public ref readonly T value => ref *_ptr;

        #region override
        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();
        #endregion

        #region sum & sub
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static const_iterator<T> operator +(const_iterator<T> left, int right) => new const_iterator<T>(left._ptr + right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static const_iterator<T> operator -(const_iterator<T> left, int right) => new const_iterator<T>(left._ptr - right);

#if AnyCPU || X64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static const_iterator<T> operator +(const_iterator<T> left, long right) => new const_iterator<T>(left._ptr + right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static const_iterator<T> operator -(const_iterator<T> left, long right) => new const_iterator<T>(left._ptr - right);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static const_iterator<T> operator ++(const_iterator<T> left) => new const_iterator<T>(left._ptr + 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static const_iterator<T> operator --(const_iterator<T> left) => new const_iterator<T>(left._ptr - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* operator -(const_iterator<T> left, const_iterator<T> right) => (void*)(left._ptr - right._ptr);
        #endregion

        #region compare & equals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(const_iterator<T> left, const_iterator<T> right) => left._ptr == right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(const_iterator<T> left, const_iterator<T> right) => left._ptr != right._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(const_iterator<T> left, const_iterator<T> right) => left._ptr <= right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(const_iterator<T> left, const_iterator<T> right) => left._ptr >= right._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(const_iterator<T> left, const_iterator<T> right) => left._ptr < right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(const_iterator<T> left, const_iterator<T> right) => left._ptr > right._ptr;
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator const_iterator<T>(T* ptr) => new const_iterator<T>(ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator const_iterator<T>(_iterator<T> iter) => new const_iterator<T>(iter._ptr);
        #endregion
    }
}
