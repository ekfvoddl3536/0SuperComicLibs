using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class SuperComic_WhereEnumeratorA00<T> : SuperComic_InnerEnumeratorBaseImpl<T>
    {
        private readonly Func<T, bool> predicate;

        public SuperComic_WhereEnumeratorA00(IEnumerator<T> src, Func<T, bool> predicate) : base(src) => this.predicate = predicate;

        public override bool MoveNext()
        {
            while (innerEnumerator.MoveNext())
                if (predicate.Invoke(innerEnumerator.Current))
                    return true;

            return false;
        }
    }
}
