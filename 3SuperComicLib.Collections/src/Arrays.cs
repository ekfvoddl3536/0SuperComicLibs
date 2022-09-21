using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                result = IntHash.CombineMOD(result, arr[x].GetHashCode());

            return result;
        }

        public static int HashCodeFast<T>(this IEnumerable<T> arr) where T : struct
        {
            if (arr == null)
                return 0;

            int result = 7;
            foreach (T v in arr)
                result = IntHash.CombineMOD(result, v.GetHashCode());

            return result;
        }

        public static int HashCode<T>(this T[] arr)
        {
            if (arr == null)
                return 0;

            int x = arr.Length;
            int result = 7;
            while (--x >= 0)
                result = IntHash.CombineMOD(result, arr[x]?.GetHashCode() ?? 0);

            return result;
        }

        public static int HashCode<T>(this IEnumerable<T> arr)
        {
            if (arr == null)
                return 0;

            int result = 7;
            foreach (T v in arr)
                result = IntHash.CombineMOD(result, v?.GetHashCode() ?? 0);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetNextSize(int oldSize)
        {
            const uint MaxArrayLength = 0x7FEF_FFFF;
            return (int)CMath.Min((uint)oldSize << 1, MaxArrayLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long GetNextSize(long oldSize)
        {
            const ulong MaxArrayLength = 0x7FEF_FFFF_FFFF; // 48 bits
            return (long)CMath.Min((ulong)oldSize << 1, MaxArrayLength);
        }
    }
}
