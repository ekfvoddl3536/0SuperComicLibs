using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public readonly struct Memory<T> : IList<T>, IReadOnlyList<T>, IEquatable<Memory<T>>
    {
        internal readonly T[] _source;
        internal readonly int _start;
        public readonly int Length;

        #region constructor
        public Memory(T[] source, int startIndex, int length)
        {
            _source = source;
            _start = startIndex;
            Length = length;
        }
        #endregion

        #region property
        public ref T this[int index] => ref _source[index + _start];
        #endregion

        #region methods (at, slice, copy, +ToArray)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T At(int index)
        {
            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return ref _source[index + _start];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int startIndex, int length)
        {
            if ((uint)(startIndex + length) > (uint)Length)
                throw new ArgumentOutOfRangeException($"'{nameof(startIndex)}' and '{nameof(length)}'");

            return new Memory<T>(_source, startIndex + _start, length);
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
        public void CopyTo(Memory<T> dst) => Array.Copy(_source, dst._source, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int startIndex, Memory<T> array, int arrayIndex, int length)
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
        public T[] GetSourceArray() => _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Range GetRange() => new Range(_start, _start + Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference() => ref _source[_start];
        #endregion

        #region interface impl (explicit)
        #region IList<T>
        T IList<T>.this[int index] { get => At(index); set => At(index) = value; }
        void IList<T>.Insert(int index, T item) => throw new NotSupportedException("FixedSizeCollection");
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException("FixedSizeCollection");
        int ICollection<T>.Count => Length;
        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.Add(T item) => throw new NotSupportedException("FixedSizeCollection");
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Array.Copy(_source, _start, array, arrayIndex, Length);
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException("FixedSizeCollection");
        #endregion

        #region IReadOnlyList<T>
        T IReadOnlyList<T>.this[int index] => At(index);
        int IReadOnlyCollection<T>.Count => Length;
        #endregion

        #region enumerable + valueiterable
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new Enumerator(_source, _start, Length);
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

        #region nested class
        private sealed class Enumerator : IEnumerator<T>
        {
            private readonly T[] _source;
            private readonly int _start;
            private readonly int _end;
            private int _index;

            public Enumerator(T[] source, int start, int length)
            {
                _source = source;
                _start = start;
                _end = start + length;

                _index = start - 1;
            }

            T IEnumerator<T>.Current => _source[_index];
            object IEnumerator.Current => _source[_index];

            bool IEnumerator.MoveNext() => ++_index < _end;

            void IEnumerator.Reset() => _index = _start - 1;

            void IDisposable.Dispose() { }
        }
        #endregion
    }
}
