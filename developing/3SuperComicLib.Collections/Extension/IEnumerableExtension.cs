using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> FastSelect<T>(this IEnumerable<T> collection, T item, IEqualityComparer<T> comparer) =>
            new Selector<T>(item, collection.GetEnumerator(), comparer);

        public static IEnumerable<T> FastSelect<T>(this IEnumerable<T> collection, T item) =>
            FastSelect(collection, item, EqualityComparer<T>.Default);

        public static IEnumerable<T> FastClone<T>(this IEnumerable<T> collection, Func<T, T> callback) =>
            new ElementCloner<T>(collection.GetEnumerator(), callback);

        public static bool ContainsAll<T>(this IEnumerable<T> collection, IEnumerable<T> items)
        {
            IEnumerator<T> src = collection.GetEnumerator();
            IEnumerator<T> other = items.GetEnumerator();

            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;

            while (other.MoveNext() && src.MoveNext())
                if (!comparer.Equals(other.Current, src.Current))
                {
                    src.Dispose();
                    other.Dispose();

                    return false;
                }

            src.Dispose();
            other.Dispose();

            return true;
        }

        public static IEnumerable<T> Limit<T>(this IEnumerable<T> collection, int limit) =>
            new LimitEnumerable<T>(collection, limit);

        public static IEnumerable<TOut> FastConvertAll<TIn, TOut>(this IEnumerable<TIn> collection, Converter<TIn, TOut> converter)
        {
#if DEBUG
            System.Diagnostics.Contracts.Contract.Requires(collection != null);
            System.Diagnostics.Contracts.Contract.Requires(converter != null);
#endif
            return new EnumerableConvert<TIn, TOut>(collection, converter);
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, T key) =>
            IndexOf(collection, key, EqualityComparer<T>.Default);

        public static int IndexOf<T>(this IEnumerable<T> collection, T key, IEqualityComparer<T> comparer)
        {
            int idx = 0;

            IEnumerator<T> e1 = collection.GetEnumerator();
            for (; e1.MoveNext(); idx++)
                if (comparer.Equals(e1.Current, key))
                    return idx;

            e1.Dispose();

            return -1;
        }

        public static IIterable<T> AsIterable<T>(this IEnumerable<T> collection) =>
            new EnumerableToIterable<T>(collection);
    }
}
