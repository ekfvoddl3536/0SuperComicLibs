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
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public class ListStack<T> : LookaheadStack<T>, IList<T>, IReadOnlyList<T>
    {
        public ListStack()
        {
        }

        public ListStack(T[] source) : base(source)
        {
        }

        public ListStack(int initalSize) => IncreaseCapacity(initalSize.Max(4));

        public T this[int index]
        {
            get => arr[index];
            set => arr[index] = value;
        }

        public int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;

            int x = size;
            T[] vs = arr;
            while (--x >= 0)
                if (comparer.Equals(vs[x], item))
                    return x;

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index >= 0 && index < size)
            {
                T[] arr = this.arr;
                if (size == arr.Length)
                    IncreaseCapacity(size + 4);
                size++;

                Array.Copy(arr, index, arr, index + 1, size - index);
                arr[index] = item;
            }
        }

        public void RemoveAt(int index)
        {
            T[] arr = this.arr;

            int idx = --size;
            Array.Copy(arr, index + 1, arr, index, idx - index);
            arr[idx] = default;
        }

        public override T[] ToArray()
        {
            int n;
            if ((n = size) <= 0)
                return Array.Empty<T>();

            T[] vs = new T[n];
            T[] arr = this.arr;
            while (--n >= 0)
                vs[n] = arr[n];

            return vs;
        }
    }
}
