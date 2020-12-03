namespace SuperComicLib.Core
{
    public static class Whitespace
    {
        public static readonly string w1 = Make(1);
        public static readonly string w2 = Make(2);
        public static readonly string w4 = Make(4);
        public static readonly string w8 = Make(8);

        /// <summary>
        /// using <see cref="Get(int)"/> method instead
        /// </summary>
        public static string Make(int cnt)
        {
            string str = string.Empty;
            for (; cnt > 0; cnt--)
                str += " ";

            return str;
        }

        /// <summary>
        /// better than <see cref="Make(int)"/> method.
        /// </summary>
        public static string Get(int cnt)
        {
            string str = string.Empty;
            for (; cnt > 8; cnt -= 8)
                str += w8;

            if (cnt > 4)
            {
                str += w4;
                cnt -= 4;
            }
            if (cnt > 2)
            {
                str += w2;
                cnt -= 2;
            }
            if (cnt != 0)
                str += w1;

            return str;
        }
    }
}
