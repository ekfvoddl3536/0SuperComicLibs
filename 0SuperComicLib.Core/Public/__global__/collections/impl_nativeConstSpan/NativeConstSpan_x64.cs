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
using System;
using System.Runtime.CompilerServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    unsafe partial struct NativeConstSpan<T>
    {
        internal readonly T* _source;
        public readonly long Length;

#region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* source, long length)
        {
            _source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* start, T* end)
        {
            _source = start;
            Length = end - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, long length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, const_iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
#endregion

#region property
        public ref readonly T this[long index] => ref *(_source + index);
#endregion

#region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(long startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(long startIndex, long length)
        {
            if ((ulong)(startIndex + length) > (ulong)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeConstSpan<T>(_source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            long len = Length;

            if (len <= 0)
                return Array.Empty<T>();

            T[] res = new T[len];

            fixed (T* pdst = &res[0])
            {
                ulong sizeInBytes = (ulong)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(_source, pdst, sizeInBytes, sizeInBytes);
            }

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((ulong)Length <= (ulong)dst.Length)
            {
                ulong sizeInBytes = (ulong)Length * (uint)sizeof(T);
                Buffer.MemoryCopy(_source, dst.Source, sizeInBytes, sizeInBytes);

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((ulong)Length <= (ulong)dst.Length, $"'{nameof(dst)}'");

            ulong sizeInBytes = (ulong)Length * (uint)sizeof(T);
            Buffer.MemoryCopy(_source, dst.Source, sizeInBytes, sizeInBytes);
        }
#endregion

#region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T at(long index)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)Length, $"index: {index} / length: {Length}");
            return ref *(_source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> begin() => new const_iterator<T>(_source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> end() => new const_iterator<T>(_source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rbegin() => new const_reverse_iterator<T>(_source + (Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rend() => new const_reverse_iterator<T>(_source - 1);
#endregion

#region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left._source == right._source && left.Length == right.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left._source != right._source || left.Length != right.Length;
#endregion
    }
}
#endif