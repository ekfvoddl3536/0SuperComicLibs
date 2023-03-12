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
using System.Linq;

namespace SuperComicLib.Collections
{
    public static class ArrayExtension
    {
        // public static IValueIterator<T> Begin<T>(this T[] array) => new Array_Iterator<T>(array);
        // 
        // public static IValueIterator<T> RBegin<T>(this T[] array) => new Array_ReverseIterator<T>(array);

        public static void Replace<T>(this T[] array, int idx, int cnt, T replace)
        {
            int max = array.Length;
            int k = idx + cnt;
            if (idx >= 0 && k <= max)
            {
                int x = idx + 1;
                if (x < k)
                {
                    for (; k < max; x++, k++)
                        array[x] = array[k];

                    for (k -= cnt; cnt > 1; cnt--)
                        array[++k] = default;
                }

                array[idx] = replace;
            }
#if DEBUG
            else
                System.Diagnostics.Debug.Fail("invalid operation");
#endif
        }

        public static void ReplaceUnsafe<T>(this T[] array, int idx, int cnt, T replace)
        {
            int max = array.Length;
            int k = idx + cnt;
            int x = idx + 1;
            if (x < k)
            {
                int fmax = max - cnt;
                for (; k < fmax; x++, k++)
                    array[x] = array[k];

                for (; k < max; k++)
                    array[k] = default;
            }

            array[idx] = replace;
        }

        public static bool FastSequenceEqual<T>(this T[] array, T[] other) where T : IEquatable<T>
        {
            int x = array.Length;
            if (x != other.Length)
                return false;

            while (--x >= 0)
                if (!array[x].Equals(other[x]))
                    return false;

            return true;
        }

        public static IEnumeratorPair<T1, T2> MakePair<T1, T2>(this T1[] array, T2[] other) =>
            array.Length == other.Length
            ? new EnumerablePair<T1, T2>(array, other, other.Length)
            : null;

        public static IEnumeratorPair<T1, T2> MakePair<T1, T2>(this T1[] array, T2[] other, int max) =>
            array.Length.Min(max) == other.Length.Min(max)
            ? new EnumerablePair<T1, T2>(array, other, max)
            : null;

        public static T[] ToFlatArray<T>(this Array array)
        {
            Type type = array.GetType().GetElementType();
            if (type != typeof(T))
                throw new ArgumentException("Mismatch Type", nameof(array));
            else if (array.Rank <= 1)
                return (T[])array;

            int n = array.Length;

            T[] result = new T[n];

            IEnumerator<T> iterator = array.Cast<T>().GetEnumerator();

            while (--n >= 0 && iterator.MoveNext())
                result[n] = iterator.Current;

            iterator.Dispose();

            return result;
        }
    }
}
