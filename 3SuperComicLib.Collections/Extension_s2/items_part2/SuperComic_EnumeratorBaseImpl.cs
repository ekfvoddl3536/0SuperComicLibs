using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal abstract class SuperComic_InnerEnumeratorBaseImpl<T> : IEnumerator<T>
    {
        protected readonly IEnumerator<T> innerEnumerator;

        protected SuperComic_InnerEnumeratorBaseImpl(IEnumerator<T> innerEnumerator) => this.innerEnumerator = innerEnumerator;

        public virtual T Current => innerEnumerator.Current;
        object IEnumerator.Current => Current;

        public abstract bool MoveNext();
        public virtual void Reset() => innerEnumerator.Reset();

        protected virtual void Dispose(bool disposing) => innerEnumerator.Dispose();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SuperComic_InnerEnumeratorBaseImpl()
        {
            Dispose(false);
        }
    }
}
