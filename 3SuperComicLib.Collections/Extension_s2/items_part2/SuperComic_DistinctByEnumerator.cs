using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class SuperComic_DistinctByEnumerator<TSource, TKey> : SuperComic_InnerEnumeratorBaseImpl<TSource>
    {
        private readonly Func<TSource, TKey> func;
        private readonly HashSet<TKey> set;

        public SuperComic_DistinctByEnumerator(IEnumerator<TSource> innerEnumerator, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer) : base(innerEnumerator)
        {
            this.func = func;
            set = new HashSet<TKey>(comparer);
        }

        public override bool MoveNext()
        {
            while (innerEnumerator.MoveNext())
                if (set.Add(func.Invoke(innerEnumerator.Current)))
                    return true;

            return false;
        }
    }
}
