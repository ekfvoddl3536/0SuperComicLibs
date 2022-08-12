using System;
using System.Threading;

namespace SuperComicLib.Collections.Concurrent
{
    public sealed class ParallelEnumeratorDef<T> : ParallelEnumerate<T>
    {
        private Action<T, CancellationTokenSource> callback;
        private CancellationTokenSource cancellation;

        public ParallelEnumeratorDef(Action<T, CancellationTokenSource> callback)
        {
            this.callback = callback;

            cancellation = new CancellationTokenSource();
        }

        protected override bool OnEnumerate(T current, int thread_idx)
        {
            CancellationTokenSource cancellation = this.cancellation;

            callback.Invoke(current, cancellation);
            return !cancellation.IsCancellationRequested;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && cancellation != null)
            {
                callback = null;

                cancellation.Dispose();
                cancellation = null;
            }
        }
    }
}
