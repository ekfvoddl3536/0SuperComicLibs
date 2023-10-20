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

#pragma warning disable CS0809 // 사용되는 멤버를 재정의하여 사용하지 않음으로 표시
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    [DebuggerTypeProxy(typeof(NativeSpanElementDebugView<>))]
    [DebuggerDisplay("{Length}")]
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref struct NativeSpan<T> where T : unmanaged
    {
        public readonly T* Source;
        public readonly long Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* source, long length)
        {
            Source = source;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* start, T* end)
        {
            Source = start;
            Length = end - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, long length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, _iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(Source + index);
        }
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public NativeSpan<T> Slice(long startIndex)
        {
            if ((ulong)startIndex >= (ulong)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeSpan<T>(Source + startIndex, Length - startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public NativeSpan<T> Slice(long startIndex, long count)
        {
            if ((startIndex | count) < 0 || Length - startIndex < count)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} or {nameof(count)}");

            return new NativeSpan<T>(Source + startIndex, count);
        }

        /// <summary>
        /// This method performs no range checks on the input value and executes the operation immediately.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, AssumeOperationValid]
        public NativeSpan<T> Slice_fast([ValidRange] long startIndex, [ValidRange] long count) =>
            new NativeSpan<T>(Source + startIndex, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public T[] ToArray() => NativeSpanHelper.ToArray(Source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(in NativeSpan<T> dst) =>
            NativeSpanHelper.TryCopyTo(Source, Length, dst.Source, dst.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in NativeSpan<T> dst)
        {
            ArgValidateHelper.ThrowIfNotEnoughLength(Length, dst.Length);
            NativeSpanHelper.CopyTo(Source, dst.Source, (ulong)Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value) => MemoryBlock.Fill(Source, (ulong)Length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear((byte*)Source, (ulong)Length * (uint)sizeof(T));
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(long index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(Source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Source + Length - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Source - 1);
        #endregion

        #region util methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong capacity() =>
            (ulong)Length * (uint)sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public ref TDest getAs<TDest>([ValidRange] long index) where TDest : unmanaged =>
            ref *(TDest*)(Source + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TDest getAs_at<TDest>(long index) where TDest : unmanaged
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(TDest*)(Source + index);
        }
        #endregion

        #region override
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString() =>
            typeof(T) == typeof(char)
            ? new string((char*)Source, 0, (int)Length)
            : $"{nameof(SuperComicLib)}.{nameof(NativeSpan<T>)}<{nameof(T)}>[{Length}]";
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeSpan<T> left, in NativeSpan<T> right) =>
            (((long)left.Source ^ (long)right.Source) | (left.Length ^ right.Length)) == 0L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeSpan<T> left, in NativeSpan<T> right) =>
            (((long)left.Source ^ (long)right.Source) | (left.Length ^ right.Length)) != 0L;
        #endregion
    }
}
