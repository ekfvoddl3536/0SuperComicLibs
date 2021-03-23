namespace SuperComicLib.Arithmetic
{
    public static class ToInteger
    {
        public static int Positive(string str, int begin)
        {
            int result = 0;
            int length = str.Length;
            for (int x = 8; --x >= 0 && begin < length;)
                result = (result + (str[begin++] - '0')) * 10;

            return result;
        }

        public static uint Unsigned(string str, int begin)
        {
            uint result = 0;
            int length = str.Length;
            for (int x = 9; --x >= 0 && begin < length;)
                result = (result + (uint)(str[begin++] - '0')) * 10;

            return result;
        }
    }
}
