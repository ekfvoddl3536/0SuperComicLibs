using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class MapEntryEnumeratorTypeWrapper<TKey, TValue> :
        IFailableEnumerator<Tuple<TKey, TValue>>,
        IFailableEnumerator<KeyValuePair<TKey, TValue>>
    {
        internal readonly IMapEntryEnumerator<TKey, TValue> _innerEnumerator;

        public MapEntryEnumeratorTypeWrapper(IMapEntryEnumerator<TKey, TValue> innerEnumerator) =>
            _innerEnumerator = innerEnumerator;

        public bool IsFailed => _innerEnumerator.IsFailed;

        object IEnumerator.Current => Current;
        KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
        {
            get
            {
                var current = _innerEnumerator.Current;
                return new KeyValuePair<TKey, TValue>(current.Item1, current.Item2);
            }
        }
        public Tuple<TKey, TValue> Current
        {
            get
            {
                var current = _innerEnumerator.Current;
                return new Tuple<TKey, TValue>(current.Item1, current.Item2);
            }
        }

        public bool MoveNext() => _innerEnumerator.MoveNext();
        public void Reset() => _innerEnumerator.Reset();
        public void Dispose() => _innerEnumerator.Dispose();
    }
}
