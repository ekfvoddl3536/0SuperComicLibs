using System;
using System.Collections;

namespace SuperComicLib.Threading
{
    public static class AwaitibleThreads
    {
        public static IEnumerator Create(Action<IWorker> taskMethod) => new AATW(taskMethod);

        private sealed class AATW : IEnumerator
        {
            private IEnumerator e;
            private AwaitibleAsyncTask inst;

            public AATW(Action<IWorker> taskMethod)
            {
                AwaitibleAsyncTask v = new AwaitibleAsyncTask();
                e = v.StartTask(taskMethod);
                inst = v;
            }

            public object Current => null;

            public bool MoveNext()
            {
                if (e.MoveNext())
                    return true;

                inst.Dispose();
                inst = null;

                e = null;

                return false;
            }

            public void Reset()
            {
            }
        }
    }
}
