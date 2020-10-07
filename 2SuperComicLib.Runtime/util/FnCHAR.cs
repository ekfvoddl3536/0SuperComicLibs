namespace SuperComicLib.Runtime
{
    public static unsafe class FnCHAR
    {
        public static bool Equals(char* src, int len, string value)
        {
            if (value.Length != len)
                return false;
       
            while (--len >= 0)
                if (src[len] != value[len])
                    return false;

            return true;
        }

        public static bool IsNewLine(char value) => value == '\r' || value == '\n';
    }
}
