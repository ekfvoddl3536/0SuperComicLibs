using System.Runtime.CompilerServices;

namespace SuperComicLib.Core
{
    public static class IntHash
    {
        /// <summary>
        /// useful when combining array hashcodes
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static void CombineMOD(ref int value, int hashcode) =>
            value = unchecked(((value << 5) + value) ^ hashcode);

        /// <summary>
        /// simplest. not special
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static void Combine(ref int value, int hashcode) =>
            value = unchecked(31 * value + hashcode);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static void Combine(ref int value, int hashcode, int key) =>
            value = unchecked((key | 1) * value + hashcode);
    }
}
