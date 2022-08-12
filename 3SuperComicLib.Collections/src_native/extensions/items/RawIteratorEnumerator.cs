using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class RawIteratorEnumerator<T> : IEnumerator<T> 
        where T : unmanaged
    {
        private readonly RawIterator<T> start;
        private readonly RawIterator<T> end;
        private RawIterator<T> current;
        private T item;

        public RawIteratorEnumerator(IRawContainer<T> container)
        {
            current = start = container.begin();
            end = container.end();
        }

        public T Current => item;
        object IEnumerator.Current => item;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (current != end)
            {
                item = (T)current;
                current++;

                return true;
            }

            return false;
        }

        public void Reset() => current = start;
    }
}
