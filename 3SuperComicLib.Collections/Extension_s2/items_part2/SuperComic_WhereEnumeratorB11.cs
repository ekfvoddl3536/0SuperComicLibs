using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class SuperComic_WhereEnumeratorB11<T> : SuperComic_InnerEnumeratorBaseImpl<T>
    {
        private readonly Func<T, int, bool> predicate;
        private int index;

        public SuperComic_WhereEnumeratorB11(IEnumerator<T> src, Func<T, int, bool> predicate) : base(src)
        {
            this.predicate = predicate;
            index = 0;
        }

        public override bool MoveNext()
        {
            for (; innerEnumerator.MoveNext(); ++index)
                if (predicate.Invoke(innerEnumerator.Current, index))
                    return true;

            return false;
        }

        public override void Reset()
        {
            base.Reset();
            index = 0;
        }
    }
}
