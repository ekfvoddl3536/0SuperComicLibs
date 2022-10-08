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

using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public static class ICollectionExtension
    {
        public static ICountObserver GetObserver(this ICollection collection) => new CountObserver(collection);

        public static ICountObserver GetObserver<T>(this ICollection<T> collection) => new CountObserver<T>(collection);

        public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            IEnumerator<T> e1 = other.GetEnumerator();
            while (e1.MoveNext())
                if (!collection.Contains(e1.Current))
                {
                    e1.Dispose();
                    return false;
                }

            e1.Dispose();
            return true;
        }

        public static bool IsEqualsAll<T>(this ICollection<T> collection, ICollection<T> other)
        {
            if (collection.Count != other.Count)
                return false;

            IEqualityComparer<T> comp = EqualityComparer<T>.Default;

            IEnumerator<T> e1 = collection.GetEnumerator();
            IEnumerator<T> e2 = other.GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext())
                if (!comp.Equals(e1.Current, e2.Current))
                    return false;

            return true;
        }
    }
}
