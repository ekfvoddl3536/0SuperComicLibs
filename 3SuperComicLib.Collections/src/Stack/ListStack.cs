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
