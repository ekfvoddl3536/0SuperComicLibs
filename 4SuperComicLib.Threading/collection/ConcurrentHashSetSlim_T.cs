using System.Collections.Generic;

namespace SuperComicLib.Collection
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