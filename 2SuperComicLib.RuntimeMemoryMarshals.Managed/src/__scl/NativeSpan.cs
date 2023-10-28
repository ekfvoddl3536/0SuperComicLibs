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
        /// <summary>
        /// Pointers to the initial position and the length of a range.
        /// <para/>
        /// The range used is <c>[<paramref name="first"/>, <paramref name="first"/> + <paramref name="length"/>)</c>, which includes <paramref name="length"/> elements starting from <paramref name="first"/>.<br/>
        /// </summary>
        /// <param name="first">The starting address of the range, including the pointed element.</param>
        /// <param name="length">The number of elements to include from <paramref name="first"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSpan(T* first, long length)
        {
            Source = first;
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
        public NativeSpan(T* first, T* last)
        {
            Source = first;
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
        public NativeSpan(_iterator<T> first, long length) : this(first._ptr, length)
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
        public NativeSpan(_iterator<T> first, _iterator<T> last) : this(first._ptr, last._ptr)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_iterator<T> cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public const_reverse_iterator<T> crend() => rend();
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
        [Obsolete(nameof(NotSupportedException))]
        public override bool Equals(object obj) => throw new NotSupportedException();

        [Obsolete(nameof(NotSupportedException))]
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
