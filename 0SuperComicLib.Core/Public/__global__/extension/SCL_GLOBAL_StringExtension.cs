// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static class SCL_GLOBAL_StringExtension
    {
        private const int hashcode_start_c = (5381 << 16) + 5381;
        private const int hashcode_last_c = 1566083941;

        #region other
        // public static string RemoveBack(this string str, int count) => str.Remove(str.Length - count, count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveBack(this string str, int count) => str.Substring(0, str.Length - count);

        public static unsafe bool StartsWith(this string str, char* ptr, int cnt)
        {
            if (str.Length < cnt)
                return false;

            fixed (char* pstr = str)
            {
                for (char* pl = pstr, end = ptr + cnt; ptr != end;)
                    if (*pl++ != *ptr++)
                        return false;
            }

            return true;
        }
        #endregion

        #region split
        //public static string[] Split_s(this string str, char[] chars, out char[] bounds)
        //{
        //    List<string> vs = new List<string>();
        //    List<char> bds = new List<char>();

        //    int x = 0, st = 0;
        //    for (int max = str.Length; x < max; x++)
        //    {
        //        foreach (char d in chars)
        //            if (str[x] == d)
        //            {
        //                int cnt = x - st;

        //                vs.Add(str.Substring(st, cnt));
        //                bds.Add(d);

        //                st += cnt + 1;

        //                break;
        //            }
        //    }

        //    if (st < x) // 아직 남은 문자가 있다
        //        vs.Add(str.Substring(st));

        //    bounds = bds.ToArray();
        //    return vs.ToArray();
        //}

        //public static string[] Split_s(this string str, string[] strs, out string[] bounds)
        //{
        //    List<string> vs = new List<string>();
        //    List<string> bds = new List<string>();

        //    int strsMax = strs.Length;
        //    int[] strsLen = Array.ConvertAll(strs, s => s.Length);

        //    int x = 0, st = 0;
        //    for (int max = str.Length - 1; x < max;) // 마지막 한 문자는 제외
        //    {
        //        for (int z = 0; z < strsMax; z++)
        //        {
        //            int sl = strsLen[z];
        //            if (x + sl >= max)
        //                continue;

        //            string ztmp = strs[z];
        //            for (int tmp = x, i = 0; i < sl; i++, tmp++)
        //                if (ztmp[i] != str[tmp])
        //                    goto end2;

        //            // matched
        //            int cnt = x - st;

        //            vs.Add(str.Substring(st, cnt));

        //            st += cnt;
        //            bds.Add(str.Substring(st, sl));

        //            st += sl;
        //            x += sl;

        //            goto end;
        //        end2:
        //            ;
        //        }

        //        x++;
        //    end:
        //        ;
        //    }

        //    if (st < x) // 아직 남은 문자가 있다
        //        vs.Add(str.Substring(st));

        //    bounds = bds.ToArray();
        //    return vs.ToArray();
        //}

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

        // public static string[] SplitExceptRange(this string str, char split, char except_begin, char except_end)
        // {
        //     List<string> vs = new List<string>();
        // 
        //     bool skip = false;
        //     char c;
        // 
        //     int x = 0, st = 0;
        //     for (int max = str.Length; x < max; x++)
        //         if (skip)
        //         {
        //             if (str[x] == except_end)
        //                 skip = false;
        //         }
        //         else if ((c = str[x]) == except_begin)
        //             skip = true;
        //         else if (c == split)
        //         {
        //             int cnt = x - st;
        // 
        //             vs.Add(str.Substring(st, cnt));
        // 
        //             st += cnt + 1;
        //         }
        // 
        //     if (st < x) // 아직 남은 문자가 있다
        //         vs.Add(str.Substring(st));
        // 
        //     return vs.ToArray();
        // }
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
            Contract.Requires(str != null);

            int hash1 = hashcode_start_c;
            int hash2 = hash1;

            fixed (char* src = str)
            {
                var res = Internal_Hashcode((int*)src, hash1, hash2, str.Length);

                return res.start + res.end * hashcode_last_c;
            }
        }

        public static unsafe int GetFixedHashcode(this string str, int startidx, int count)
        {
            if ((startidx | count) < 0 ||
                (uint)(startidx + count) > (uint)str.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            int hash1 = hashcode_start_c;
            int hash2 = hash1;

            fixed (char* src = str)
            {
                var res = Internal_Hashcode((int*)(src + startidx), hash1, hash2, count);

                return res.start + res.end * hashcode_last_c;
            }
        }

        private static unsafe Range Internal_Hashcode(int* pint, int hash1, int hash2, int len)
        {
            for (int x = len; x > 2; x-= 4)
            {
                hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                pint += 2;
            }

            // x > 2 = x > 0010, if x=0011 -> x > 2 true -> x > 2 false -> x if > 0? true
            if ((len & 1) != 0)
                hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];

            return new Range(hash1, hash2);
        }
        #endregion
    }
}
