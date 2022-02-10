using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class EnumerableConvert<TIn, TOut> : IEnumerable<TOut>, IEnumerator<TOut>
    {
        private IEnumerator<TIn> collection;
        private Converter<TIn, TOut> converter;
        private TOut current;

        public EnumerableConvert(IEnumerable<TIn> collection, Converter<TIn, TOut> converter)
        {
            this.collection = collection.GetEnumerator();
            this.converter = converter;
        }

        public IEnumerator<TOut> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public TOut Current => current;

        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (collection.MoveNext())
            {
                current = converter.Invoke(collection.Current);
                return true;
            }

            return false;
        }

        public void Reset() => collection.Reset();

        public void Dispose()
        {
            collection.Dispose();
            collection = null;

            converter = null;

            current = default;

            GC.SuppressFinalize(this);
        }
    }
}
