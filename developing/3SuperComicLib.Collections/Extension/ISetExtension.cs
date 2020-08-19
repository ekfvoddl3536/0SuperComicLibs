using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class ISetExtension
    {
        public static bool AddRange<T>(this ISet<T> set, IEnumerable<T> items)
        {
            bool changed = false;
            foreach (T item in items)
                changed |= set.Add(item);

            return changed;
        }
    }
}
