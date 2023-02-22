using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class MapEntryEnumerable_KVPair<TKey, TValue> : IMapEntryEnumerator<TKey, TValue>
    {
        private readonly IEnumerator<KeyValuePair<TKey, TValue>> src;

        public MapEntryEnumerable_KVPair(IEnumerator<KeyValuePair<TKey, TValue>> src) => this.src = src;

        public ValueTuple<TKey, TValue> Current
        {
            get
            {
                var current = src.Current;
                return new ValueTuple<TKey, TValue>(current.Key, current.Value);
            }
        }
        object IEnumerator.Current => Current;
        public bool IsFailed => false;

        public bool MoveNext() => src.MoveNext();
        public void Reset() => src.Reset();
        public void Dispose() => src.Dispose();
    }
}
