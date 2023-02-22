using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class ValueSelectEnumerable_Tuple<TKey, TValue> : EnumerableTypeConverterBase<Tuple<TKey, TValue>, TValue>
    {
        public ValueSelectEnumerable_Tuple(IEnumerable<Tuple<TKey, TValue>> collection) : base(collection) { }
        public ValueSelectEnumerable_Tuple(IEnumerator<Tuple<TKey, TValue>> innerEnumerator) : base(innerEnumerator) { }
        public override TValue Current => _innerEnumerator.Current.Item2;
    }
}
