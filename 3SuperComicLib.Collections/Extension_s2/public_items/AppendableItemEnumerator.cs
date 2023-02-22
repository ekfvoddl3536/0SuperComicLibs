using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed class ScalableEnumerator<T> : IEnumerator<T>
    {
        private SingleLinkedNode<T> first;
        private SingleLinkedNode<T> last;
        private SingleLinkedNode<T> pcur;

        public ScalableEnumerator(T element)
        {
            first = new SingleLinkedNode<T>(element);
            last = first;
            pcur = first;
        }

        public IEnumerator<T> AppendCore1(T element)
        {
            var item = new SingleLinkedNode<T>(element);

            last.next = item;
            last = item;

            return this;
        }

        public IEnumerator<T> PrependCore1(T element)
        {
            var item = new SingleLinkedNode<T>(first, element);

            if (pcur == first)
                pcur = item;

            first = item;

            return this;
        }

        T IEnumerator<T>.Current => pcur.value;
        object IEnumerator.Current => pcur.value;

        bool IEnumerator.MoveNext()
        {
            var after = pcur;
            if (after != null)
            {
                pcur = after.next;
                return true;
            }

            return false;
        }

        void IEnumerator.Reset() => pcur = first;

        void IDisposable.Dispose()
        {
            // cut reference
            for (var p = first; p != null;)
            {
                var t = p.next;
                p.next = null;

                GC.SuppressFinalize(p);

                p = t;
            }

            GC.SuppressFinalize(this);
        }
    }
}
