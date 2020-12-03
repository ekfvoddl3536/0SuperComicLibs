using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IForwardIteratorExtension
    {
        public static IEnumerable<T> ToEnumerator<T>(this IForwardIterator<T> inst) =>
            new IteratorToEnumeratorWrapper<T>(inst);
    }
}
