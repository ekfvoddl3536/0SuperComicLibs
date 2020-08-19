namespace SuperComicLib.Runtime
{
    public static unsafe class FnCHAR
    {
        public static bool StartsWith(char* src, int len, string value)
        {
            int x = value.Length;
            len = x < len ? x : len;
            if (len < 1)
                return false;

            for (x = 0; x < len; x++)
                if (src[x] != value[x])
                    return false;

            return true;
        }
    }
}
