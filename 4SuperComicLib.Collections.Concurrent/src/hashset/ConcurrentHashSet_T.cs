using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SuperComicLib.Collections.Concurrent
{
    public class ConcurrentHashSet<T> : IDisposable, ICollection<T>, IEnumerable<T>, IEnumerable
        where T : unmanaged
    {
        #region 필드    
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
        private bool disposedValue = false; // 중복 호출을 검색하려면

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

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~ConcurrentHashSet()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(false);
        }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}