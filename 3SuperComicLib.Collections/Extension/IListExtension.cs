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

using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class IListExtension
    {
        // public static IValueIterator<T> Begin<T>(this IList<T> item) => new List_Iterator<T>(item);
        // 
        // public static IValueIterator<T> RBegin<T>(this IList<T> item) => new List_ReverseIterator<T>(item);

        public static IAddOnlyList<T> AsAddOnly<T>(this IList<T> item) => new List_AddOnlyWrapper<T>(item);

        public static void RemoveBack<T>(this IList<T> item, int count)
        {
            int idx = item.Count;
            int max = idx - count;

            while (idx > max)
            {
                idx--;
                item[idx] = default;
            }
        }

        public static void ReplaceAll_C<T>(this IList<T> item, T replace, int begin, int __COUNT__)
        {
            int max = item.Count.Min(begin + __COUNT__);
            for (; begin < max; begin++)
                item[begin] = replace;
        }

        public static void ReplaceAll<T>(this IList<T> item, T replace, int begin, int __END__)
        {
            int max = (item.Count - 1).Min(__END__);
            for (; begin <= max; begin++)
                item[begin] = replace;
        }

        public static void ReplaceRange_C<T>(this IList<T> item, T replace, int begin, int __COUNT__)
        {
            if (begin < item.Count)
            {
                item[begin] = replace;

                int max = item.Count.Min(begin + __COUNT__);
                for (begin++; begin < max; begin++)
                    item.RemoveAt(begin);
            }
        }

        public static void ReplaceRange<T>(this IList<T> item, T replace, int begin, int __END__)
        {
            if (begin < item.Count)
            {
                item[begin] = replace;

                int max = (item.Count - 1).Min(__END__);
                for (begin++; begin <= max; begin++)
                    item.RemoveAt(begin);
            }
        }
    }
}
