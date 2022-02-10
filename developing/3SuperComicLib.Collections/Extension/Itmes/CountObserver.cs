using System;
using System.Collections;

namespace SuperComicLib.Collections
{
    internal sealed class CountObserver : ICountObserver
    {
        private ICollection collection;
        private int prev_cnt;

        public CountObserver(ICollection collection)
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
            if (target is ICollection col)
            {
                collection = col;
                prev_cnt = col.Count;
            }
        }

        public void Update() => prev_cnt = collection.Count;
    }
}
