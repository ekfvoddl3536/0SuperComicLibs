using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class MapEntryEnumerable_Tuple<TKey, TValue> : IMapEntryEnumerator<TKey, TValue>
    {
        private readonly IEnumerator<Tuple<TKey, TValue>> src;

        public MapEntryEnumerable_Tuple(IEnumerator<Tuple<TKey, TValue>> src) => this.src = src;

        public ValueTuple<TKey, TValue> Current
        {
            get
            {
                var current = src.Current;
                return new ValueTuple<TKey, TValue>(current.Item1, current.Item2);
            }
        }
        object IEnumerator.Current => Current;
        public bool IsFailed => true;

        public bool MoveNext() => src.MoveNext();
        public void Reset() => src.Reset();
        public void Dispose() => src.Dispose();
    }
}
