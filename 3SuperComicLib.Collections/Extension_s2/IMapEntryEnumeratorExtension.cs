using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IMapEntryEnumeratorExtension
    {
        public static IFailableEnumerator<Tuple<TKey, TValue>> AsTuple<TKey, TValue>(this IMapEntryEnumerator<TKey, TValue> @this) =>
            new MapEntryEnumeratorTypeWrapper<TKey, TValue>(@this);

        public static IFailableEnumerator<KeyValuePair<TKey, TValue>> AsKeyValuePair<TKey, TValue>(this IMapEntryEnumerator<TKey, TValue> @this) =>
            new MapEntryEnumeratorTypeWrapper<TKey, TValue>(@this);
    }
}
