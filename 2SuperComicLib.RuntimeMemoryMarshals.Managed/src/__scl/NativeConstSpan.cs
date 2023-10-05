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
    public readonly unsafe ref struct NativeConstSpan<T> where T : unmanaged
    {
        internal readonly T* _source;
        public readonly nint_t Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* source, nint_t length)
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
        public NativeConstSpan(const_iterator<T> start, nint_t length) : this(start._ptr, length)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> start, const_iterator<T> end) : this(start._ptr, end._ptr)
        {
        }
        #endregion

        #region property
        public ref readonly T this[nint_t index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(_source + (long)index);
        }
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan<T> Slice(nint_t startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public NativeConstSpan<T> Slice(nint_t startIndex, nint_t count)
        {
            ArgValidateHelper.ThrowIfLengthOutOfRange(startIndex + count, Length);
            return new NativeConstSpan<T>(_source + (long)startIndex, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public T[] ToArray() => NativeSpanHelper.ToArray(_source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(in NativeSpan<T> dst) =>
            NativeSpanHelper.TryCopyTo(_source, Length, dst.Source, dst.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in NativeSpan<T> dst)
        {
            ArgValidateHelper.ThrowIfLengthOutOfRange(Length, dst.Length);
            NativeSpanHelper.CopyTo(_source, dst.Source, (nuint_t)Length);
        }
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T at(nint_t index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(_source + (long)index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> begin() => new const_iterator<T>(_source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> end() => new const_iterator<T>(_source + (long)Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rbegin() => new const_reverse_iterator<T>(_source + (long)Length - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> rend() => new const_reverse_iterator<T>(_source - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* DangerousGetPointer() => _source;
        #endregion

        #region util methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nuint_t capacity() =>
            (nuint_t)Length * (uint)sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public ref readonly TDest getAs<TDest>([ValidRange] nint_t index) where TDest : unmanaged =>
            ref *(TDest*)(_source + (long)index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly TDest getAs_at<TDest>(nint_t index) where TDest : unmanaged
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(TDest*)(_source + (long)index);
        }
        #endregion

        #region override
        [Obsolete("NotSupport")]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete("NotSupport")]
        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString() =>
            typeof(T) == typeof(char)
            ? new string((char*)_source, 0, (int)Length)
            : $"{nameof(SuperComicLib)}.{nameof(NativeConstSpan<T>)}<{nameof(T)}>[{Length}]";
        #endregion

        #region static members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator NativeConstSpan<T>(in NativeSpan<T> value) => new NativeConstSpan<T>(value.Source, value.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in NativeConstSpan<T> left, in NativeConstSpan<T> right) =>
            CMathi.CXOR(left._source, right._source, left.Length, right.Length) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeConstSpan<T> left, in NativeConstSpan<T> right) =>
            CMathi.CXOR(left._source, right._source, left.Length, right.Length) != 0;
        #endregion
    }
}
