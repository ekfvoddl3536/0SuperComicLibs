using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class BarrieredStack<T> : LookaheadStack<T>
    {
        private Stack<int> barrier;

        public BarrieredStack() : this(null)
        {
        }

        public BarrieredStack(T[] source) : base(source) => 
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

                int tmp = size;
                while (--tmp >= 0)
                    vs[--ncnt] = arr[tmp];

                return vs;
            }
            else
                return base.ToArray();
        }

        public override T[] ToArrayReverse()
        {
            T[] result = ToArray();
            result.ReverseFast();
            return result;
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