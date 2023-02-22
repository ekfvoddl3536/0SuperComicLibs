using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class MapEntryEnumerable_ValueTuple<TKey, TValue> : IMapEntryEnumerator<TKey, TValue>
    {
        private readonly IEnumerator<ValueTuple<TKey, TValue>> src;

        public MapEntryEnumerable_ValueTuple(IEnumerator<ValueTuple<TKey, TValue>> src) => this.src = src;

        public ValueTuple<TKey, TValue> Current => src.Current;
        object IEnumerator.Current => Current;
        public bool IsFailed => true;

        public bool MoveNext() => src.MoveNext();
        public void Reset() => src.Reset();
        public void Dispose() => src.Dispose();
    }
}
