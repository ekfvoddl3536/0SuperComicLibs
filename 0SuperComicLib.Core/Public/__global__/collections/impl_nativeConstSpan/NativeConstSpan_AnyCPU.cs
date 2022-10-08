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

#if AnyCPU
#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct NativeConstSpan<T>
    {
        internal readonly T* _source;
        private readonly void* _length;

#region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeConstSpan(T* source, size_t length)
        {
            _source = source;
            _length = length.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeConstSpan(T* start, T* end)
        {
            _source = start;
            _length = (void*)(end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, size_t length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public NativeConstSpan(const_iterator<T> start, const_iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
#endregion

#region property
        public ref readonly T this[int index] => ref *(_source + index);

        [CodeContracts.X64Only]
        public ref readonly T this[long index] => ref *(_source + index);

        [CodeContracts.X64LossOfLength]
        public int Length => (int)_length;

        [CodeContracts.X64Only]
        public long LongLength => (long)_length;
#endregion

#region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeConstSpan<T>(_source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            int len = Length;

            if (len <= 0)
                return Array.Empty<T>();

            T[] res = new T[len];

            fixed (T* pdst = &res[0])
                MemoryBlock.Memmove(_source, pdst, _length, sizeof(T));

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((ulong)LongLength <= (ulong)dst.LongLength)
            {
                MemoryBlock.Memmove(_source, dst.Source, _length, sizeof(T));

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)LongLength <= (ulong)dst.LongLength, $"'{nameof(dst)}'");

            MemoryBlock.Memmove(_source, dst.Source, _length, sizeof(T));
        }
#endregion

#region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T at(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)Length, $"index: {index} / length: {Length}");
            return ref *(_source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public ref readonly T at(long index)
        {
            Contract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)LongLength, $"index: {index} / length: {Length}");
            return ref *(_source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public const_iterator<T> begin() => new const_iterator<T>(_source);
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public const_iterator<T> end() =>
            IntPtr.Size == sizeof(int)
            ? new const_iterator<T>(_source + Length)
            : new const_iterator<T>(_source + LongLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public const_reverse_iterator<T> rbegin() =>
            IntPtr.Size == sizeof(int)
            ? new const_reverse_iterator<T>(_source + (Length - 1))
            : new const_reverse_iterator<T>(_source + (LongLength - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public const_reverse_iterator<T> rend() => new const_reverse_iterator<T>(_source - 1);
#endregion

#region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left._source == right._source && left._length == right._length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeConstSpan<T> left, NativeConstSpan<T> right) =>
            left._source != right._source || left._length != right._length;
#endregion
    }
}
#endif