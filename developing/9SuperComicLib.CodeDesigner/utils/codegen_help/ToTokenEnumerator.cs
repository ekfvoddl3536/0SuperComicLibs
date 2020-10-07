using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public sealed class ToTokenEnumerator : IEnumerator<Token>
    {
        internal Stack<INodeEnumerator> stack;
        internal INodeEnumerator ne;
        internal ExNode.Enumerator e1;
        internal int save;
        internal Token current;

        public ToTokenEnumerator(INodeEnumerator ne, bool deep, bool applyMove)
        {
            this.ne = ne;
            if (deep && ne.Index > ne.TokenCount)
            {
                e1 = (ExNode.Enumerator)ne;
                save =
                    applyMove
                    ? -1
                    : e1.index;
            }

            stack = new Stack<INodeEnumerator>();
        }

        public Token Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (e1 == null) // ok
                if (ne.MoveNext())
                {
                    current = ((TokNode)ne.Current).GetToken();
                    return true;
                }
                else
                    return false;

        loop:
            if (ne.MoveNext())
            {
                if (ne.Current.ChildCount == 0)
                {
                    current = ((TokNode)ne.Current).GetToken();
                    return true;
                }
                else
                {
                    stack.Push(ne);
                    ne = ne.Current.GetEnumerator();
                }
                goto loop;
            }

            if (stack.Count > 0)
            {
                ne.Dispose();
                ne = stack.Pop();
                goto loop;
            }

            if (save >= 0)
                e1.index = save;

            return false;
        }

        public void Reset() { }

        public void Dispose()
        {
            if (stack != null)
            {
                while (stack.Count > 0)
                    stack.Pop().Dispose();

                stack = null;

                ne = null;
                e1 = null;

                current = null;

                save = 0;
            }
            GC.SuppressFinalize(this);
        }
    }
}
