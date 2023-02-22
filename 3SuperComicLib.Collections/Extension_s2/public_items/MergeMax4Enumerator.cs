using System;
using System.Collections;
using System.Collections.Generic;

namespace SuperComicLib.Collections
{
    public sealed unsafe class MergeMax4Enumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> first;
        private readonly IEnumerator<T> second;
        private IEnumerator<T> opt_e1;
        private IEnumerator<T> opt_e2;
        private IEnumerator<T> current;
        private byte* state;

        public MergeMax4Enumerator(IEnumerator<T> first, IEnumerator<T> second)
        {
            this.first = first;
            this.second = second;
            opt_e1 = null;
            opt_e2 = null;
            current = first;
            state = null;
        }

        public IEnumerator<T> MergeCoreN(IEnumerator<T> other)
        {
            if (opt_e1 == null)
                opt_e1 = other;
            else if (opt_e2 == null)
                opt_e2 = other;
            else
                return new MergeMax4Enumerator<T>(this, other);

            return this;
        }

        public IEnumerator<T> AppendCore1(T element)
        {
            var last = opt_e2 ?? opt_e1 ?? second;
            if (last is ScalableEnumerator<T> ade)
            {
                ade.AppendCore1(element);
                return this;
            }

            return MergeCoreN(new ScalableEnumerator<T>(element));
        }

        public IEnumerator<T> PrependCore1(T element)
        {
            if (first is ScalableEnumerator<T> ade)
            {
                ade.PrependCore1(element);
                return this;
            }

            return new MergeMax4Enumerator<T>(new ScalableEnumerator<T>(element), this);
        }

        public IEnumerator<T> LastEnumerator() => opt_e2 ?? opt_e1 ?? second;

        T IEnumerator<T>.Current => current.Current;
        object IEnumerator.Current => current.Current;

        bool IEnumerator.MoveNext()
        {
            for (; !current.MoveNext(); ++state)
            {
                switch ((int)state)
                {
                    case 0:
                        current = second;
                        break;

                    case 1:
                        current = opt_e1;
                        break;

                    case 2:
                        current = opt_e2;
                        break;

                    default:
                        return false;
                }

                if (current == null)
                    return false;
            }

            return true;
        }

        void IEnumerator.Reset()
        {
            first.Reset();
            second.Reset();

            opt_e1?.Reset();
            opt_e2?.Reset();

            current = first;
            state = null;
        }

        void IDisposable.Dispose()
        {
            first.Dispose();
            second.Dispose();

            opt_e1?.Dispose();
            opt_e2?.Dispose();

            current = null;
        }
    }
}
