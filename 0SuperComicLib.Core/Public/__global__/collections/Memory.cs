// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    /// <summary>
    /// 원본 배열에 대한 부분 배열 정보
    /// </summary>
    [DebuggerTypeProxy(typeof(MemoryElementDebugView<>))]
    [DebuggerDisplay("{Length}")]
    public readonly struct Memory<T> : IList<T>, IReadOnlyList<T>, IEquatable<Memory<T>>
    {
        /// <summary>
        /// 원본 배열
        /// </summary>
        public readonly T[] _source;
        /// <summary>
        /// 원본 배열에서 시작하는 위치
        /// </summary>
        public readonly int _start;
        /// <summary>
        /// 부분 배열의 길이
        /// </summary>
        public readonly int Length;

        #region constructors
        /// <summary>
        /// <see cref="ArraySegment{T}"/>의 정보를 사용하여, <see cref="Memory{T}"/>를 초기화
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory(in ArraySegment<T> segment)
        {
            _source = segment.Array;
            _start = segment.Offset;
            Length = segment.Count;
        }

        /// <summary>
        /// 원본 배열, 시작 위치, 길이 정보를 사용해 부분 배열을 구성
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory(T[] source, int startIndex, int length)
        {
            _source = source;
            _start = startIndex;
            Length = length;
        }

        /// <summary>
        /// 원본 배열, 시작 위치 정보를 사용해 부분 배열을 구성
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory(T[] source, int startIndex) : this(source, startIndex, source.Length - startIndex)
        {
        }

        /// <summary>
        /// 원본 배열 정보를 사용해 부분 배열을 구성
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory(T[] source) : this(source, 0, source.Length)
        {
        }
        #endregion

        #region property
        /// <summary>
        /// 부분 배열의 <paramref name="index"/> 번째 원소에 대한 참조 가져오기<para/>
        /// 고성능 시나리오를 위한 API. 범위 검사가 없음.
        /// </summary>
        public ref T this[[ValidRange] int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid, AssumeInputsValid]
            get => ref _source[_start + index];
        }

        /// <summary>
        /// 현재 부분 배열이 유효한지 여부
        /// </summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !IsNull && !IsIndexOutOfRange;
        }

        /// <summary>
        /// 현재 부분 배열에 제공된 원본 배열이 Null인지 여부
        /// </summary>
        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source == null;
        }

        /// <summary>
        /// 현재 부분 배열에 제공된 시작 위치와 길이 정보가 원본 배열 범위를 초과하는지 여부
        /// </summary>
        public bool IsIndexOutOfRange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeOperationValid]
            get => ((_start >> 31) | (_source.Length - _start)) < (uint)Length;
        }
        #endregion

        #region methods (at, slice, copy, +ToArray)
        /// <summary>
        /// 부분 배열의 <paramref name="index"/> 번째 원소에 대한 참조 가져오기
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T at(int index)
        {
            if ((uint)index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _source[_start + index];
        }

        /// <summary>
        /// 현재 부분 배열에 대한 부분 배열을 만듭니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public Memory<T> Slice(int startIndex)
        {
            if (((Length >> 31) | Length) < (uint)startIndex) 
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new Memory<T>(_source, _start + startIndex, Length - startIndex);
        }

        /// <summary>
        /// 현재 부분 배열에 대한 부분 배열을 만듭니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), NoOverhead]
        public Memory<T> Slice(int startIndex, int count)
        {
            if ((((Length | startIndex) >> 31) | (Length - startIndex)) < (uint)count)
                throw new ArgumentOutOfRangeException($"'{nameof(startIndex)}' or '{nameof(count)}'");

            return new Memory<T>(_source, _start + startIndex, count);
        }

        /// <summary>
        /// 현재 부분 배열에 대한 부분 배열을 만듭니다. (범위 검사가 없음)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), AssumeInputsValid, AssumeOperationValid]
        public Memory<T> Slice_fast([ValidRange] int startIndex, [ValidRange] int count) =>
            new Memory<T>(_source, _start + startIndex, count);

        /// <summary>
        /// 현재 부분 배열 범위의 원소를 배열로 만듭니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            if (Length <= 0)
                return Array.Empty<T>();

            T[] res = new T[Length];
            Array.Copy(_source, _start, res, 0, Length);

            return res;
        }

        /// <summary>
        /// 현재 부분 배열 범위의 원소를 대상 부분 배열로 복사합니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Memory<T> dst) => Array.Copy(_source, dst._source, Length);

        /// <summary>
        /// 현재 부분 배열 범위의 원소를 대상 부분 배열로 복사합니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Memory<T> dst, int count)
        {
            if (Length < count)
                throw new ArgumentOutOfRangeException(nameof(count));

            Array.Copy(_source, _start, dst._source, dst._start, count);
        }

        /// <summary>
        /// 현재 부분 배열 범위의 원소를 대상 부분 배열로 복사합니다.<br/>
        /// 참고: <see cref="List{T}.CopyTo(int, T[], int, int)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int startIndex, in Memory<T> array, int count)
        {
            if ((((Length | startIndex) >> 31) | (Length - startIndex)) < (uint)count)
                throw new ArgumentOutOfRangeException($"'{nameof(startIndex)}' or '{nameof(count)}'");

            Array.Copy(_source, _start + startIndex, array._source, array._start, count);
        }

        /// <summary>
        /// 현재 <see cref="Memory{T}"/>의 정보를 사용하여, <see cref="ArraySegment{T}"/>로 변환합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<T> AsArraySegment() => new ArraySegment<T>(_source, _start, Length);
        #endregion

        #region methods (interface member)
        /// <summary>
        /// 부분 배열 범위에서 지정된 원소의 위치를 찾습니다.
        /// </summary>
        /// <returns>부분 배열 범위에 상대적인 인덱스, 값을 찾지 못한 경우 -1 보다 작을 수 있습니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Array.IndexOf(_source, item, _start, Length) - _start;

        /// <summary>
        /// 부분 배열 범위에서 지정된 원소가 존재하는지 여부
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item) => IndexOf(item) >= 0;

        /// <summary>
        /// 부분 배열 범위의 원소를 기본 값으로 설정합니다
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => Array.Clear(_source, _start, Length);
        #endregion

        #region methods (array functional)
        /// <summary>
        /// 지정된 원소를 찾기 위해 이진 검색을 수행
        /// </summary>
        /// <returns>부분 배열 범위에 상대적인 인덱스</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(T value) => Array.BinarySearch(_source, _start, Length, value) - _start;

        /// <summary>
        /// 지정된 원소를 찾기 위해 이진 검색을 수행
        /// </summary>
        /// <returns>부분 배열 범위에 상대적인 인덱스</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(T value, IComparer<T> comparer) => Array.BinarySearch(_source, _start, Length, value, comparer) - _start;
        #endregion

        #region collection methods
        /// <summary>
        /// 부분 배열 범위에서 지정된 원소의 마지막 위치를 찾습니다.
        /// </summary>
        /// <returns>부분 배열 범위에 상대적인 인덱스, 값을 찾지 못한 경우 -1 보다 작을 수 있습니다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(T item) => Array.LastIndexOf(_source, item, _start, Length) - _start;

        /// <summary>
        /// 현재 부분 배열 범위의 원소를 대상 부분 배열로 복사하려고 시도합니다.
        /// </summary>
        /// <returns>실패하면 false, 성공하면 true 입니다</returns>
        public bool TryCopyTo(Memory<T> other)
        {
            if (_source == null || other._source == null)
                return false;

            if (IsIndexOutOfRange || other.IsIndexOutOfRange || Length > other.Length)
                return false;

            Array.Copy(_source, _start, other._source, other._start, Length);
            return true;
        }
        #endregion

        #region special method
        /// <summary>
        /// fixed에 사용하기 위한, 특수 메소드
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference() => ref _source[_start];
        #endregion

        #region interface impl (explicit)
        #region IList<T>
        T IList<T>.this[int index] { get => at(index); set => at(index) = value; }
        void IList<T>.Insert(int index, T item) => throw new NotSupportedException(MemorySR.ERROR_FIXEDSIZECOLLECTION);
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException(MemorySR.ERROR_FIXEDSIZECOLLECTION);
        int ICollection<T>.Count => Length;
        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.Add(T item) => throw new NotSupportedException(MemorySR.ERROR_FIXEDSIZECOLLECTION);
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Array.Copy(_source, _start, array, arrayIndex, Length);
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException(MemorySR.ERROR_FIXEDSIZECOLLECTION);
        #endregion

        #region IReadOnlyList<T>
        T IReadOnlyList<T>.this[int index] => at(index);
        int IReadOnlyCollection<T>.Count => Length;
        #endregion

        #region enumerable
        /// <summary>
        /// <see cref="IEnumerable{T}.GetEnumerator"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => _source.Skip(_start).Take(Length).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region equatable
        bool IEquatable<Memory<T>>.Equals(Memory<T> other) => this == other;
        #endregion
        #endregion

        #region override
        /// <summary>
        /// <see cref="object.Equals(object)"/>
        /// </summary>
        public override bool Equals(object obj) => obj is Memory<T> other && this == other;
        /// <summary>
        /// <see cref="object.GetHashCode"/>
        /// </summary>
        public override int GetHashCode() => _source.GetHashCode() ^ _start ^ Length;
        #endregion

        #region static
        /// !== CAST
        /// <summary>
        /// 온전한 배열로 부터 부분 배열 정보를 만듭니다. 이 부분 배열은 실제로 원본 배열 전체 범위입니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Memory<T>(T[] source) => new Memory<T>(source, 0, source.Length);

        /// !== EQ 
        /// <summary>
        /// 원본 배열의 참조 값과 배열 범위만 검사합니다. (얕은 비교)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Memory<T> left, in Memory<T> right) =>
            left._source == right._source &&
            ((left._start ^ right._start) | (left.Length ^ right.Length)) == 0;
        /// <summary>
        /// 원본 배열의 참조 값과 배열 범위만 검사합니다. (얕은 비교)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Memory<T> left, in Memory<T> right) =>
            left._source != right._source ||
            ((left._start ^ right._start) | (left.Length ^ right.Length)) != 0;
        #endregion
    }
}
