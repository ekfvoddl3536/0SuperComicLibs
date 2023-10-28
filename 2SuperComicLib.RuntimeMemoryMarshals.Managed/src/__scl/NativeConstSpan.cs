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
    public readonly unsafe ref struct NativeConstSpan<T> where T : unmanaged
    {
        internal readonly T* _source;
        public readonly long Length;

        #region constructors
        /// <summary>
        /// Pointers to the initial position and the length of a range.
        /// <para/>
        /// The range used is <c>[<paramref name="first"/>, <paramref name="first"/> + <paramref name="length"/>)</c>, which includes <paramref name="length"/> elements starting from <paramref name="first"/>.<br/>
        /// </summary>
        /// <param name="first">The starting address of the range, including the pointed element.</param>
        /// <param name="length">The number of elements to include from <paramref name="first"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* first, long length)
        {
            _source = first;
            Length = length;
        }

        /// <summary>
        /// Pointers to the initial and final positions in a range.
        /// <para/>
        /// The range used is <c>[<paramref name="first"/>, <paramref name="last"/>)</c>, which includes all the elements between <paramref name="first"/> and <paramref name="last"/>.<br/>
        /// Including the element pointed by <paramref name="first"/> but not the element pointed by <paramref name="last"/>.
        /// </summary>
        /// <param name="first">The starting address of the range, including the pointed element.</param>
        /// <param name="last">The ending address of the range, <b>excluding</b> the pointed element.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(T* first, T* last)
        {
            _source = first;
            Length = last - first;
        }

        /// <summary>
        /// Iterator to the initial position and the length of a range.
        /// <para/>
        /// The range used is <c>[<paramref name="first"/>, <paramref name="first"/> + <paramref name="length"/>)</c>, which includes <paramref name="length"/> elements starting from <paramref name="first"/>.<br/>
        /// </summary>
        /// <param name="first">The starting address of the range, including the pointed element.</param>
        /// <param name="length">The number of elements to include from <paramref name="first"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> first, long length) : this(first._ptr, length)
        {
        }

        /// <summary>
        /// iterators to the initial and final positions in a range.
        /// <para/>
        /// The range used is <c>[<paramref name="first"/>, <paramref name="last"/>)</c>, which includes all the elements between <paramref name="first"/> and <paramref name="last"/>.<br/>
        /// Including the element pointed by <paramref name="first"/> but not the element pointed by <paramref name="last"/>.
        /// </summary>
        /// <param name="first">The starting address of the range, including the pointed element.</param>
        /// <param name="last">The ending address of the range, <b>excluding</b> the pointed element.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeConstSpan(const_iterator<T> first, const_iterator<T> last) : this(first._ptr, last._ptr)
        {
        }
        #endregion

        #region property
        public ref readonly T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(_source + index);
        }
        #endregion

        #region def methods
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public NativeConstSpan<T> Slice(long startIndex)
        {
            if ((ulong)startIndex >= (ulong)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new NativeConstSpan<T>(_source + startIndex, Length - startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public NativeConstSpan<T> Slice(long startIndex, long count)
        {
            if ((startIndex | count) < 0 || Length - startIndex < count)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} or {nameof(count)}");

            return new NativeConstSpan<T>(_source + startIndex, count);
        }

        /// <summary>
        /// This method performs no range checks on the input value and executes the operation immediately.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, AssumeOperationValid]
        public NativeConstSpan<T> Slice_fast([ValidRange] long startIndex, [ValidRange] long count) =>
            new NativeConstSpan<T>(_source + startIndex, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining), X64LossOfLength]
        public T[] ToArray() => NativeSpanHelper.ToArray(_source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(in NativeSpan<T> dst) =>
            NativeSpanHelper.TryCopyTo(_source, Length, dst.Source, dst.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in NativeSpan<T> dst)
        {
            ArgValidateHelper.ThrowIfNotEnoughLength(Length, dst.Length);
            NativeSpanHelper.CopyTo(_source, dst.Source, (ulong)Length);
        }
        #endregion

        #region impl methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T at(long index)
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(_source + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => new const_iterator<T>(_source);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => new const_iterator<T>(_source + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => new const_reverse_iterator<T>(_source + Length - 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => new const_reverse_iterator<T>(_source - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* DangerousGetPointer() => _source;
        #endregion

        #region util methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong capacity() =>
            (ulong)Length * (uint)sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid]
        public ref readonly TDest getAs<TDest>([ValidRange] long index) where TDest : unmanaged =>
            ref *(TDest*)(_source + index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly TDest getAs_at<TDest>(long index) where TDest : unmanaged
        {
            ArgValidateHelper.ThrowIfIndexOutOfRange(index, Length);
            return ref *(TDest*)(_source + index);
        }
        #endregion

        #region override
        [Obsolete(nameof(NotSupportedException))]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete(nameof(NotSupportedException))]
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
            (((long)left._source ^ (long)right._source) | (left.Length ^ right.Length)) == 0L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in NativeConstSpan<T> left, in NativeConstSpan<T> right) =>
            (((long)left._source ^ (long)right._source) | (left.Length ^ right.Length)) != 0L;
        #endregion
    }
}
