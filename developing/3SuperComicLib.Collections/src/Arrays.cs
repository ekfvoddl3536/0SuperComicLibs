using System.Collections.Generic;
using SuperComicLib.Core;

namespace SuperComicLib.Collections
{
    public static class Arrays
    {
        public static int HashCodeFast<T>(this T[] arr) where T : struct
        {
            if (arr == null)
                return 0;

            int x = arr.Length;
            int result = 7;
            while (--x >= 0)
                IntHash.CombineMOD(ref result, arr[x].GetHashCode());

            return result;
        }

        public static int HashCodeFast<T>(this IEnumerable<T> arr) where T : struct
        {
            if (arr == null)
                return 0;

            int result = 7;
            foreach (T v in arr)
                IntHash.CombineMOD(ref result, v.GetHashCode());

            return result;
        }

        public static int HashCode<T>(this T[] arr)
        {
            if (arr == null)
                return 0;

            int x = arr.Length;
            int result = 7;
            while (--x >= 0)
                IntHash.CombineMOD(ref result, arr[x]?.GetHashCode() ?? 0);

            return result;
        }

        public static int HashCode<T>(this IEnumerable<T> arr)
        {
            if (arr == null)
                return 0;

            int result = 7;
            foreach (T v in arr)
                IntHash.CombineMOD(ref result, v?.GetHashCode() ?? 0);

            return result;
        }
    }
}
