using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class ElementCloner<T> : IEnumerable<T>, IEnumerator<T>
    {
        private Func<T, T> callback;
        private IEnumerator<T> item;

        public ElementCloner(IEnumerator<T> item, Func<T, T> callback)
        {
            this.item = item;
            this.callback = callback;
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T Current => callback.Invoke(item.Current);
        object IEnumerator.Current => Current;

        public bool MoveNext() => item.MoveNext();

        public void Reset() => item.Reset();

        public void Dispose()
        {
            if (item != null)
            {
                item.Dispose();
                item = null;

                callback = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
