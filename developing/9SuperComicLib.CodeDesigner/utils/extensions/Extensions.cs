namespace SuperComicLib.CodeDesigner
{
    internal static class Extensions
    {
        //public static TableItem Find(this TableItem[] arr, int state)
        //{
        //    int x = arr.Length;
        //    while (--x >= 0)
        //        if (arr[x].state == state)
        //            return arr[x];
        //
        //    return default;
        //}

        public static int ToNonterminal(this string str) => 
            (str.GetFixedHashcode() & int.MaxValue) | ExpressInt.nonterminal_flag;

        public static bool IsTerminal(this int val) => (val & ExpressInt.nonterminal_flag) == 0;
    }
}
