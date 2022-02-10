using System;

namespace SuperComicLib.Collections
{
    public sealed class InternalTypeStack : BarrieredStack<Type>
    {
        public InternalTypeStack()
        {
        }

        public InternalTypeStack(int initalCapacity) : base(initalCapacity)
        {
        }

        public InternalTypeStack(Type[] source) : base(source)
        {
        }

        /// <summary>
        /// mark point부터 입력된 순서대로 반환됩니다
        /// </summary>
        /// <returns>일반 List또는 Queue형식과 같은 FIFO 구조의 배열</returns>
        public override Type[] PopAuto()
        {
            Type[] vs, arr = this.arr;
            int x, ncnt;
        loop:
            if (barrier.Count > 0)
            {
                ncnt = size - barrier.Pop();
                if (ncnt == 0)
                    return Type.EmptyTypes;
                else if (ncnt < 0)
                    goto loop;

                vs = new Type[ncnt];
                x = size;
                while (--ncnt >= 0)
                    vs[ncnt] = arr[--x];
            }
            else
            {
                ncnt = size;
                vs = new Type[ncnt];

                while (--ncnt >= 0)
                    vs[ncnt] = Pop();
            }

            return vs;
        }
    }
}
