using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class CountObserver<T> : ICountObserver
    {
        private ICollection<T> collection;
        private int prev_cnt;

        public CountObserver(ICollection<T> collection)
        {
            this.collection = collection;
            prev_cnt = collection.Count;
        }

        public bool IsNotUpdated => prev_cnt == collection.Count;
        public bool IsUpdated => prev_cnt != collection.Count;
        public bool IsAdded => prev_cnt < collection.Count;
        public bool IsRemoved => prev_cnt > collection.Count;
        public bool IsDisposed => false;
        public int Count => collection.Count;

        public void Observe(object target)
        {
#if DEBUG
            if (target == null)
                throw new ArgumentNullException(nameof(target));
#endif
            if (target is ICollection<T> col)
            {
                collection = col;
                prev_cnt = col.Count;
            }
        }

        public void Update() => prev_cnt = collection.Count;
    }
}
