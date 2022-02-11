using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperComicLib.Collections
{
    internal sealed class EnumerableToIterable<T> : IValueIterable<T>
    {
        private readonly IEnumerable<T> inst;

        public EnumerableToIterable(IEnumerable<T> inst) => this.inst = inst;

        public IValueIterator<T> Begin() => new Worker(inst);
        public IValueIterator<T> RBegin() => new Worker(inst.Reverse());
        public T[] ToArray() => inst.ToArray();

        private sealed class Worker : IValueIterator<T>
        {
            private IEnumerable<T> collection;
            private IEnumerator<T> enumerator;
            private T value;
            private int state;
            private int countCache;

            public Worker(IEnumerable<T> collection)
            {
                this.collection = collection;

                var e = collection.GetEnumerator();
                if (e.MoveNext())
                    value = e.Current;
                else
                    state = 2;

                enumerator = e;
                countCache = -1;
            }

            public bool IsAlive => state != 2;

            public int Count =>
                countCache < 0
                ? countCache = collection.Count()
                : countCache;

            public T Value
            {
                get => value;
                set => throw new NotSupportedException();
            }

            public void Add()
            {
                if (state == 0)
                {
                    if (enumerator.MoveNext())
                        value = enumerator.Current;
                    else
                        state = 1;
                }
                else
                    state = 2;
            }

            public void Reset()
            {
                var e = enumerator;

                e.Reset();

                if (e.MoveNext())
                {
                    value = e.Current;
                    state = 0;
                }
                else
                    state = 2;
            }

            public bool LazyAdd()
            {
                if (state == 0)
                {
                    if (enumerator.MoveNext())
                        value = enumerator.Current;
                    else
                        state = 1;

                    return true;
                }

                state = 2;
                return false;
            }

            public T[] ToArray() => collection.ToArray();

            public void Dispose()
            {
                if (collection != null)
                {
                    collection = null;
                    
                    enumerator.Dispose();
                    enumerator = null;

                    value = default;

                    state = 0;
                    countCache = 0;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}