namespace SuperComicLib.Runtime
{
    public static class FsInt
    {
        public static int Parse(char value) => value - '0';

        public static bool TryParse(char value, out int result)
        {
            if (value >= '0' && value <= '9')
            {
                result = value - '0';
                return true;
            }

            result = 0;
            return false;
        }

        public static int Parse(string value)
        {
            int len = value.Length - 1;

            int result = 0;
            int e = 1;
            for (int x = len; x >= 0 && e > 0; x--, e = 10 * (len - x))
                result += (value[x] - '0') * e;

            return result;
        }

        public static bool TryParse(string value, out int result)
        {
            int len = value.Length - 1;

            result = 0;
            int e = 1;
            for (int x = len; x >= 0 && e > 0; x--, e = 10 * (len - x))
                if (TryParse(value[x], out int tmp))
                    result += tmp * e;
                else
                    return false;

            return true;
        }

        public static int Parse(string value, int idx)
        {
            int len = value.Length;

            int result = 0;
            int e = 1;
            for (int x = idx; x < len && e > 0; x++, e = 10 * (x - idx))
                if (TryParse(value[x], out int tmp))
                    result = (result + tmp) * e;
                else
                    break;

            return result;
        }

        public static int ParseHex(char value) =>
            value >= 'A' && value <= 'F' || value >= 'a' && value <= 'f'
            ? (value & 7) + 9
            : value >= '0' && value <= '9'
            ? value - '0'
            : -1;
    }
}
