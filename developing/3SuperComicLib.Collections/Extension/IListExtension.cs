using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IListExtension
    {
        public static IForwardIterator<T> Begin<T>(this IList<T> item) => new List_Iterator<T>(item);

        public static IForwardIterator<T> RBegin<T>(this IList<T> item) => new List_ReverseIterator<T>(item);

        public static IAddOnlyList<T> AsAddOnly<T>(this IList<T> item) => new List_AddOnlyWrapper<T>(item);

        public static void RemoveBack<T>(this IList<T> item, int count)
        {
            int idx = item.Count;
            int max = idx - count;

            while (idx > max)
            {
                idx--;
                item[idx] = default;
            }
        }

        public static void ReplaceAll_C<T>(this IList<T> item, T replace, int begin, int __COUNT__)
        {
            int max = item.Count.Min(begin + __COUNT__);
            for (; begin < max; begin++)
                item[begin] = replace;
        }

        public static void ReplaceAll<T>(this IList<T> item, T replace, int begin, int __END__)
        {
            int max = (item.Count - 1).Min(__END__);
            for (; begin <= max; begin++)
                item[begin] = replace;
        }

        public static void ReplaceRange_C<T>(this IList<T> item, T replace, int begin, int __COUNT__)
        {
            if (begin < item.Count)
            {
                item[begin] = replace;

                int max = item.Count.Min(begin + __COUNT__);
                for (begin++; begin < max; begin++)
                    item.RemoveAt(begin);
            }
        }

        public static void ReplaceRange<T>(this IList<T> item, T replace, int begin, int __END__)
        {
            if (begin < item.Count)
            {
                item[begin] = replace;

                int max = (item.Count - 1).Min(__END__);
                for (begin++; begin <= max; begin++)
                    item.RemoveAt(begin);
            }
        }
    }
}
