using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class KeyValueTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public readonly TKey[] Keys;
        public readonly TValue[] Values;
        private int m_index;

        public KeyValueTable(int fixedSize)
        {
            if (fixedSize <= 0)
                throw new ArgumentOutOfRangeException($"invalid size: '{fixedSize}'");

            Keys = new TKey[fixedSize];
            Values = new TValue[fixedSize];
        }

        public int Capacity => Keys.Length;

        public int Count => m_index;

        public void Add(TKey key, TValue value)
        {
            var ks = Keys;
            if (m_index < 0 || m_index >= ks.Length)
                throw new IndexOutOfRangeException("need more capacity");

            ref int idx = ref m_index;
            ks[idx] = key;
            Values[idx++] = value;
        }

        public IEnumeratorPair<TKey, TValue> GetEnumeratorPair() => 
            new EnumerablePair<TKey, TValue>(Keys, Values, m_index);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            new InnerEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        private sealed class InnerEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly TKey[] m_keys;
            private readonly TValue[] m_values;
            private int m_idx;

            public InnerEnumerator(KeyValueTable<TKey, TValue> inst)
            {
                m_keys = inst.Keys;
                m_values = inst.Values;
                m_idx = -1;
            }

            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(m_keys[m_idx], m_values[m_idx]);

            object IEnumerator.Current => Current;

            public bool MoveNext() => ++m_idx < m_keys.Length;

            public void Reset() => m_idx = -1;

            public void Dispose() { }
        }
    }
}
