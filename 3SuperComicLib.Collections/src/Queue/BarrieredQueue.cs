using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class BarrieredQueue<T> : LookaheadQueue<T>
    {
        private Stack<int> barrier;

        public BarrieredQueue() : this (null)
        {
        }

        public BarrieredQueue(T[] source) : base(source) =>
            barrier = new Stack<int>(16);

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

        public override T[] ToArray()
        {
            loop:
            if (barrier.Count > 0)
            {
                int ncnt = size - barrier.Pop();
                if (ncnt <= 0)
                    goto loop;

                T[] vs = new T[ncnt];
                T[] arr = this.arr;

                int x, y;
                x = y = head;

                int len = arr.Length;
                for (int z = 0; z < ncnt;)
                    vs[z++] = arr[(x + y++) % len];

                return vs;
            }
            else
                return base.ToArray();
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