﻿using System;

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

        public static bool TryToInt32(this in FRngString value, out int result)
        {
            string str = value.str;
            int sidx = value.sidx;
            int eidx = value.eidx;

            char first = str[sidx];

            int locr;
            if (char.IsDigit(first))
                locr = first - '0';
            else
            {
                sidx++;
                locr = 0;
            }

            for (; sidx < eidx; sidx++)
            {
                char c = str[sidx];
                if (!char.IsDigit(c))
                {
                    result = 0;
                    return false;
                }
                else
                    locr = locr * 10 + (c - '0');
            }

            result = 
                first == '-' 
                ? -locr 
                : locr;
            return true;
        }

        public static int ToInt32(this in FRngString value)
        {
            TryToInt32(value, out int result);
            return result;
        }

        public static decimal ToDecimal(this in FRngString value) => decimal.Parse(value.ToString());

        public static double ToDouble(this in FRngString value) => double.Parse(value.ToString());

        public static float ToFloat(this in FRngString value) => float.Parse(value.ToString());
    }
}
