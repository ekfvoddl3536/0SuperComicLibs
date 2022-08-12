using System.Collections.Generic;

namespace SuperComicLib.Collections.Concurrent
{
    public static class IEnumerator_IEnumerable_Extension
    {
        public static ConcurrentEnumerator<T> ToConcurrent<T>(this IEnumerator<T> enumerator) =>
            new ConcurrentEnumerator<T>(enumerator);

        public static ConcurrentEnumerator<T> GetConcurrentEnumerator<T>(this IEnumerable<T> collection) =>
            new ConcurrentEnumerator<T>(collection.GetEnumerator());
    }
}
