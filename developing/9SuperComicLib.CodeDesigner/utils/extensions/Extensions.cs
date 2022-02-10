using System.Runtime.CompilerServices;

namespace SuperComicLib.CodeDesigner
{
    internal static class Extensions
    {
        // public static int ToNonterminal(this string str) => (str.GetFixedHashcode() & int.MaxValue) | ExpressInt.nonterminal_flag;
        // 
        // public static int ToNonterminalEX(this string str)
        // {
        //     if (str[0] == '<')
        //         str = str.Substring(1, str.Length - 2);
        // 
        //     return str.ToNonterminal();
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static int ToNonterminal(this int val) => val | ExpressInt.nonterminal_flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static int ToIndex(this int val) => val & ExpressInt.nonterminal_bitmask;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        public static bool IsTerminal(this int val) => (val & ExpressInt.nonterminal_flag) == 0;
    }
}
