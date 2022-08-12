using SuperComicLib.Threading;
using System;
using System.Collections.Generic;

namespace SuperComicLib.Collections.Concurrent
{
    public sealed class ConcurrentEnumerator<T>
    {
        private readonly IEnumerator<T> source;
        private SpinBlockLockSlim blockLock;

        public ConcurrentEnumerator(IEnumerator<T> enumerator)
        {
            System.Diagnostics.Contracts.Contract.Requires(enumerator != null);

            source = enumerator;
        }

        public bool MoveNext(out T item)
        {
            IEnumerator<T> src = source;

            ref SpinBlockLockSlim block = ref blockLock;

            block.Enter();
            try
            {
                bool result;
                if (result = src.MoveNext())
                    item = src.Current;
                else
                    item = default;

                block.Exit();

                return result;
            }
            catch (Exception exc)
            {
                block.Exit();
                throw exc;
            }
        }

        public void Reset()
        {
            ref SpinBlockLockSlim block = ref blockLock;

            block.Enter();
            try
            {
                source.Reset();
                block.Exit();
            }
            catch (Exception exc)
            {
                block.Exit();
                throw exc;
            }
        }

        public void Dispose()
        {
            source.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
