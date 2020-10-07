using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class ICollectionExtension
    {
        public static ICountObserver GetObserver(this ICollection collection) => new CountObserver(collection);

        public static ICountObserver GetObserver<T>(this ICollection<T> collection) => new CountObserver<T>(collection);

        public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            IEnumerator<T> e1 = other.GetEnumerator();
            while (e1.MoveNext())
                if (!collection.Contains(e1.Current))
                {
                    e1.Dispose();
                    return false;
                }

            e1.Dispose();
            return true;
        }

        public static bool IsEqualsAll<T>(this ICollection<T> collection, ICollection<T> other)
        {
            if (collection.Count != other.Count)
                return false;

            IEqualityComparer<T> comp = EqualityComparer<T>.Default;

            IEnumerator<T> e1 = collection.GetEnumerator();
            IEnumerator<T> e2 = other.GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext())
                if (!comp.Equals(e1.Current, e2.Current))
                    return false;

            return true;
        }
    }
}
