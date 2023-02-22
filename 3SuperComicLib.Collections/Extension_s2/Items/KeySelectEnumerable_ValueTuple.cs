using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class KeySelectEnumerable_ValueTuple<TKey, TValue> : EnumerableTypeConverterBase<ValueTuple<TKey, TValue>, TKey>
    {
        public KeySelectEnumerable_ValueTuple(IEnumerable<ValueTuple<TKey, TValue>> collection) : base(collection) { }
        public KeySelectEnumerable_ValueTuple(IEnumerator<ValueTuple<TKey, TValue>> innerEnumerator) : base(innerEnumerator) { }
        public override TKey Current => _innerEnumerator.Current.Item1;
    }
}
