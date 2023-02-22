using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class ValueSelectEnumerable_ValueTuple<TKey, TValue> : EnumerableTypeConverterBase<ValueTuple<TKey, TValue>, TValue>
    {
        public ValueSelectEnumerable_ValueTuple(IEnumerable<ValueTuple<TKey, TValue>> collection) : base(collection) { }
        public ValueSelectEnumerable_ValueTuple(IEnumerator<ValueTuple<TKey, TValue>> innerEnumerator) : base(innerEnumerator) { }
        public override TValue Current => _innerEnumerator.Current.Item2;
    }
}
