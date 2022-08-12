using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SuperComicLib.Collections.Concurrent
{
    public class ConcurrentHashSet<T> : IDisposable, ICollection<T>, IEnumerable<T>, IEnumerable
        where T : unmanaged
    {
        #region �ʵ�    
        protected HashSet<T> cache;

        protected ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        #endregion
        public ConcurrentHashSet() => cache = new HashSet<T>();
        public ConcurrentHashSet(IEqualityComparer<T> comparer) => cache = new HashSet<T>(comparer);
        public ConcurrentHashSet(IEnumerable<T> collection) => cache = new HashSet<T>(collection);
        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) => cache = new HashSet<T>(collection, comparer);

        public int Count => cache.Count;

        bool ICollection<T>.IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => cache.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => cache.GetEnumerator();

        public virtual void Add(T item)
        {
            rwls.EnterWriteLock();
            try { cache.Add(item); }
            finally
            {
                rwls.ExitWriteLock();
            }
        }

        public virtual void Clear()
        {
            rwls.EnterWriteLock();
            try { cache.Clear(); }
            finally
            {
                rwls.ExitWriteLock();
            }
        }

        public virtual bool Contains(T item)
        {
            rwls.EnterReadLock();
            try { return cache.Contains(item); }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public virtual void CopyTo(T[] array)
        {
            rwls.EnterReadLock();
            try { cache.CopyTo(array); }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            rwls.EnterReadLock();
            try { cache.CopyTo(array, arrayIndex); }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public virtual void CopyTo(T[] array, int arrayIndex, int count)
        {
            rwls.EnterReadLock();
            try { cache.CopyTo(array, arrayIndex, count); }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public virtual bool Remove(T item)
        {
            rwls.EnterWriteLock();
            try { return cache.Remove(item); }
            finally
            {
                rwls.ExitWriteLock();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // �ߺ� ȣ���� �˻��Ϸ���

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                rwls.Dispose();
                cache.Clear();

                rwls = null;
                cache = null;

                disposedValue = true;
            }
        }

        // TODO: ���� Dispose(bool disposing)�� �������� �ʴ� ���ҽ��� �����ϴ� �ڵ尡 ���ԵǾ� �ִ� ��쿡�� �����ڸ� �������մϴ�.
        ~ConcurrentHashSet()
        {
            // �� �ڵ带 �������� ������. ���� Dispose(bool disposing)�� ���� �ڵ带 �Է��ϼ���.
            Dispose(false);
        }

        // ���� ������ ������ �ùٸ��� �����ϱ� ���� �߰��� �ڵ��Դϴ�.
        public void Dispose()
        {
            // �� �ڵ带 �������� ������. ���� Dispose(bool disposing)�� ���� �ڵ带 �Է��ϼ���.
            Dispose(true);
            // TODO: ���� �����ڰ� �����ǵ� ��� ���� �ڵ� ���� �ּ� ó���� �����մϴ�.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}