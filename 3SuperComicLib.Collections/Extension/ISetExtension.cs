using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class ISetExtension
    {
        public static bool AddRange<T>(this ISet<T> set, IEnumerable<T> items)
        {
            bool changed = false;

            IEnumerator<T> e1 = items.GetEnumerator();
            while (e1.MoveNext())
                changed |= set.Add(e1.Current);

            return changed;
        }

        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> items, ref bool result)
        {
            IEnumerator<T> e1 = items.GetEnumerator();
            while (e1.MoveNext())
                result |= set.Add(e1.Current);
        }
    }
}
