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
using System.Runtime.CompilerServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    unsafe partial struct NativeSpan<T>
    {
        public readonly T* Source;
        private readonly void* _length;

#region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining), X64Only]
        public NativeSpan(T* source, size_t length)
        {
            Source = source;
            _length = length.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64Only]
        public NativeSpan(T* start, T* end)
        {
            Source = start;
            _length = (void*)(end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, size_t length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64Only]
        public NativeSpan(_iterator<T> start, _iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
#endregion

#region property
        public ref T this[int index] => ref *(Source + index);

        [X64Only]
        public ref T this[long index] => ref *(Source + index);

        [X64LossOfLength]
        public int Length => (int)_length;

        [X64Only]
        public long LongLength => (long)_length;
#endregion

#region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeSpan<T>(Source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64Only]
        public NativeSpan<T> Slice(long startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64Only]
        public NativeSpan<T> Slice(long startIndex, long length)
        {
            if ((ulong)(startIndex + length) > (ulong)_length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeSpan<T>(Source + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            int len = (int)_length;

            if (len <= 0)
                return Array.Empty<T>();

            T[] res = new T[len];

            fixed (T* pdst = &res[0])
                MemoryBlock.Memmove(Source, pdst, _length, sizeof(T));

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(NativeSpan<T> dst)
        {
            if ((ulong)_length <= (ulong)dst._length)
            {
                MemoryBlock.Memmove(Source, dst.Source, _length, sizeof(T));

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(NativeSpan<T> dst)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((ulong)_length <= (ulong)dst._length, $"'{nameof(dst)}'");

            MemoryBlock.Memmove(Source, dst.Source, _length, sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value) => MemoryBlock.Memset(Source, _length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear(Source, _length, sizeof(T));
#endregion

#region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((uint)index < (uint)_length, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64Only]
        public ref T at(long index)
        {
            FastContract.Requires<ArgumentOutOfRangeException>((ulong)index < (ulong)_length, $"index: {index} / length: {Length}");
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() =>
            sizeof(void*) == sizeof(int)
            ? new _iterator<T>(Source + Length)
            : new _iterator<T>(Source + LongLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() =>
            sizeof(void*) == sizeof(int)
            ? new reverse_iterator<T>(Source + (Length - 1))
            : new reverse_iterator<T>(Source + (LongLength - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Source - 1);
#endregion

#region util methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long capacity() =>
            sizeof(void*) == sizeof(int)
            ? (uint)_length * (uint)sizeof(T)
            : (long)_length * (uint)sizeof(T);
#endregion

#region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source == right.Source && left._length == right._length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(NativeSpan<T> left, NativeSpan<T> right) =>
            left.Source != right.Source || left._length != right._length;
#endregion
    }
}
#endif