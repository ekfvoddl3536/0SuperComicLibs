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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SuperComicLib.Collections
{
    using static Arrays;

    [DebuggerTypeProxy(typeof(EnumerableView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class DoubleQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection
    {
        private T[] _arr;
        private int _size;
        private int _head;
        private int _tail;
        private int _version;

        #region constructors
        public DoubleQueue() => _arr = Array.Empty<T>();

        public DoubleQueue(int capacity) => _arr = new T[(int)CMath.Max((uint)capacity, 4u)];
        #endregion

        #region property
        public int Count => _size;

        public int Capacity => _arr.Length;
        #endregion

        #region methods
        public void Enqueue(in T value)
        {
            if (_size == _arr.Length)
                IncreaseCapacity();

            _arr[_tail] = value;
            QueueMoveNext(ref _tail, _arr.Length);

            ++_size;
            ++_version;
        }

        public T Dequeue()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException("empty collection");

            ++_version;
            --_size;

            ref T val = ref _arr[_head];
            QueueMoveNext(ref _head, _arr.Length);

            return val;
        }

        public void EnqueueBack(in T value)
        {
            if (_size == _arr.Length)
                IncreaseCapacity();

            QueueMovePrev(ref _head, _arr.Length);
            _arr[_head] = value;

            ++_size;
            ++_version;
        }

        public T DequeueBack()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException("empty collection");

            ++_version;
            --_size;

            QueueMovePrev(ref _tail, _arr.Length);
            return _arr[_tail];
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int copy = _size;
            int frontLen = (int)CMath.Min((uint)(_arr.Length - _head), (uint)copy);

            Array.Copy(_arr, _head, array, arrayIndex, frontLen);

            if (copy != frontLen)
                Array.Copy(_arr, 0, array, arrayIndex + frontLen, copy - frontLen);
        }

        private void IncreaseCapacity()
        {
            int newSize = (int)CMath.Max((uint)_arr.Length << 1, 4u);
            T[] newArr = new T[newSize];

            int copy = _size - _head;
            Array.Copy(_arr, _head, newArr, 0, copy);

            Array.Copy(_arr, 0, newArr, copy, _head);

            _arr = newArr;
            _head = 0;
            _tail = _size;
        }

        public T[] ToArray()
        {
            if (_size == 0)
                return Array.Empty<T>();

            T[] res = new T[_size];
            CopyTo(res, 0);

            return res;
        }
        #endregion

        #region impl IEnumerable<T>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region impl ICollection
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        void ICollection.CopyTo(Array array, int index)
        {
            if (!(array is T[] other))
                throw new ArgumentException($"'{nameof(array)}' invalid cast");

            CopyTo(other, index);
        }
        #endregion

        #region nested class
        private sealed class Enumerator : IEnumerator<T>
        {
            private DoubleQueue<T> _owner;
            private readonly int _version;
            private int _pos;

            public Enumerator(DoubleQueue<T> owner)
            {
                _owner = owner;
                _version = owner._version;
                _pos = owner._head - 1;
            }

            public T Current => _owner._arr[_pos];
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_version != _owner._version)
                    throw new InvalidOperationException("fail version");

                QueueMoveNext(ref _pos, _owner._arr.Length);
                return _owner._tail != _pos;
            }

            public void Reset()
            {
                if (_version != _owner._version)
                    throw new InvalidOperationException("fail version");

                _pos = _owner._head - 1;
            }

            public void Dispose() => _owner = null;
        }
        #endregion
    }
}