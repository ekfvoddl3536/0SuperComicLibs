using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public abstract class EnumerableTypeConverterBase<TIn, TOut> : IEnumerable<TOut>, IEnumerator<TOut>
    {
        protected readonly IEnumerator<TIn> _innerEnumerator;

        protected EnumerableTypeConverterBase(IEnumerable<TIn> collection) =>
            _innerEnumerator = collection.GetEnumerator();
        protected EnumerableTypeConverterBase(IEnumerator<TIn> innerEnumerator) =>
            _innerEnumerator = innerEnumerator;

        IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public abstract TOut Current { get; }
        object IEnumerator.Current => Current;

        bool IEnumerator.MoveNext() => _innerEnumerator.MoveNext();
        void IEnumerator.Reset() => _innerEnumerator.Reset();
        void IDisposable.Dispose() => _innerEnumerator.Dispose();
    }
}
