using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class LimitEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IEnumerator<T> collection;
        private int limit;
        private int x;

        public LimitEnumerable(IEnumerable<T> collection, int limit)
        {
            this.collection = collection.GetEnumerator();
            this.limit = limit;
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public T Current => collection.Current;
        object IEnumerator.Current => collection.Current;

        public bool MoveNext() => ++x < limit && collection.MoveNext();

        public void Reset()
        {
            collection.Reset();
            x = 0;
        }

        public void Dispose()
        {
            if (collection != null)
            {
                collection.Dispose();
                collection = null;

                limit = x = 0;
            }
            GC.SuppressFinalize(this);
        }
    }
}