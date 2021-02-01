using System;
using System.Collections;
using System.Threading;

namespace SuperComicLib.Threading
{
    internal sealed class MethodEnumerator : IDisposable, IEnumerator, IWorker
    {
        private AutoResetEvent mre;
        private Action act;
        private bool working;
        private bool skipOne;

        public MethodEnumerator()
        {
            mre = new AutoResetEvent(false);
            act = null;
            working = skipOne = true;
        }

        public object Current => null;

        public Awaiter Invoke(Action action)
        {
            act = action;
            skipOne = false;
            return new Awaiter(mre);
        }

        public bool MoveNext()
        {
            if (working)
            {
                if (!skipOne)
                {
                    skipOne = true;
                    act.Invoke();
                    mre.Set();
                }

                return true;
            }

            return false;
        }

        public void Reset() { }

        public void Dispose()
        {
            if (mre != null)
            {
                mre.Dispose();
                mre = null;

                act = null;
                working = skipOne = false;
            }
            GC.SuppressFinalize(this);
        }
    }
}
