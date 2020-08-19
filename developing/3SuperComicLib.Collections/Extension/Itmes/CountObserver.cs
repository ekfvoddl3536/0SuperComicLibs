using System;
using System.Collections;

namespace SuperComicLib.Collections
{
    internal readonly struct CountObserver : IUniObserver
    {
        private readonly ICollection collection;
        private readonly int prev_cnt;

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

        public void Observe(object target)
        {
#if DEBUG
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            throw new NotSupportedException();
#endif
        }
    }
}
