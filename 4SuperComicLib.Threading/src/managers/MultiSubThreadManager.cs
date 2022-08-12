using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    internal sealed class MultiSubThreadManager : IDisposable
    {
        private Task[] tasks;
        private Action<int, IMultiWorker>[] methods;
        private volatile int nextMethodIndex;
        private readonly int sizeOfMethods;

        public MultiSubThreadManager(Action<int, IMultiWorker>[] taskMethods, int threadCount)
        {
            int msize = taskMethods.Length;

            methods = taskMethods;
            sizeOfMethods = msize;

            tasks = new Task[Math.Min(msize, threadCount)];
        }

        public IEnumerator StartTask()
        {
            Task[] vs = tasks;
            int len = vs.Length;

            ParallelWorker e = new ParallelWorker(len);
            for (int x = 0; x < len; x++)
                vs[x] = Task.Factory.StartNew(SubThread, new Package(e, x));

            return e;
        }

        private Action<int, IMultiWorker> TryGetNext()
        {
            int cachedIndex = Interlocked.Increment(ref nextMethodIndex);
            return
                cachedIndex >= sizeOfMethods
                ? null
                : methods[cachedIndex];
        }

        private void SubThread(object state)
        {
            Package p = (Package)state;

            int workerIndex = p.workerIndex;
            IMultiWorker inst = p.inst;

        loop:
            Action<int, IMultiWorker> current = TryGetNext();
            if (current != null)
            {
                current.Invoke(workerIndex, inst);
                goto loop;
            }

            if (p.inst.ExitThread())
                p.inst.Dispose();
        }

        public void Dispose()
        {
            if (tasks != null)
            {
                Task[] vs = tasks;
                int x = vs.Length;
                while (--x >= 0)
                {
                    Task t = vs[x];
                    if (!t.IsCompleted)
                        t.Wait();

                    t.Dispose();
                    vs[x] = null;
                }

                tasks = null;

                Action<int, IMultiWorker>[] acts = methods;
                x = acts.Length;
                while (--x >= 0)
                    acts[x] = null;

                methods = null;
            }
            GC.SuppressFinalize(this);
        }

        #region block
        private sealed class Package
        {
            public readonly ParallelWorker inst;
            public readonly int workerIndex;

            public Package(ParallelWorker inst, int workerIndex)
            {
                this.inst = inst;
                this.workerIndex = workerIndex;
            }
        }
        #endregion
    }
}
