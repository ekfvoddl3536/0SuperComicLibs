using System;
using System.Threading;

namespace SuperComicLib.Threading
{
    internal sealed class SimpleThreadWorker : WorkerBase, IWorker
    {
        private AutoResetEvent are;
        private Action act;

        public SimpleThreadWorker()
        {
            are = new AutoResetEvent(false);
            act = null;
            working = skipOne = true;
        }

        public Awaiter Invoke(Action action)
        {
            act = action;
            skipOne = false;
            return new Awaiter(are);
        }

        protected override void OnProcess()
        {
            act.Invoke();
            are.Set();
        }

        protected override void Dispose(bool disposing)
        {
            if (are != null)
            {
                are.Dispose();
                are = null;

                act = null;

                base.Dispose(disposing);
            }
        }
    }
}
