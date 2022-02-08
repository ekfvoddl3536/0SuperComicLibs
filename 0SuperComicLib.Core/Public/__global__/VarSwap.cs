using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static class VarSwap
    {
        /// <summary>
        /// use local-var. not special
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Local<T>(ref T item1, ref T item2)
        {
            T backup = item1;
            item1 = item2;
            item2 = backup;
        }

        /// <summary>
        /// use local-var. return previous <paramref name="item1"/> value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RetLocal<T>(ref T item1, in T item2)
        {
            T backup = item1;
            item1 = item2;
            return backup;
        }

        /// <summary>
        /// use out-parameter.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Param<T>(T item1, ref T item2, out T item2_backup)
        {
            item2_backup = item2;
            item2 = item1;
        }
    }
}
