using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    #region wrapper
    internal readonly struct IStackWrapper<T> : IStack<T>
    {
        private readonly Stack<T> inst;

        public IStackWrapper(Stack<T> inst) => this.inst = inst;

        #region un use
        public IEnumerator<T> GetEnumerator() => null;
        IEnumerator IEnumerable.GetEnumerator() => null;
        public int Capacity => 0;
        public T Peek() => default;
        public void Push(T item) { }
        public void Clear() { }
        public void Dispose() { }
        #endregion

        public int Count => inst.Count;

        public T Pop() => inst.Pop();
    }
    #endregion

    internal sealed class StackPopEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IStack<T> v;
        private T current;

        public StackPopEnumerable(Stack<T> s) => 
            v = new IStackWrapper<T>(s);

        public StackPopEnumerable(IStack<T> s) =>
            v = s;

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public T Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (v.Count > 0)
            {
                current = v.Pop();
                return true;
            }

            return false;
        }

        public void Reset() => throw new InvalidOperationException("can't reset");

        public void Dispose()
        {
            v = null;
            current = default;

            GC.SuppressFinalize(this);
        }
    }
}