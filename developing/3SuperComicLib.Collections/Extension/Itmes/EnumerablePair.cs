using System;

namespace SuperComicLib.Collections
{
    internal sealed class EnumerablePair<T1, T2> : IEnumerablePair<T1, T2>
    {
        private T1[] array;
        private T2[] other;
        private int idx;
        private int max;

        public EnumerablePair(T1[] array, T2[] other, int max)
        {
            this.array = array;
            this.other = other;

            this.max = max;
            idx = -1;
        }

        public T1 Item1 => array[idx];
        public T2 Item2 => other[idx];

        public bool MoveNext() => ++idx < max;

        public void Reset() => idx = -1;

        public void Dispose()
        {
            array = null;
            other = null;
            idx = 0;
            max = 0;

            GC.SuppressFinalize(this);
        }
    }
}