using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public class BarrieredStack<T> : LookaheadStack<T>
    {
        protected Stack<int> barrier;

        public BarrieredStack() : this(null)
        {
        }

        public BarrieredStack(T[] source) : base(source) => 
            barrier = new Stack<int>();

        public BarrieredStack(int initalCapacity) : base(initalCapacity) =>
            barrier = new Stack<int>();

        public void MarkPoint() => barrier.Push(size);

        public void UnmarkPoint()
        {
            if (barrier.Count > 0)
                barrier.Pop();
        }

        public void RemoveAuto()
        {
            if (barrier.Count > 0)
            {
                int ncnt = barrier.Pop();
                while (size > ncnt)
                    arr[--size] = default;
            }
            else
                Clear();
        }

        public virtual T[] PopAuto()
        {
            T[] vs, arr = this.arr;
            int x, ncnt;
        loop:
            if (barrier.Count > 0)
            {
                ncnt = size - barrier.Pop();
                if (ncnt == 0)
                    return Array.Empty<T>();
                else if (ncnt < 0)
                    goto loop;

                vs = new T[ncnt];
                x = size - ncnt;
                while (--ncnt >= 0)
                    vs[ncnt] = arr[x++];
            }
            else
            {
                ncnt = size;
                vs = new T[ncnt];

                for (x = 0; x < ncnt;)
                    vs[x++] = Pop();
            }

            return vs;
        }

        protected override void Dispose(bool disposing)
        {
            if (barrier != null)
            {
                barrier.Clear();
                barrier = null;
            }
            base.Dispose(disposing);
        }
    }
}