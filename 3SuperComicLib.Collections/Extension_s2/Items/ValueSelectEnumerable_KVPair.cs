using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class ValueSelectEnumerable_KVPair<TKey, TValue> : EnumerableTypeConverterBase<KeyValuePair<TKey, TValue>, TValue>
    {
        public ValueSelectEnumerable_KVPair(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }
        public ValueSelectEnumerable_KVPair(IEnumerator<KeyValuePair<TKey, TValue>> innerEnumerator) : base(innerEnumerator) { }
        public override TValue Current => _innerEnumerator.Current.Value;
    }
}
