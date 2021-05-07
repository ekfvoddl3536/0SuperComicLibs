using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class RangeForEachIterator<T> : IIterator<T>, IEnumerator<T>
    {
        private T[] arr;
        private int start;
        private int end;
        private int idx;

        public RangeForEachIterator(T[] arr, int start, int end)
        {
            this.arr = arr;
            this.start = start;
            this.end = end;
            idx = start;
        }

        #region iterator
        bool IIterator<T>.IsAlive => idx < end;

        int IIterator<T>.Count => end - start;

        void IIterator<T>.Add() => idx++;
        bool IIterator<T>.LazyAdd() => ++idx < end;

        T IIterator<T>.Value 
        {
            get => arr[idx];
            set => arr[idx] = value;
        }

        T[] IIterator<T>.ToArray() => arr;
        #endregion

        #region enumerator
        T IEnumerator<T>.Current => throw new NotImplementedException();
        object IEnumerator.Current => throw new NotImplementedException();

        bool IEnumerator.MoveNext() => throw new NotImplementedException();
        #endregion

        #region common
        public void Reset() => idx = start;

        public void Dispose()
        {
            arr = null;
            start = 0;
            end = 0;
            idx = 0;

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
