using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class KeySelectEnumerable_Tuple<TKey, TValue> : EnumerableTypeConverterBase<Tuple<TKey, TValue>, TKey>
    {
        public KeySelectEnumerable_Tuple(IEnumerable<Tuple<TKey, TValue>> collection) : base(collection) { }
        public KeySelectEnumerable_Tuple(IEnumerator<Tuple<TKey, TValue>> innerEnumerator) : base(innerEnumerator) { }
        public override TKey Current => _innerEnumerator.Current.Item1;
    }
}
