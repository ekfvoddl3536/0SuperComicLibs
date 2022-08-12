namespace SuperComicLib.Text
{
    public static class HexCharInvalidCheckerExtension
    {
        public static bool IsInvalidHex(this char c)
        {
            // A ~ F(a ~ f), 0 ~ 9
            const int VALID_MAP = 0x7E_03FF;

            int bit = ((c & 0x40) >> 2) | (c & 0xF);

            return ((VALID_MAP >> bit) & 1) == 0;
        }

        public static unsafe bool IsInvalidHexString(this string str)
        {
            const int VALID_MAP = 0x7E_03FF;

            fixed (char* psrc = str)
            {
                char* ptr = psrc;
                for (int i = str.Length; --i >= 0;)
                {
                    int tmp = *ptr++;
                    int bit = ((tmp & 0x40) >> 2) | (tmp & 0xF);

                    if (((VALID_MAP >> bit) & 1) == 0)
                        return true;
                }
            }

            return false;
        }
    }
}
