using System;
using System.Collections.Generic;

namespace SuperComicLib
{
    public static class SCL_GLOBAL_StringExtension
    {
        private const int hashcode_start_c = (5381 << 16) + 5381;
        private const int hashcode_last_c = 1566083941;

        #region other
        // public static string RemoveBack(this string str, int count) => str.Remove(str.Length - count, count);
        public static string RemoveBack(this string str, int count) => str.Substring(0, str.Length - count);

        public static unsafe bool StartsWith(this string str, char* ptr, int cnt)
        {
            if (str.Length < cnt)
                return false;

            while (--cnt >= 0)
                if (str[cnt] != ptr[cnt])
                    return false;

            return true;
        }
        #endregion

        #region split
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
                end2:
                    ;
                }

                x++;
            end:
                ;
            }

            if (st < x) // 아직 남은 문자가 있다
                vs.Add(str.Substring(st));

            bounds = bds.ToArray();
            return vs.ToArray();
        }

        public static string[] Split(this string str, string value) => str.Split(value.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitExcept(this string str, char split, char except_char)
        {
            List<string> vs = new List<string>();

            int x = 0, st = 0;
            for (int max = str.Length; x < max; x++)
                if (str[x] == except_char)
                    x++; // skip
                else if (str[x] == split)
                {
                    int cnt = x - st;

                    vs.Add(str.Substring(st, cnt));

                    st += cnt + 1;
                }

            if (st < x) // 아직 남은 문자가 있다
                vs.Add(str.Substring(st));

            return vs.ToArray();
        }

        public static string[] SplitExcept(this string str, char split, Predicate<char> except_char)
        {
            List<string> vs = new List<string>();

            int x = 0, st = 0;
            for (int max = str.Length; x < max; x++)
                if (except_char.Invoke(str[x]))
                    x++; // skip
                else if (str[x] == split)
                {
                    int cnt = x - st;

                    vs.Add(str.Substring(st, cnt));

                    st += cnt + 1;
                }

            if (st < x) // 아직 남은 문자가 있다
                vs.Add(str.Substring(st));

            return vs.ToArray();
        }

        public static string[] SplitExceptRange(this string str, char split, char except_begin, char except_end)
        {
            List<string> vs = new List<string>();

            bool skip = false;
            char c;

            int x = 0, st = 0;
            for (int max = str.Length; x < max; x++)
                if (skip)
                {
                    if (str[x] == except_end)
                        skip = false;
                }
                else if ((c = str[x]) == except_begin)
                    skip = true;
                else if (c == split)
                {
                    int cnt = x - st;

                    vs.Add(str.Substring(st, cnt));

                    st += cnt + 1;
                }

            if (st < x) // 아직 남은 문자가 있다
                vs.Add(str.Substring(st));

            return vs.ToArray();
        }
        #endregion

        #region index
        public static int IndexOfWhitespace(this string str)
        {
            for (int x = 0, max = str.Length; x < max; x++)
                if (char.IsWhiteSpace(str[x]))
                    return x;
            return -1;
        }

        public static int IndexOfExcept(this string str, char value, char except_char)
        {
            int x = 0, max = str.Length;
            for (; x < max; x++)
                if (str[x] == except_char)
                    x++;
                else if (str[x] == value)
                    return x;

            return -1;
        }

        public static int LastIndexOfExcept(this string str, char value, char except_char)
        {
            int x = str.Length;
            while (--x >= 0)
                if (str[x] == except_char)
                    x--;
                else if (str[x] == value)
                    return x;

            return -1;
        }

        public static int IndexOfExcept(this string str, char value, char except_begin, char except_end)
        {
            bool skip = false;

            int x = 0, max = str.Length;
            for (; x < max; x++)
                if (str[x] == except_begin)
                    skip = true;
                else if (str[x] == except_end)
                    skip = false;
                else if (!skip && str[x] == value)
                    return x;

            return -1;
        }

        public static int LastIndexOfExcept(this string str, char value, char except_begin, char except_end)
        {
            bool skip = false;

            int x = str.Length;
            while (--x >= 0)
                if (str[x] == except_end)
                    skip = true;
                else if (str[x] == except_begin)
                    skip = false;
                else if (!skip && str[x] == value)
                    return x;

            return -1;
        }
        #endregion

        #region hashcode
        public static unsafe int GetFixedHashcode(this string str)
        {
#if DEBUG
            System.Diagnostics.Contracts.Contract.Requires(str != null);
#endif
            int hash1 = hashcode_start_c;
            int hash2 = hash1;

            fixed (char* src = str)
                Internal_Hashcode(ref hash1, ref hash2, (int*)src, str.Length);

            return hash1 + hash2 * hashcode_last_c;
        }

        public static unsafe int GetFixedHashcode(this string str, IEnumerable<string> additionalStrings) =>
            GetFixedHashcode(str, additionalStrings, out _);

        public static unsafe int GetFixedHashcode(this string str, IEnumerable<string> additionalStrings, out int totalLength)
        {
#if DEBUG
            System.Diagnostics.Contracts.Contract.Requires(str != null);
            System.Diagnostics.Contracts.Contract.Requires(additionalStrings != null);
#endif
            int hash1 = hashcode_start_c;
            int hash2 = hash1;

            fixed (char* src = str)
                totalLength = Internal_Hashcode(ref hash1, ref hash2, (int*)src, str.Length);

            // too long
            var iter = additionalStrings.GetEnumerator();

            while (iter.MoveNext())
            {
                str = iter.Current;
                fixed (char* src = str)
                    totalLength += Internal_Hashcode(ref hash1, ref hash2, (int*)src, str.Length);
            }

            iter.Dispose();

            return hash1 + hash2 * hashcode_last_c;
        }

        public static unsafe int GetFixedHashcode(this string str, int startidx, int count)
        {
            if (startidx < 0 ||
                count < 1 ||
                startidx + count > str.Length)
                throw new ArgumentOutOfRangeException();

            int hash1 = hashcode_start_c;
            int hash2 = hash1;

            fixed (char* src = str)
                Internal_Hashcode(ref hash1, ref hash2, (int*)(src + startidx), count);

            return hash1 + hash2 * hashcode_last_c;
        }

        private static unsafe int Internal_Hashcode(ref int hash1, ref int hash2, int* pint, int len)
        {
            int x = len;
            while (x > 2)
            {
                hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                pint += 2;
                x -= 4;
            }

            if (x > 0)
                hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];

            return len;
        }
        #endregion
    }
}
