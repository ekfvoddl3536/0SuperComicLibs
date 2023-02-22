using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class SuperComic_DistinctEnumerator<T> : SuperComic_InnerEnumeratorBaseImpl<T>
    {
        private readonly HashSet<T> set;

        public SuperComic_DistinctEnumerator(IEnumerator<T> innerEnumerator, IEqualityComparer<T> comparer) : base(innerEnumerator) =>
            set = new HashSet<T>(comparer);

        public override bool MoveNext()
        {
            while (innerEnumerator.MoveNext())
                if (set.Add(innerEnumerator.Current))
                    return true;

            return false;
        }
    }
}
