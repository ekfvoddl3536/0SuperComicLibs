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

using System.Collections.Generic;

namespace SuperComicLib.Collections.Concurrent
{
    public sealed class ConcurrentHashSetSlim<T> : ConcurrentHashSet<T>
        where T : unmanaged
    {
        public ConcurrentHashSetSlim()
        {
        }

        public ConcurrentHashSetSlim(IEnumerable<T> collection) : base(collection)
        {
        }

        public ConcurrentHashSetSlim(IEqualityComparer<T> comparer) : base(comparer)
        {
        }

        public ConcurrentHashSetSlim(IEnumerable<T> collection, IEqualityComparer<T> comparer) : base(collection, comparer)
        {
        }

        public override void Add(T item)
        {
            rwls.EnterWriteLock();
            cache.Add(item);
            rwls.ExitWriteLock();
        }

        public override void Clear()
        {
            rwls.EnterWriteLock();
            cache.Clear();
            rwls.ExitWriteLock();
        }

        public override bool Contains(T item)
        {
            rwls.EnterReadLock();
            bool result = cache.Contains(item);
            rwls.ExitReadLock();
            return result;
        }

        public override void CopyTo(T[] array)
        {
            rwls.EnterReadLock();
            cache.CopyTo(array);
            rwls.ExitReadLock();
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            rwls.EnterReadLock();
            cache.CopyTo(array, arrayIndex);
            rwls.ExitReadLock();
        }

        public override void CopyTo(T[] array, int arrayIndex, int count)
        {
            rwls.EnterReadLock();
            cache.CopyTo(array, arrayIndex, count);
            rwls.ExitReadLock();
        }

        public override bool Remove(T item)
        {
            rwls.EnterWriteLock();
            bool result = cache.Remove(item);
            rwls.ExitWriteLock();
            return result;
        }
    }
}