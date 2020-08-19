using System;
using System.Collections.Generic;

namespace SuperComicLib
{
    public static class StringExtension
    {
        // public static string RemoveBack(this string str, int count) => str.Remove(str.Length - count, count);
        public static string RemoveBack(this string str, int count) => str.Substring(0, str.Length - count);

        public static string[] Split_s(this string str, char[] chars, out char[] bounds)
        {
            List<string> vs = new List<string>();
            List<char> bds = new List<char>();

            int x = 0, st = 0;
            for (int max = str.Length; x < max; x++)
            {
                foreach (char d in chars)
                    if (str[x] == d)
                    {
                        int cnt = x - st;

                        vs.Add(str.Substring(st, cnt));
                        bds.Add(d);

                        st += cnt + 1;

                        break;
                    }
            }

            if (st < x) // 아직 남은 문자가 있다
                vs.Add(str.Substring(st));

            bounds = bds.ToArray();
            return vs.ToArray();
        }

        public static string[] Split_s(this string str, string[] strs, out string[] bounds)
        {
            List<string> vs = new List<string>();
            List<string> bds = new List<string>();

            int strsMax = strs.Length;
            int[] strsLen = Array.ConvertAll(strs, s => s.Length);

            int x = 0, st = 0;
            for (int max = str.Length - 1; x < max;) // 마지막 한 문자는 제외
            {
                for (int z = 0; z < strsMax; z++)
                {
                    int sl = strsLen[z];
                    if (x + sl >= max)
                        continue;

                    string ztmp = strs[z];
                    for (int tmp = x, i = 0; i < sl; i++, tmp++)
                        if (ztmp[i] != str[tmp])
                            goto end2;

                    // matched
                    int cnt = x - st;

                    vs.Add(str.Substring(st, cnt));

                    st += cnt;
                    bds.Add(str.Substring(st, sl));

                    st += sl;
                    x += sl;

                    goto end;
                    end2:;
                }

                x++;
                end:;
            }

            if (st < x) // 아직 남은 문자가 있다
                vs.Add(str.Substring(st));

            bounds = bds.ToArray();
            return vs.ToArray();
        }

        public static string[] Split(this string str, string value) => str.Split(value.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        public static int IndexOfWhitespace(this string str)
        {
            for (int x = 0, max = str.Length; x < max; x++)
                if (char.IsWhiteSpace(str[x]))
                    return x;
            return -1;
        }

        public static unsafe int GetFixedHashcode(this string str)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(str != null);
#endif
            fixed (char* src = str)
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                int* pint = (int*)src;
                int len = str.Length;
                while (len > 2)
                {
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                    pint += 2;
                    len -= 4;
                }

                if (len > 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];

                return hash1 + hash2 * 1566083941;
            }
        }

        public static unsafe bool StartsWith(this string str, char* ptr, int cnt)
        {
            if (str.Length < cnt)
                return false;

            while (--cnt >= 0)
                if (str[cnt] != ptr[cnt])
                    return false;

            return true;
        }
    }
}
