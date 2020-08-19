using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class ArrayExtension
    {
        public static IForwardIterator<T> Begin<T>(this T[] array) => new Array_Iterator<T>(array);

        public static IForwardIterator<T> RBegin<T>(this T[] array) => new Array_ReverseIterator<T>(array);

        public static IRangeRefArray<T> Slice<T>(this T[] array, int start, int end) => new RangeArray<T>(array, start, end);

        public static IRangeRefArray<T> Slice<T>(this T[] array, int start, T end) => Slice(array, start, end, EqualityComparer<T>.Default);

        public static IRangeRefArray<T> Slice<T>(this T[] array, int start, T end, IEqualityComparer<T> comparer)
        {
            int max = array.Length;
            for (int x = start; x < max; x++)
                if (comparer.Equals(array[x], end))
                    return new RangeArray<T>(array, start, x);

            return null;
        }

        public static IRangeRefArray<T> Slice<T>(this T[] array, int max, Predicate<T> start_predicate, Predicate<T> end_predicate)
        {
            int x = 0;
            while (x < max && !start_predicate(array[x]))
                x++;

            int st = x;
            do
                x++;
            while (x < max && !end_predicate(array[x]));

            return
                st < max && x < max
                ? new RangeArray<T>(array, st, x)
                : new RangeArray<T>(array, 0, max - 1); // index compare
        }

        public static void Replace<T>(this T[] array, int idx, int cnt, T replace)
        {
            int max = array.Length;
            int k = idx + cnt;
            if (idx >= 0 && k <= max)
            {
                int x = idx + 1;
                if (x < k)
                {
                    for (; k < max; x++, k++)
                        array[x] = array[k];

                    for (k -= cnt; cnt > 1; cnt--)
                        array[++k] = default;
                }

                array[idx] = replace;
            }
#if DEBUG
            else
                System.Diagnostics.Debug.Fail("invalid operation");
#endif
        }

        public static void ReplaceUnsafe<T>(this T[] array, int idx, int cnt, T replace)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("[ WARN ] SKIP VALUE CHECK -> SCL::Collections::ArrayExtension");
#endif
            int max = array.Length;
            int k = idx + cnt;
            int x = idx + 1;
            if (x < k)
            {
                int fmax = max - cnt;
                for (; k < fmax; x++, k++)
                    array[x] = array[k];

                for (; k < max; k++)
                    array[k] = default;
            }

            array[idx] = replace;
        }

        public static bool FastSequenceEqual<T>(this T[] array, T[] other) where T : IEquatable<T>
        {
            int x = array.Length;
            if (x != other.Length)
                return false;

            while (--x >= 0)
                if (!array[x].Equals(other[x]))
                    return false;

            return true;
        }

        public static IEnumerablePair<T1, T2> MakePair<T1, T2>(this T1[] array, T2[] other) =>
            array.Length == other.Length 
            ? new EnumerablePair<T1, T2>(array, other, other.Length)
            : null;

        public static IEnumerablePair<T1, T2> MakePair<T1, T2>(this T1[] array, T2[] other, int max) =>
            array.Length.Min(max) == other.Length.Min(max)
            ? new EnumerablePair<T1, T2>(array, other, max)
            : null;
    }
}
