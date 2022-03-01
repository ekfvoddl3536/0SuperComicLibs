using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperComicLib.Collections
{
    internal sealed class List_Iterator<T> : IValueIterator<T>
    {
        private IList<T> inst;
        private int index;
        private T value;

        public List_Iterator(IList<T> inst)
        {
            this.inst = inst;
            Reset();
        }

        public bool IsAlive => index < inst.Count;
        public int Count => inst.Count;
        public ref T Value => ref value;

        public void Add() => inst[index++] = value; // apply previous value

        public bool LazyAdd()
        {
            inst[index++] = value; // apply previous value
            return index < inst.Count;
        }

        public void Reset() =>
            value =
                inst.Count == (index = 0)
                ? default(T)
                : inst[0];

        public T[] ToArray() => inst.ToArray();

        public void Dispose()
        {
            inst = null;
            index = 0;

            GC.SuppressFinalize(this);
        }
    }
}
