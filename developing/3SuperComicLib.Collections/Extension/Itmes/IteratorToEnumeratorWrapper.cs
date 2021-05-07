using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class IteratorToEnumeratorWrapper<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IIterator<T> inst;
        private T current;

        public IteratorToEnumeratorWrapper(IIterator<T> inst) => this.inst = inst;

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public T Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (inst.IsAlive)
            {
                current = inst.Value;

                inst.Add();
                return true;
            }

            return false;
        }

        public void Reset() => inst.Reset();

        public void Dispose()
        {
            if (inst != null)
            {
                inst.Dispose();
                inst = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}