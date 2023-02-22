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

#pragma warning disable IDE1006
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public readonly struct Memory<T> : IList<T>, IReadOnlyList<T>, IEquatable<Memory<T>>
    {
        public readonly T[] _source;
        public readonly int _start;
        public readonly int Length;

        #region constructors
        public Memory(T[] source, int startIndex, int length)
        {
            _source = source;
            _start = startIndex;
            Length = length;
        }

        public Memory(T[] source, int startIndex) : this(source, startIndex, source.Length - startIndex)
        {
        }

        public Memory(T[] source) : this(source, 0, source.Length)
        {
        }
        #endregion

        #region property
        public ref T this[int index] => ref _source[_start + index];
        #endregion

        #region methods (at, slice, copy, +ToArray)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _source[_start + index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int startIndex) => Slice(startIndex, Length - startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException($"'{nameof(startIndex)}' and '{nameof(length)}'");

            return new Memory<T>(_source, _start + startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            if (Length == 0)
                return Array.Empty<T>();

            T[] res = new T[Length];
            Array.Copy(_source, _start, res, 0, Length);

            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Memory<T> dst) => Array.Copy(_source, dst._source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Memory<T> dst, int length) => Array.Copy(_source, _start, dst._source, dst._start, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int startIndex, in Memory<T> array, int arrayIndex, int length)
        {
            if ((uint)startIndex >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if ((uint)arrayIndex >= (uint)array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            Array.Copy(_source, _start + startIndex, array._source, array._start + arrayIndex, length);
        }
        #endregion

        #region methods (interface member)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Array.IndexOf(_source, item, _start, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => IndexOf(item) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => Array.Clear(_source, _start, Length);
        #endregion

        #region methods (array functional
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(T value) => Array.BinarySearch(_source, _start, Length, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(T value, IComparer<T> comparer) => Array.BinarySearch(_source, _start, Length, value, comparer);
        #endregion

        #region special method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference() => ref _source[_start];
        #endregion

        #region interface impl (explicit)
        #region IList<T>
        T IList<T>.this[int index] { get => at(index); set => at(index) = value; }
        void IList<T>.Insert(int index, T item) => throw new NotSupportedException("FixedSizeCollection");
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException("FixedSizeCollection");
        int ICollection<T>.Count => Length;
        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.Add(T item) => throw new NotSupportedException("FixedSizeCollection");
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Array.Copy(_source, _start, array, arrayIndex, Length);
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException("FixedSizeCollection");
        #endregion

        #region IReadOnlyList<T>
        T IReadOnlyList<T>.this[int index] => at(index);
        int IReadOnlyCollection<T>.Count => Length;
        #endregion

        #region enumerable
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => _source.Skip(_start).Take(Length).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region equatable
        bool IEquatable<Memory<T>>.Equals(Memory<T> other) => this == other;
        #endregion
        #endregion

        #region override
        public override bool Equals(object obj) => obj is Memory<T> other && this == other;
        public override int GetHashCode() => _source.GetHashCode() ^ _start ^ Length;
        #endregion

        #region static
        // !== CAST
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Memory<T>(T[] source) => new Memory<T>(source, 0, source.Length);

        // !== EQ
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Memory<T> left, in Memory<T> right) =>
            left._source == right._source &&
            ((left._start - right._start) | (left.Length - right.Length)) == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Memory<T> left, in Memory<T> right) =>
            left._source != right._source ||
            ((left._start - right._start) | (left.Length - right.Length)) != 0;
        #endregion
    }
}
