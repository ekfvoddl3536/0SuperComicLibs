using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SuperComicLib.Threading
{
    internal sealed unsafe class ParallelWorker : WorkerBase, IMultiWorker
    {
        private AutoResetEvent[] ares;
        private Action[] actions;
        private ConcurrentStack<int> indexes;
        private volatile int runningThreads;

        public ParallelWorker(int size)
        {
            indexes = new ConcurrentStack<int>();
            actions = new Action[size];

            runningThreads = size;

            AutoResetEvent[] vs = new AutoResetEvent[size];
            while (--size >= 0)
                vs[size] = new AutoResetEvent(false);

            ares = vs;

        }

        public bool ExitThread() => Interlocked.Decrement(ref runningThreads) <= 0;

        public Awaiter Invoke(int handle, Action action)
        {
            actions[handle] = action;
            skipOne = false;

            indexes.Push(handle);

            return new Awaiter(ares[handle]);
        }

        protected override void OnProcess()
        {
            AutoResetEvent[] ares = this.ares;
            Action[] actions = this.actions;
            ConcurrentStack<int> indexes = this.indexes;

            while (indexes.TryPop(out int idx))
            {
                actions[idx].Invoke();
                ares[idx].Set();
            }
        }

        protected override void Dispose(bool disposing)
        {
#if DEBUG
            if (runningThreads > 0)
                throw new InvalidOperationException();
#endif
            if (ares != null)
            {
                int length = ares.Length;
                int k = 0;
                for (; k < length; k++)
                {
                    ares[k].Dispose();
                    ares[k] = null;
                }

                ares = null;

                for (k = 0; k < length; k++)
                    actions[k] = null;

                actions = null;

                indexes.Clear();
                indexes = null;

                base.Dispose(disposing);
            }
        }
    }
}
