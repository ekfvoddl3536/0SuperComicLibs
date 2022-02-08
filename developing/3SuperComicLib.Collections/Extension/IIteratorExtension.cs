using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IIteratorExtension
    {
        public static IEnumerator<T> ToEnumerator<T>(this IValueIterator<T> inst) =>
            new IteratorToEnumeratorWrapper<T>(inst).GetEnumerator();
    }
}
