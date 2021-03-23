using System;

namespace SuperComicLib.Text
{
    public static class FRngStringExtension
    {
        public static HashedString ToHashedString(this in FRngString value)
        {
            if (value.IsEmpty)
                throw new ArgumentNullException(nameof(value));

            int sidx = value.sidx;
            int cnt = value.eidx - sidx;
            return new HashedString(value.str.GetFixedHashcode(sidx, cnt), cnt);
        }

        public static int ToInt32(this FRngString value)
        {
            string str = value.str;
            int sidx = value.sidx;
            int eidx = value.eidx;

            char first = str[sidx];

            int result;
            if (char.IsDigit(first))
                result = first - '0';
            else
            {
                sidx++;
                result = 0;
            }

            for (; sidx < eidx; sidx++)
            {
                char c = str[sidx];
                if (!char.IsDigit(c))
                    return 0;
                else
                    result = result * 10 + (c - '0');
            }

            return
                first == '-'
                ? -result
                : result;
        }
    }
}
