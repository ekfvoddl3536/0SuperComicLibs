using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class Selector<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IEqualityComparer<T> comparer;
        private IEnumerator<T> item;
        private T value;

        public Selector(T value, IEnumerator<T> item, IEqualityComparer<T> comparer)
        {
            this.value = value;

            this.item = item;
            this.comparer = comparer;
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T Current => item.Current;
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (item.MoveNext())
                if (comparer.Equals(item.Current, value))
                    return true;

            return false;
        }

        public void Reset() => item.Reset();

        public void Dispose()
        {
            if (item != null)
            {
                item.Dispose();
                item = null;

                comparer = null;

                value = default;
            }
            GC.SuppressFinalize(this);
        }
    }
}
