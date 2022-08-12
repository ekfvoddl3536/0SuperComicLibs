using System;
using System.Collections;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    internal sealed class SingleSubThreadManager : IDisposable
    {
        private Task task;

        public IEnumerator StartTask(Action<IWorker> asyncMethod)
        {
            SimpleThreadWorker e = new SimpleThreadWorker();
            task = Task.Factory.StartNew(SubThread, new Package(e, asyncMethod));
            return e;
        }

        private static void SubThread(object state)
        {
            Package p = (Package)state;

            p.method.Invoke(p.inst);
            p.inst.Dispose();
        }

        public void Dispose()
        {
            if (task != null)
            {
                Task t = task;
                if (!t.IsCompleted)
                    t.Wait();

                t.Dispose();
                task = null;
            }
            GC.SuppressFinalize(this);
        }

        #region block
        private sealed class Package
        {
            public readonly SimpleThreadWorker inst;
            public readonly Action<IWorker> method;

            public Package(SimpleThreadWorker inst, Action<IWorker> method)
            {
                this.inst = inst;
                this.method = method;
            }
        }
        #endregion
    }
}
