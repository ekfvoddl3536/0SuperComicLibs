using System;
using System.Collections;

namespace SuperComicLib.Threading
{
    internal abstract class WorkerBase : IDisposable, IEnumerator
    {
        protected bool working;
        protected bool skipOne;

        public object Current => null;

        public void Reset() { }

        public bool MoveNext()
        {
            if (!skipOne)
            {
                skipOne = true;
                OnProcess();
            }

            return working;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => working = false;

        protected abstract void OnProcess();
    }
}
