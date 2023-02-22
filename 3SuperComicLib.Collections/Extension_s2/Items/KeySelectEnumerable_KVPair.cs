using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class KeySelectEnumerable_KVPair<TKey, TValue> : EnumerableTypeConverterBase<KeyValuePair<TKey, TValue>, TKey>
    {
        public KeySelectEnumerable_KVPair(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }
        public KeySelectEnumerable_KVPair(IEnumerator<KeyValuePair<TKey, TValue>> innerEnumerator) : base(innerEnumerator) { }
        public override TKey Current => _innerEnumerator.Current.Key;
    }
}
