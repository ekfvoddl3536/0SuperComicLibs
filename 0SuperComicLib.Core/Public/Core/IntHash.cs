using System.Runtime.CompilerServices;

namespace SuperComicLib.Core
{
    public static class IntHash
    {
        /// <summary>
        /// useful when combining array hashcodes
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static int CombineMOD(int value, int hashcode) =>
            unchecked(((value << 5) + value) ^ hashcode);

        /// <summary>
        /// simplest. not special
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static int Combine(int value, int hashcode) =>
            unchecked(31 * value + hashcode);

        /// <summary>
        /// simplest. not special
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static int Combine(int value, int hashcode, int key) => 
            unchecked((key | 1) * value + hashcode);
    }
}
