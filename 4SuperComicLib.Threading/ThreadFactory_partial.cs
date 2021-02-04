using System;
using System.Collections;

namespace SuperComicLib.Threading
{
    partial class ThreadFactory
    {
        #region classes
        #region base
        private abstract class I : IEnumerator, IDisposable
        {
            private IEnumerator e;

            protected void Init(IEnumerator e) => this.e = e;

            public object Current => null;

            public bool MoveNext()
            {
                if (e.MoveNext())
                    return true;

                Dispose();
                return false;
            }

            public virtual void Reset() { }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (e is IDisposable ed)
                    ed.Dispose();

                e = null;
            }
        }
        #endregion

        #region e0
        private sealed class E0 : I
        {
            private SingleSubThreadManager inst;

            public E0(Action<IWorker> taskMethod)
            {
                SingleSubThreadManager v = new SingleSubThreadManager();
                inst = v;
                Init(v.StartTask(taskMethod));
            }

            protected override void Dispose(bool disposing)
            {
                if (inst != null)
                {
                    inst.Dispose();
                    inst = null;

                    base.Dispose(disposing);
                }
            }
        }
        #endregion

        #region e1
        private sealed class E1 : I
        {
            private MultiSubThreadManager inst;

            public E1(Action<int, IMultiWorker>[] taskMethods, int threadSize)
            {
                MultiSubThreadManager v = new MultiSubThreadManager(taskMethods, threadSize);
                inst = v;
                Init(v.StartTask());
            }

            protected override void Dispose(bool disposing)
            {
                if (inst != null)
                {
                    inst.Dispose();
                    inst = null;

                    base.Dispose(disposing);
                }
            }
        }
        #endregion
        #endregion
    }
}
