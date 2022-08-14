using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperComicLib.Collections
{
    internal sealed class List_ReverseIterator<T> : IValueIterator<T>
    {
        private IList<T> inst;
        private int index;
        private T value;

        public List_ReverseIterator(IList<T> inst)
        {
            this.inst = inst;
            index = inst.Count - 1;
        }

        public bool IsAlive => index >= 0;
        public int Count => inst.Count;
        public ref T Value => ref value;

        public void Add() => inst[index--] = value; // apply previous value

        public void Reset() => 
            value =
                (index = inst.Count - 1) >= 0
                ? inst[index]
                : default(T);

        public bool LazyAdd()
        {
            inst[index--] = value; // apply previous value
            return index < inst.Count;
        }

        public T[] ToArray() => inst.ToArray();

        public void Dispose()
        {
            inst = null;
            index = 0;

            GC.SuppressFinalize(this);
        }
    }
}
