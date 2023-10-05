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
        public readonly nint_t Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* source, nint_t length)
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
        public NativeSpan(_iterator<T> start, nint_t length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(_iterator<T> start, _iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref T this[nint_t index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(Source + (long)index);
        }
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan<T> Slice(nint_t startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public NativeSpan<T> Slice(nint_t startIndex, nint_t count)
        {
            ArgValidateHelper.ThrowIfLengthOutOfRange(startIndex + count, Length);
            return new NativeSpan<T>(Source + (long)startIndex, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public T[] ToArray() => NativeSpanHelper.ToArray(Source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(in NativeSpan<T> dst) =>
            NativeSpanHelper.TryCopyTo(Source, Length, dst.Source, dst.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in NativeSpan<T> dst)
        {
            ArgValidateHelper.ThrowIfLengthOutOfRange(Length, dst.Length);
            NativeSpanHelper.CopyTo(Source, dst.Source, (nuint_t)Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(in T value) => MemoryBlock.Fill(Source, (nuint_t)Length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => MemoryBlock.Clear((byte*)Source, (nuint_t)Length * (uint)sizeof(T));
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(nint_t index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(Source + (long)index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> begin() => new _iterator<T>(Source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public _iterator<T> end() => new _iterator<T>(Source + (long)Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rbegin() => new reverse_iterator<T>(Source + (long)Length - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reverse_iterator<T> rend() => new reverse_iterator<T>(Source - 1);
        #endregion

        #region util methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nuint_t capacity() => 
            (nuint_t)Length * (uint)sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public ref TDest getAs<TDest>([ValidRange] nint_t index) where TDest : unmanaged => 
            ref *(TDest*)(Source + (long)index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TDest getAs_at<TDest>(nint_t index) where TDest : unmanaged
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(TDest*)(Source + (long)index);
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
            CMathi.CXOR(left.Source, right.Source, left.Length, right.Length) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeSpan<T> left, in NativeSpan<T> right) =>
            CMathi.CXOR(left.Source, right.Source, left.Length, right.Length) != 0;
        #endregion
    }
}
