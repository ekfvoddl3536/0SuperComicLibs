using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class MapEntryEnumerable_Pair<TKey, TValue> : IMapEntryEnumerator<TKey, TValue>
    {
        private readonly IEnumerator<TKey> keys;
        private readonly IEnumerator<TValue> values;
        private bool m_state;

        public MapEntryEnumerable_Pair(IEnumerator<TKey> keys, IEnumerator<TValue> values)
        {
            this.keys = keys;
            this.values = values;
        }

        public ValueTuple<TKey, TValue> Current => new ValueTuple<TKey, TValue>(keys.Current, values.Current);
        object IEnumerator.Current => Current;
        public bool IsFailed => m_state;

        public bool MoveNext()
        {
            bool first = keys.MoveNext();
            bool second = values.MoveNext();

            m_state = first ^ second;

            return first & second;
        }

        public void Reset()
        {
            keys.Reset();
            values.Reset();
        }
        public void Dispose()
        {
            keys.Dispose();
            values.Dispose();
        }
    }
}
