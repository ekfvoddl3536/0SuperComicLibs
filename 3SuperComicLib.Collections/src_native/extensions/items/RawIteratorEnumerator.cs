using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed unsafe class RawIteratorEnumerator<T> : IEnumerator<T> 
        where T : unmanaged
    {
        private readonly T* start;
        private readonly T* end;
        private T* current;
        private T item;

        public RawIteratorEnumerator(IRawContainer<T> container)
        {
            current = start = (T*)container.begin().UnsafePointerValue;
            end = (T*)container.end().UnsafePointerValue;
        }

        public T Current => item;
        object IEnumerator.Current => item;

        public void Dispose() { }

        public bool MoveNext()
        {
            if (current != end)
            {
                item = *current++;
                return true;
            }

            return false;
        }

        public void Reset() => current = start;
    }
}
