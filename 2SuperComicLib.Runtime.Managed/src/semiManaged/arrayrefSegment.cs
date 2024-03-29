﻿// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Runtime
{
    /// <summary>
    /// Represents a subarray of the original <see cref="arrayref{T}"/>
    /// </summary>
    [DebuggerTypeProxy(typeof(SemiManagedArrayElementDebugView<>))]
    [DebuggerDisplay("{Length}")]
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public readonly unsafe struct arrayrefSegment<T> : IEquatable<arrayrefSegment<T>>, IList<T>, IReadOnlyList<T>
        where T : unmanaged
    {
        public readonly arrayref<T> _source;
        public readonly int _start;
        public readonly int Length;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayrefSegment(in arrayref<T> source, int startIndex, int count)
        {
            _source = source;
            _start = startIndex;
            Length = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayrefSegment(in arrayref<T> source, int startIndex) : this(source, startIndex, source.Length - startIndex)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayrefSegment(in arrayref<T> source) : this(source, 0, source.Length)
        {
        }
        #endregion

        #region property
        /// <summary>
        /// Whether the current subarray is valid.
        /// </summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !IsNull && !IsIndexOutOfRange;
        }

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source.IsNull;
        }

        public bool IsIndexOutOfRange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => ((_start >> 31) | (_source.Length - _start)) < (uint)Length;
        }
        #endregion

        #region indexer + at
        public ref T this[[ValidRange] int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
            get => ref _source[_start + index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _source[_start + index];
        }
        #endregion

        #region methods (slice, copy, +toUmgArray)
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public arrayrefSegment<T> Slice(int startIndex)
        {
            if (((Length >> 31) | Length) < (uint)startIndex)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new arrayrefSegment<T>(_source, _start + startIndex, Length - startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public arrayrefSegment<T> Slice(int startIndex, int count)
        {
            if ((((Length | startIndex) >> 31) | (Length - startIndex)) < (uint)count)
                throw new ArgumentOutOfRangeException($"'{nameof(startIndex)}' and '{nameof(count)}'");

            return new arrayrefSegment<T>(_source, _start + startIndex, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T> ToUnmanagedArray()
        {
            var res = new arrayref<T>(Length);
            Array.Copy(_source.AsManaged(), _start, res.AsManaged(), 0, Length);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> AsMemory() => new Memory<T>(_source.AsManaged(), _start, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* GetDataPointer() => _source.GetDataPointer() + _start * ILUnsafe.SizeOf<T>();
        #endregion

        #region interface impl (explicit)
        #region IList<T>
        T IList<T>.this[int index]
        {
            get => at(index);
            set => at(index) = value;
        }
        int IList<T>.IndexOf(T item) => AsMemory().IndexOf(item);
        void IList<T>.Insert(int index, T item) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        int ICollection<T>.Count => Length;
        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.Add(T item) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        void ICollection<T>.Clear() => AsMemory().Clear();
        bool ICollection<T>.Contains(T item) => AsMemory().Contains(item);
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => AsMemory().CopyTo(array, arrayIndex);
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException(Arrayrefs.ERROR_FIXEDSIZECOLLECTION);
        #endregion

        #region IReadOnlyList<T>
        T IReadOnlyList<T>.this[int index] => at(index);
        int IReadOnlyCollection<T>.Count => Length;
        #endregion

        #region enumerable
        public IEnumerator<T> GetEnumerator() => AsMemory().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
        #endregion

        #region interface impl
        bool IEquatable<arrayrefSegment<T>>.Equals(arrayrefSegment<T> other) => this == other;

        public override bool Equals(object obj) => obj is arrayrefSegment<T> other && this == other;
        public override int GetHashCode() => _source.GetHashCode() ^ _start ^ Length;
        #endregion

        #region iterator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.iterator begin() => _source.begin() + _start;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.iterator end() => _source.begin() + (_start + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.reverse_iterator rbegin() => new arrayref<T>.reverse_iterator(_source.begin() + (_start + Length - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.reverse_iterator rend() => new arrayref<T>.reverse_iterator(_source.begin() + (_start - 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_iterator cbegin() => begin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_iterator cend() => end();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_reverse_iterator crbegin() => rbegin();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public arrayref<T>.const_reverse_iterator crend() => rend();
        #endregion

        #region operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in arrayrefSegment<T> left, in arrayrefSegment<T> right) => left._source == right._source && (ILUnsafe.ReadOnlyAs<int, long>(left._start) ^ ILUnsafe.ReadOnlyAs<int, long>(right._start)) == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in arrayrefSegment<T> left, in arrayrefSegment<T> right) => !(left == right);
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator arrayrefSegment<T>(in arrayref<T> v) => new arrayrefSegment<T>(v);
        #endregion
    }
}
