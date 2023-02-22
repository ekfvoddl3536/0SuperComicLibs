using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class SystemCollectionsGenericExtension
    {
        #region ienumerable -> map entry
        public static IMapEntryEnumerator<TKey, TValue> GetMapEntryEnumerator<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> @this) =>
            new MapEntryEnumerable_Tuple<TKey, TValue>(@this.GetEnumerator());
        public static IMapEntryEnumerator<TKey, TValue> GetMapEntryEnumerator<TKey, TValue>(this IEnumerable<ValueTuple<TKey, TValue>> @this) =>
            new MapEntryEnumerable_ValueTuple<TKey, TValue>(@this.GetEnumerator());
        public static IMapEntryEnumerator<TKey, TValue> GetMapEntryEnumerator<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this) =>
            new MapEntryEnumerable_KVPair<TKey, TValue>(@this.GetEnumerator());
        public static IMapEntryEnumerator<TKey, TValue> GetMapEntryEnumerator<TKey, TValue>(this IEnumerable<TKey> @this, IEnumerable<TValue> values) =>
            new MapEntryEnumerable_Pair<TKey, TValue>(@this.GetEnumerator(), values.GetEnumerator());
        #endregion

        #region ienumerator -> map entry
        public static IMapEntryEnumerator<TKey, TValue> AsMapEntryEnumerator<TKey, TValue>(this IEnumerator<Tuple<TKey, TValue>> @this) =>
            @this is MapEntryEnumeratorTypeWrapper<TKey, TValue> cc 
            ? cc._innerEnumerator
            : new MapEntryEnumerable_Tuple<TKey, TValue>(@this);
        public static IMapEntryEnumerator<TKey, TValue> AsMapEntryEnumerator<TKey, TValue>(this IEnumerator<ValueTuple<TKey, TValue>> @this) =>
            @this is IMapEntryEnumerator<TKey, TValue> cc
            ? cc
            : new MapEntryEnumerable_ValueTuple<TKey, TValue>(@this);
        public static IMapEntryEnumerator<TKey, TValue> AsMapEntryEnumerator<TKey, TValue>(this IEnumerator<KeyValuePair<TKey, TValue>> @this) =>
            @this is MapEntryEnumeratorTypeWrapper<TKey, TValue> cc
            ? cc._innerEnumerator
            : new MapEntryEnumerable_KVPair<TKey, TValue>(@this);
        public static IMapEntryEnumerator<TKey, TValue> AsMapEntryEnumerator<TKey, TValue>(this IEnumerator<TKey> @this, IEnumerator<TValue> values) =>
            new MapEntryEnumerable_Pair<TKey, TValue>(@this, values);
        #endregion

        #region ienumerable (keys, values)
        public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> @this) =>
            new KeySelectEnumerable_Tuple<TKey, TValue>(@this);
        public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<ValueTuple<TKey, TValue>> @this) =>
            new KeySelectEnumerable_ValueTuple<TKey, TValue>(@this);
        public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this) =>
            new KeySelectEnumerable_KVPair<TKey, TValue>(@this);

        public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> @this) =>
            new ValueSelectEnumerable_Tuple<TKey, TValue>(@this);
        public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<ValueTuple<TKey, TValue>> @this) =>
            new ValueSelectEnumerable_ValueTuple<TKey, TValue>(@this);
        public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this) =>
            new ValueSelectEnumerable_KVPair<TKey, TValue>(@this);
        #endregion

        #region ienumerator (keys, values)
        public static IEnumerator<TKey> AsKeyEnumerator<TKey, TValue>(this IEnumerator<Tuple<TKey, TValue>> @this) =>
            new KeySelectEnumerable_Tuple<TKey, TValue>(@this);
        public static IEnumerator<TKey> AsKeyEnumerator<TKey, TValue>(this IEnumerator<ValueTuple<TKey, TValue>> @this) =>
            new KeySelectEnumerable_ValueTuple<TKey, TValue>(@this);
        public static IEnumerator<TKey> AsKeyEnumerator<TKey, TValue>(this IEnumerator<KeyValuePair<TKey, TValue>> @this) =>
            new KeySelectEnumerable_KVPair<TKey, TValue>(@this);

        public static IEnumerator<TValue> AsValueEnumerator<TKey, TValue>(this IEnumerator<Tuple<TKey, TValue>> @this) =>
            new ValueSelectEnumerable_Tuple<TKey, TValue>(@this);
        public static IEnumerator<TValue> AsValueEnumerator<TKey, TValue>(this IEnumerator<ValueTuple<TKey, TValue>> @this) =>
            new ValueSelectEnumerable_ValueTuple<TKey, TValue>(@this);
        public static IEnumerator<TValue> AsValueEnumerator<TKey, TValue>(this IEnumerator<KeyValuePair<TKey, TValue>> @this) =>
            new ValueSelectEnumerable_KVPair<TKey, TValue>(@this);
        #endregion
    }
}
