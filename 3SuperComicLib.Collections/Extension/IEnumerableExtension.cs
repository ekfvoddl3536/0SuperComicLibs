// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Collections
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> FastSelect<T>(this IEnumerable<T> collection, T item, IEqualityComparer<T> comparer) =>
            new Selector<T>(item, collection.GetEnumerator(), comparer);

        public static IEnumerable<T> FastSelect<T>(this IEnumerable<T> collection, T item) =>
            FastSelect(collection, item, EqualityComparer<T>.Default);

        public static IEnumerable<T> FastClone<T>(this IEnumerable<T> collection, Func<T, T> callback) =>
            new ElementCloner<T>(collection.GetEnumerator(), callback);

        public static bool ContainsAll<T>(this IEnumerable<T> collection, IEnumerable<T> items)
        {
            IEnumerator<T> src = collection.GetEnumerator();
            IEnumerator<T> other = items.GetEnumerator();

            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;

            while (other.MoveNext() && src.MoveNext())
                if (!comparer.Equals(other.Current, src.Current))
                {
                    src.Dispose();
                    other.Dispose();

                    return false;
                }

            src.Dispose();
            other.Dispose();

            return true;
        }

        public static IEnumerable<T> Limit<T>(this IEnumerable<T> collection, int limit) =>
            new LimitEnumerable<T>(collection, limit);

        public static IEnumerable<TOut> FastConvertAll<TIn, TOut>(this IEnumerable<TIn> collection, Converter<TIn, TOut> converter)
        {
            FastContract.Requires(collection != null);
            FastContract.Requires(converter != null);
            return new EnumerableConvert<TIn, TOut>(collection, converter);
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, T key) =>
            IndexOf(collection, key, EqualityComparer<T>.Default);

        public static int IndexOf<T>(this IEnumerable<T> collection, T key, IEqualityComparer<T> comparer)
        {
            int idx = 0;

            IEnumerator<T> e1 = collection.GetEnumerator();
            for (; e1.MoveNext(); idx++)
                if (comparer.Equals(e1.Current, key))
                    return idx;

            e1.Dispose();

            return -1;
        }

        // public static IValueIterable<T> AsIterable<T>(this IEnumerable<T> collection) =>
        //     new EnumerableToIterable<T>(collection);
    }
}
