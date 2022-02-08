using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    internal sealed class List_Iterator<T> : IValueIterator<T>
    {
        private IList<T> inst;
        private int index;

        public List_Iterator(IList<T> inst) => this.inst = inst;

        public bool IsAlive => index < inst.Count;
        public int Count => inst.Count;
        public T Value
        {
            get => inst[index];
            set => inst[index] = value;
        }

        public void Add() => index++;

        public void Reset() => index = 0;

        public bool LazyAdd() => ++index < inst.Count;

        public T[] ToArray()
        {
            int max = inst.Count;
            T[] temp = new T[max];

            for (int x = 0; x < max; x++)
                temp[x] = inst[x];

            return temp;
        }

        public void Dispose()
        {
            inst = null;
            index = 0;

            GC.SuppressFinalize(this);
        }
    }
}
